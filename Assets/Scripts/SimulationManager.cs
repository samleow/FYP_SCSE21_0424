
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

    private Vector3Int _fixedOrigin = new Vector3Int(-16, -17, 0);
    private Vector3Int _fixedSize = new Vector3Int(32, 33, 1);

    protected override void Awake()
    {
        base.Awake();

        _tileMatrix = new TileMatrix();
    }

    // Need reduce checks somehow
    // For now brute force check for junctions
    void PlotWaypoints()
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
                                // S in bounds
                                if (i - 1 >= 0)
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
                                // N in bounds
                                if (i + 1 < 33)
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
                                // W in bounds
                                if (j - 1 >= 0)
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
                                // E in bounds
                                if (j + 1 < 32)
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

    void CreateAWaypoint(int x, int y)
    {
        GameObject wp = new GameObject();
        wp.name = "waypoint";
        // hardcoded tag?
        wp.tag = "Waypoint";
        wp.transform.SetParent(waypointParent.transform);
        wp.transform.localScale = new Vector3(1, 1, 1);
        wp.transform.localPosition = new Vector3(_fixedOrigin.x + x + 0.5f, _fixedOrigin.y + y + 0.5f, 0);
        wp.AddComponent<BoxCollider2D>();
        wp.GetComponent<BoxCollider2D>().isTrigger = true;
    }

    void GenerateWaypoints()
    {
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

    // Testing on Erase button
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
    }
}
