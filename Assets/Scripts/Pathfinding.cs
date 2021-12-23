using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Global;

public class Pathfinding
{
    protected class Node
    {
        public Waypoint waypoint;
        public float f;
        public float g;
        public float h;

        public Node parent;

        public Node(Waypoint waypoint = null, Node parent = null, float f = 0.0f, float g = 0.0f, float h = 0.0f)
        {
            this.waypoint = waypoint;
            this.parent = parent;
            this.f = f;
            this.g = g;
            this.h = h;
        }
    }

    private Waypoint _currWP;
    private Waypoint _targetWP;

    private Stack<Direction> _steps;

    // DEBUGGING
    public Character player;
    public Character ghost;

    public Pathfinding(Waypoint currWP = null, Waypoint targetWP = null)
    {
        _currWP = currWP;
        _targetWP = targetWP;
        _steps = new Stack<Direction>();
    }

    public void SetCurrWP(Waypoint currWP)
    {
        _currWP = currWP;
    }

    public void SetTargetWP(Waypoint targetWP)
    {
        _targetWP = targetWP;
    }

    // returns distance between two points in the grid
    // for heuristic value
    private float ManhattanDistance(Waypoint start, Waypoint end)
    {
        return Mathf.Abs(start.gridPos.x - end.gridPos.x) + Mathf.Abs(start.gridPos.y - end.gridPos.y);
    }

    // A-Star Algorithm
    public Stack<Direction> GetSteps(Waypoint currentWP, Waypoint targetWP)
    {
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        openList.Add(new Node(currentWP));

        // temporary return value
        if (_steps == null) _steps = new Stack<Direction>();
        _steps.Clear();

        while (openList.Count > 0)
        {
            Node current = openList[0];
            foreach (Node node in openList)
            {
                if (node.f < current.f)
                    current = node;
            }

            if (current.waypoint.Equals(targetWP))
            {
                // construct path and return steps
                while (current.parent != null)
                {
                    _steps.Push((Direction)current.parent.waypoint.GetWaypointDirection(current.waypoint));
                    current = current.parent;
                }

                return _steps;
            }

            openList.Remove(current);
            closedList.Add(current);

            foreach (var child in current.waypoint.branches)
            {
                float g_score = current.g + ManhattanDistance(current.waypoint, child.Value);
                float h_score = ManhattanDistance(child.Value, targetWP);

                // check if child is alrdy in the closedList
                bool isIn = false;
                foreach (Node node in closedList)
                {
                    if (node.waypoint.Equals(child.Value))
                    {
                        // if current path has better f score, update values in closed list
                        if (g_score+h_score < node.f)
                        {
                            node.parent = current;
                            node.f = g_score + h_score;
                            node.g = g_score;
                            node.h = h_score;
                        }
                        isIn = true;
                        break;
                    }
                }

                // TODO: child.Value may be lost outside of scope, need double check !!
                if(!isIn)
                    openList.Add(new Node(child.Value, current, g_score+h_score, g_score, h_score));
            }

        }

        return _steps;
    }

    // overload
    public Stack<Direction> GetSteps()
    {
        return GetSteps(_currWP, _targetWP);
    }

    // DEBUGGING
    public void TestPathfinding()
    {

        _currWP = ghost._currWP;
        _targetWP = player._currWP;

        GetSteps();

        Debug.Log("Steps");

        while (_steps.Count > 0)
        {
            Debug.Log(" |\tDirection:\t" + _steps.Pop().ToString());
        }
    }


}
