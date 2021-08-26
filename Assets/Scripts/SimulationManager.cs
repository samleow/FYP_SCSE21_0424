
using UnityEngine;
using UnityEngine.Tilemaps;

public class SimulationManager : Singleton<SimulationManager>
{
    public class TileMatrix
    {
        public int[,] walls, points, waypoints;
    }

    private TileMatrix _tileMatrix;

    public Tilemap tm;
    public GameObject waypointParent;

    public const string WAYPOINT_TAG = "Waypoint";
    public const string WALL_TAG = "Walls";

    private Vector3Int _fixedOrigin = new Vector3Int(-16, -17, 0);
    private Vector3Int _fixedSize = new Vector3Int(32, 33, 1);

    private bool _waypointsGenerated = false;

    protected override void Awake()
    {
        base.Awake();

        _tileMatrix = new TileMatrix();
        _waypointsGenerated = false;
    }

    // Need reduce checks somehow
    // For now brute force check for junctions
    // Plot waypoints onto a grid by checking for junctions
    private void PlotWaypoints()
    {
        _tileMatrix.waypoints = new int[33, 32];
        for (int i = 0; i < 33; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                _tileMatrix.waypoints[i, j] = 1;

                // if tile is not wall
                if (_tileMatrix.walls[i, j] == 0)
                {
                    // check for junctions

                    // N clear
                    if (i + 1 < 33 && _tileMatrix.walls[i + 1, j] == 0)
                    {
                        // E wall
                        if (j + 1 < 32 && _tileMatrix.walls[i, j + 1] == 1)
                        {
                            // W wall
                            if (j - 1 < 32 && _tileMatrix.walls[i, j - 1] == 1)
                            {
                                // S clear
                                if (i - 1 >= 0 && _tileMatrix.walls[i - 1, j] == 0)
                                {
                                    _tileMatrix.waypoints[i, j] = 0;
                                }
                            }
                        }
                    }

                    // S clear
                    else if (i - 1 >= 0 && _tileMatrix.walls[i - 1, j] == 0)
                    {
                        // E wall
                        if (j + 1 < 32 && _tileMatrix.walls[i, j + 1] == 1)
                        {
                            // W wall
                            if (j - 1 < 32 && _tileMatrix.walls[i, j - 1] == 1)
                            {
                                // N clear
                                if (i + 1 < 33 && _tileMatrix.walls[i + 1, j] == 0)
                                {
                                    _tileMatrix.waypoints[i, j] = 0;
                                }
                            }
                        }
                    }

                    // E clear
                    else if (j + 1 < 32 && _tileMatrix.walls[i, j + 1] == 0)
                    {
                        // N wall
                        if (i + 1 < 33 && _tileMatrix.walls[i + 1, j] == 1)
                        {
                            // S wall
                            if (i - 1 >= 0 && _tileMatrix.walls[i - 1, j] == 1)
                            {
                                // W clear
                                if (j - 1 >= 0 && _tileMatrix.walls[i, j - 1] == 0)
                                {
                                    _tileMatrix.waypoints[i, j] = 0;
                                }
                            }
                        }
                    }

                    // W clear
                    else if (j - 1 >= 0 && _tileMatrix.walls[i, j - 1] == 0)
                    {
                        // N wall
                        if (i + 1 < 33 && _tileMatrix.walls[i + 1, j] == 1)
                        {
                            // S wall
                            if (i - 1 >= 0 && _tileMatrix.walls[i - 1, j] == 1)
                            {
                                // E clear
                                if (j + 1 < 32 && _tileMatrix.walls[i, j + 1] == 0)
                                {
                                    _tileMatrix.waypoints[i, j] = 0;
                                }
                            }
                        }
                    }
                }
                else
                {
                    _tileMatrix.waypoints[i, j] = 0;
                }
            }
        }
    }

    // Creates a waypoint gameobject with (x, y) grid coordinates
    private void CreateAWaypoint(int x, int y)
    {
        GameObject wp = new GameObject();
        wp.name = "waypoint " + x + " " + y;
        wp.tag = WAYPOINT_TAG;
        wp.transform.SetParent(waypointParent.transform);
        wp.transform.localScale = new Vector3(1, 1, 1);
        wp.transform.localPosition = new Vector3(_fixedOrigin.x + x + 0.5f, _fixedOrigin.y + y + 0.5f, 0);
        wp.AddComponent<BoxCollider2D>();
        wp.GetComponent<BoxCollider2D>().isTrigger = true;
        Waypoint wpComp = wp.AddComponent<Waypoint>();
        wpComp.gridPos.Set(x,y);
    }

    // Generates all waypoint gameobjects from the grid
    private void GenerateWaypoints()
    {
        if (_waypointsGenerated)
            return;
        else
            _waypointsGenerated = true;

        for (int i = 0; i < 33; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                if(_tileMatrix.waypoints[i, j] == 1)
                {
                    CreateAWaypoint(j,i);
                }
            }
        }
    }

    // Link the waypoint gameobjects
    // May have heavy processing due to multiple GetComponent calls in a loop
    // Other colliders may affect waypoint linkings
    // Eg. Player, ghosts, points pickup colliders
    private void LinkWaypoints()
    {
        foreach (Transform wp_tf in waypointParent.transform)
        {
            RaycastHit2D[] result = new RaycastHit2D[1];
            BoxCollider2D collider = wp_tf.GetComponent<BoxCollider2D>();
            Waypoint wp_comp = wp_tf.GetComponent<Waypoint>();
            // north/up
            if (collider.Raycast(Vector2.up, result) > 0)
            {
                if (result[0].collider.tag.Equals(WAYPOINT_TAG))
                {
                    wp_comp.north = result[0].transform.GetComponent<Waypoint>();
                }
            }
            // south/down
            if (collider.Raycast(Vector2.down, result) > 0)
            {
                if (result[0].collider.tag.Equals(WAYPOINT_TAG))
                {
                    wp_comp.south = result[0].transform.GetComponent<Waypoint>();
                }
            }
            // east/right
            if (collider.Raycast(Vector2.right, result) > 0)
            {
                if (result[0].collider.tag.Equals(WAYPOINT_TAG))
                {
                    wp_comp.east = result[0].transform.GetComponent<Waypoint>();
                }
            }
            // west/left
            if (collider.Raycast(Vector2.left, result) > 0)
            {
                if (result[0].collider.tag.Equals(WAYPOINT_TAG))
                {
                    wp_comp.west = result[0].transform.GetComponent<Waypoint>();
                }
            }

        }
    }

    // Prints out grid
    void DebugPrintGrid(int [,] grid)
    {
        string s;
        for (int i = 32; i >= 0; i--)
        {
            s = "";
            for (int j = 0; j < 32; j++)
            {
                s += grid[i, j] + " ";
            }

            Debug.Log(s);
        }
    }

    // Testing on button click
    public void Debugger()
    {

        //tm.CompressBounds();
        tm.origin = _fixedOrigin;
        tm.size = _fixedSize;
        tm.ResizeBounds();


        TileBase[] tb = tm.GetTilesBlock(tm.cellBounds);
        int[] a = new int[tb.Length];

        for (int i=0; i<tb.Length; i++)
        {
            a[i] = tb[i] == null ? 0 : 1;

        }
        
        // grid is known, 32x33 (XY)
        // grid starts from bottom left corner
        _tileMatrix.walls = new int[33, 32];
        TileBase[,] tb2d = new TileBase[33,32];
        int k = 0;
        for (int i=0; i<33; i++)
        {
            for (int j = 0; j < 32; j++)
            {
                tb2d[i, j] = tb[k];
                _tileMatrix.walls[i, j] = a[k];
                k += 1;
            }
        }

        PlotWaypoints();

        //DebugPrintGrid(_tileMatrix.waypoints);

        GenerateWaypoints();

        LinkWaypoints();
    }
}
