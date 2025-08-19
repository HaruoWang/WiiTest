using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using WiimoteApi;

public class Engrave : MonoBehaviour
{
    public WiimoteModel model;
    private Wiimote wiimote;

    public GameObject voxelPrefab;
    private GameObject[,,] voxelGrid;
    public int gridSize = 10;
    public float voxelSpacing = 1f;

    void Start()
    {
        voxelGrid = new GameObject[gridSize, gridSize, gridSize];
        for (int x = 0; x < gridSize; x++)
            for (int y = 0; y < gridSize; y++)
                for (int z = 0; z < gridSize; z++)
                {
                    Vector3 pos = new Vector3(x, y, z) * voxelSpacing;
                    voxelGrid[x, y, z] = GameObject.Find("Cube");
                }
    }

    void Update()
    {
        if (wiimote == null && WiimoteManager.Wiimotes.Count > 0)
        {
            wiimote = WiimoteManager.Wiimotes[0];
        }

        if (wiimote != null)
        {
            int ret = wiimote.ReadWiimoteData();

            if (wiimote.Button.a)
            {
                CarveVoxel(gridSize / 2, gridSize / 2, gridSize / 2);
            }

            if (wiimote.Button.b)
            {
                CarveVoxel(gridSize / 2 + 1, gridSize / 2, gridSize / 2);
            }
        }
    }

    void CarveVoxel(int x, int y, int z)
    {
        if (x >= 0 && x < gridSize &&
            y >= 0 && y < gridSize &&
            z >= 0 && z < gridSize)
        {
            if (voxelGrid[x, y, z] != null)
            {
                Destroy(voxelGrid[x, y, z]);
                voxelGrid[x, y, z] = null;
            }
        }
    }

    void UpdateButtons()
    {
        model.a.enabled = wiimote.Button.a;
        //model.b.enabled = wiimote.Button.b;
        //model.one.enabled = wiimote.Button.one;
        //model.two.enabled = wiimote.Button.two;
        //model.d_up.enabled = wiimote.Button.d_up;
        //model.d_down.enabled = wiimote.Button.d_down;
        //model.d_left.enabled = wiimote.Button.d_left;
        //model.d_right.enabled = wiimote.Button.d_right;
        //model.plus.enabled = wiimote.Button.plus;
        //model.minus.enabled = wiimote.Button.minus;
        //model.home.enabled = wiimote.Button.home;
    }

    void OnApplicationQuit()
    {
        if (wiimote != null)
        {
            WiimoteManager.Cleanup(wiimote);
            wiimote = null;
        }
    }

    void OnGUI()
    {
        GUILayout.BeginVertical("box", GUILayout.Width(300));

        GUILayout.Label("Wiimote Found: " + WiimoteManager.HasWiimote());
        GUILayout.Label("Count: " + WiimoteManager.Wiimotes.Count);

        if (GUILayout.Button("Find Wiimote"))
        {
            WiimoteManager.FindWiimotes();
            if (WiimoteManager.HasWiimote())
                wiimote = WiimoteManager.Wiimotes[0];
        }

        if (GUILayout.Button("Cleanup"))
        {
            if (wiimote != null)
            {
                WiimoteManager.Cleanup(wiimote);
                wiimote = null;
            }
        }

        GUILayout.EndVertical();
    }

    [System.Serializable]
    public class WiimoteModel
    {
        public Transform rot;
        public Renderer a;
        //public Renderer b;
        //public Renderer one;
        //public Renderer two;
        //public Renderer d_up;
        //public Renderer d_down;
        //public Renderer d_left;
        //public Renderer d_right;
        //public Renderer plus;
        //public Renderer minus;
        //public Renderer home;
    }
}
