using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Global;

public class Ghost : Character
{
    // should check with player's current WP instead of players actual position
    private GameObject _player;
    private PlayerHandler _playerH = null;

    private Waypoint _targetWP = null;

    private Pathfinding _pathfinding;

    // temporary variable to test pathfinding
    // will change to proper fsm structure in the future
    public string state = "";

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag(SimulationManager.PLAYER_TAG);
        _playerH = _player.GetComponent<PlayerHandler>();
        _pathfinding = new Pathfinding();
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
                Debug.Log("Ghost " + this.name + " win!");
                return;
            }

            // if no current waypoint set
            // e.g. ghost spawned in between 2 waypoints
            if (!_currWP)
            {
                int layermask = ~(LayerMask.GetMask("Ghost", "Player"));
                RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(GetTargetDirection(_player.transform).x, 0), Mathf.Infinity, layermask);
                if (hit.collider && hit.collider.CompareTag(SimulationManager.WAYPOINT_TAG))
                    _targetWP = hit.transform.GetComponent<Waypoint>();
                else
                {
                    hit = Physics2D.Raycast(transform.position, new Vector2(0,GetTargetDirection(_player.transform).y), Mathf.Infinity, layermask);
                    if (hit.collider && hit.collider.CompareTag(SimulationManager.WAYPOINT_TAG))
                        _targetWP = hit.transform.GetComponent<Waypoint>();
                }
            }

            // if no initial target waypoint, move towards current waypoint
            if (!_targetWP)
                _targetWP = _currWP;

            if (MoveTo(_targetWP.transform))
                SetTargetWP();
        }
    }

    private void SetTargetWP()
    {
        if (!_currWP)
        {
            _targetWP = null;
            return;
        }

        _pathfinding.SetCurrWP(_currWP);
        if (_playerH._currWP == null)
        {
            // target wp right in front of player instead
            _pathfinding.SetTargetWP(_playerH.GetTargetWP());
        }
        else
            _pathfinding.SetTargetWP(_playerH._currWP);

        Stack<Direction> _steps = _pathfinding.GetSteps();

        if (_steps != null)
        {
            // target wp right in front of player instead
            if (_steps.Count == 0)
                _targetWP = _playerH.GetTargetWP();
            else
                _targetWP = _currWP.branches[_steps.Pop()];
        }
    }

    // For prototype test
    // Will be replaced with A* pathfinding
    private void OldSetTargetWP()
    {
        if (!_currWP)
        {
            _targetWP = null;
            return;
        }

        Vector2 dir = GetTargetDirection(_player.transform);

        if (dir.x < 0)
        {
            if(_currWP.branches.ContainsKey(Direction.LEFT))
                _targetWP = _currWP.branches[Direction.LEFT];
        }
        else if (dir.x > 0)
        {
            if (_currWP.branches.ContainsKey(Direction.RIGHT))
                _targetWP = _currWP.branches[Direction.RIGHT];
        }

        if (dir.y < 0)
        {
            if (_currWP.branches.ContainsKey(Direction.DOWN))
                _targetWP = _currWP.branches[Direction.DOWN];
        }
        else if (dir.y > 0)
        {
            if (_currWP.branches.ContainsKey(Direction.UP))
                _targetWP = _currWP.branches[Direction.UP];
        }
    }

    private Vector2 GetTargetDirection(Transform target)
    {
        return target.position - transform.position;
    }

    // debug render the ghost pathfinding path
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (_pathfinding == null || _pathfinding.path == null)
            return;
        Pathfinding.Node path = _pathfinding.path;

        while (path.parent != null)
        {
            Gizmos.DrawLine(path.parent.waypoint.transform.position, path.waypoint.transform.position);
            path = path.parent;
        }

        Gizmos.DrawLine(this.transform.position, path.waypoint.transform.position);
    }
}
