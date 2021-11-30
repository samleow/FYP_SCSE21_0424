using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Character
{
    // should check with player's current WP instead of players actual position
    private GameObject _player;

    private float _speed = 2;

    private Waypoint _targetWP = null;

    // temporary variable to test pathfinding
    // will change to proper fsm structure in the future
    public string state = "";

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag(SimulationManager.PLAYER_TAG);
        state = "IDLE";
    }

    void Update()
    {
        // temporary hardcoded state for testing
        if (state == "MOVE")
        {
            if (_collider.IsTouching(_player.GetComponent<Collider2D>()))
            {
                state = "WIN";
                return;
            }

            if (!_targetWP)
                _targetWP = _currWP;

            if (MoveTo(_targetWP.transform))
            {
                SetTargetWP();
            }
        }
    }

    private bool MoveTo(Transform target)
    {
        float step = _speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        if (Vector3.Distance(transform.position, target.position) < 0.001f)
        {
            transform.position = target.position;
            return true;
        }
        return false;
    }

    private void SetTargetWP()
    {
        if (!_currWP)
            _targetWP = null;

        Vector2 dir = GetDirection(_player.transform);

        if (dir.x < 0)
        {
            if (_currWP.west)
                _targetWP = _currWP.west;
        }
        else if (dir.x > 0)
        {
            if (_currWP.east)
                _targetWP = _currWP.east;
        }

        if (dir.y < 0)
        {
            if (_currWP.south)
                _targetWP = _currWP.south;
        }
        else if (dir.y > 0)
        {
            if (_currWP.north)
                _targetWP = _currWP.north;
        }


        // testing wp linkage access
        //if (_targetWP = _currWP.south) { }
        //else if (_targetWP = _currWP.east) { }
        //else if (_targetWP = _currWP.west) { }
        //else if (_targetWP = _currWP.north) { }

    }

    private Vector2 GetDirection(Transform target)
    {
        return target.position - transform.position;
    }
}
