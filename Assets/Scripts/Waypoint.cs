using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Global;

public class Waypoint : MonoBehaviour
{
    public Vector2Int gridPos;

    //public Waypoint north, south, east, west;

    public Dictionary<Direction, Waypoint> branches = new Dictionary<Direction, Waypoint>();

    public Waypoint()
    {
        this.gridPos = new Vector2Int();
        if (branches == null)
            branches = new Dictionary<Direction, Waypoint>();
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        foreach (var branch in branches)
        {
            if (!branch.Value)
                continue;

            Gizmos.DrawLine(this.transform.position, branch.Value.transform.position);
        }

    }

    public Direction ?GetWaypointDirection(Waypoint wp)
    {
        if (branches.ContainsValue(wp))
        {
            foreach (var branch in branches)
            {
                if (branch.Value.Equals(wp))
                {
                    return branch.Key;
                }
            }
        }
        return null;
    }

}
