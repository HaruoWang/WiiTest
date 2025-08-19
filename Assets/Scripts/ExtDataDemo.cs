using UnityEngine;
using WiimoteApi;
using WiimoteApi.Util;

public class ExtDataDemo : MonoBehaviour
{
    private Wiimote wiimote;

    private const int PanelWidth = 300;
    private const int PanelPadding = 10;
    private const int LineHeight = 24;

    private Rect windowRect = new Rect(320, 20, 470, 300);
    private Vector2 scrollPosition = Vector2.zero;

    private bool extensionInitialized = false;

    void Update()
    {
        if (!WiimoteManager.HasWiimote()) return;

        if (WiimoteManager.Wiimotes.Count > 0 && wiimote == null)
        {
            wiimote = WiimoteManager.Wiimotes[0];
            extensionInitialized = false;
        }

        if (wiimote != null && !extensionInitialized)
        {
            Debug.Log("Wiimote connected, initializing extension...");
            wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_EXT16);
            wiimote.SendStatusInfoRequest();
            extensionInitialized = true;
        }

        if (wiimote != null)
        {
            int ret;
            do { ret = wiimote.ReadWiimoteData(); }
            while (ret > 0);
        }
    }

    void OnGUI()
    {
        GUI.Box(new Rect(0, 0, PanelWidth, Screen.height), "");

        float x = PanelPadding;
        float y = PanelPadding;

        GUI.Label(new Rect(x, y, PanelWidth, LineHeight), "Wiimote Found: " + WiimoteManager.HasWiimote());
        y += LineHeight;
        GUI.Label(new Rect(x, y, PanelWidth, LineHeight), "Count: " + WiimoteManager.Wiimotes.Count);
        y += LineHeight + 6;

        if (GUI.Button(new Rect(x, y, PanelWidth - 2 * PanelPadding, LineHeight), "Find Wiimote"))
        {
            WiimoteManager.FindWiimotes();
            if (WiimoteManager.HasWiimote() && WiimoteManager.Wiimotes.Count > 0)
            {
                wiimote = WiimoteManager.Wiimotes[0];
                extensionInitialized = false;
            }
        }
        y += LineHeight + 4;

        if (GUI.Button(new Rect(x, y, PanelWidth - 2 * PanelPadding, LineHeight), "Cleanup"))
        {
            if (wiimote != null)
            {
                WiimoteManager.Cleanup(wiimote);
                wiimote = null;
                extensionInitialized = false;
            }
        }
        y += LineHeight + 10;

        GUI.Label(new Rect(x, y, 100, LineHeight), "LED Test:");
        y += LineHeight;

        float btnW = (PanelWidth - 2 * PanelPadding - 6) / 4f;
        for (int i = 0; i < 4; i++)
        {
            if (GUI.Button(new Rect(x + i * (btnW + 2), y, btnW, LineHeight), i.ToString()))
            {
                if (wiimote != null)
                    wiimote.SendPlayerLED(i == 0, i == 1, i == 2, i == 3);
            }
        }
        y += LineHeight + 10;

        if (wiimote != null && wiimote.Type == WiimoteType.PROCONTROLLER)
        {
            float[] ls = wiimote.WiiUPro?.GetLeftStick01();
            float[] rs = wiimote.WiiUPro?.GetRightStick01();

            if (ls != null && rs != null)
            {
                GUI.Label(new Rect(x, y, PanelWidth, LineHeight), $"LS: {ls[0]:0.00}, {ls[1]:0.00}");
                y += LineHeight;
                GUI.Label(new Rect(x, y, PanelWidth, LineHeight), $"RS: {rs[0]:0.00}, {rs[1]:0.00}");
                y += LineHeight;
            }
        }

        if (wiimote != null)
            windowRect = GUI.Window(0, windowRect, DataWindow, "Data");
    }

    void DataWindow(int id)
    {
        if (wiimote == null)
        {
            GUI.Label(new Rect(10, 25, 200, 20), "No Wiimote Connected");
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
            return;
        }

        ReadOnlyArray<byte> data;
        try
        {
            data = wiimote.RawExtension;
            if (data == null || data.Length == 0)
            {
                GUI.Label(new Rect(10, 25, 200, 20), "No Extension Data");
                GUI.DragWindow(new Rect(0, 0, 10000, 20));
                return;
            }
        }
        catch
        {
            GUI.Label(new Rect(10, 25, 200, 20), "RawExtension not ready");
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
            return;
        }

        float left = 10f;
        float top = 25f;
        float rowH = 20f;
        float colW = 40f;

        GUI.Label(new Rect(left, top, colW, rowH), "##");
        GUI.Label(new Rect(left + colW, top, colW, rowH), "Val");
        for (int b = 7; b >= 0; b--)
            GUI.Label(new Rect(left + colW * (2 + (7 - b)), top, colW, rowH), b.ToString());

        float viewX = left;
        float viewY = top + rowH + 5f;
        float viewW = 440f;
        float viewH = 230f;

        float contentW = colW * 10f;
        float contentH = data.Length * rowH;

        scrollPosition = GUI.BeginScrollView(new Rect(viewX, viewY, viewW, viewH),
                                             scrollPosition,
                                             new Rect(0, 0, contentW, contentH));

        for (int i = 0; i < data.Length; i++)
        {
            float ry = i * rowH;
            byte val = data[i];

            GUI.Label(new Rect(0, ry, colW, rowH), i.ToString());
            GUI.Label(new Rect(colW, ry, colW, rowH), val.ToString("X2"));

            byte bit = 0x80;
            for (int k = 0; k < 8; k++)
            {
                bool on = (val & bit) == bit;
                GUI.Label(new Rect(colW * (2 + k), ry, colW, rowH), on ? "1" : "0");
                bit >>= 1;
            }
        }

        GUI.EndScrollView();
        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
}