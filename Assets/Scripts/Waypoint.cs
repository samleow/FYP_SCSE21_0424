using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Vector2Int gridPos;

    public Waypoint north, south, east, west;

    public Waypoint(Vector2Int gridPos, Waypoint n = null, Waypoint s = null, Waypoint e = null, Waypoint w = null)
    {
        if (gridPos != null)
            this.gridPos = gridPos;
        else
            this.gridPos = new Vector2Int();
        north = n;
        south = s;
        east = e;
        west = w;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (north)
            Gizmos.DrawLine(this.transform.position, north.transform.position);
        if (south)
            Gizmos.DrawLine(this.transform.position, south.transform.position);
        if (east)
            Gizmos.DrawLine(this.transform.position, east.transform.position);
        if (west)
            Gizmos.DrawLine(this.transform.position, west.transform.position);
    }

}
