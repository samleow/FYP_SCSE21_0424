using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandler : Character
{
    private Waypoint _targetWP = null;
    private Waypoint _queuedWP = null;

    // temporary variable to test movement
    // will change to proper fsm structure in the future
    // e.g. PLAYER_CONTROL, AI_CONTROL(RUN), AI_CONTROL(CHASE), AI_CONTROL(COLLECT)
    public string state = "";

    void Start()
    {
        // set state initially to player control
        state = "PLAYER_CONTROL";
        _speed = 3;
    }

    void Update()
    {
        // temporary hardcoded state for testing
        if (state == "PLAYER_CONTROL")
        {
            if (!_targetWP)
            {
                if (!SetTargetWP(GetDirectionVector()))
                    return;
            }

            if(MoveTo(_targetWP.transform))
            {
                _targetWP = _queuedWP;
            }
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started)
            return;

        Vector2 dir = context.ReadValue<Vector2>();

        SetTargetWP(dir);
    }

    // can add queueing of next waypoint in here
    private bool SetTargetWP(Vector2 dir)
    {
        int layermask = ~(LayerMask.GetMask("Player"));
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position+dir, dir, Mathf.Infinity, layermask);

        if (hit.collider && hit.collider.CompareTag(SimulationManager.WAYPOINT_TAG))
        {
            SetDirectionByVector(dir);
            _targetWP = hit.transform.GetComponent<Waypoint>();
        }
        else
        {
            // add queueing here

            return false;
        }
        return true;
    }

}
