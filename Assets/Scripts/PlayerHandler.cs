using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandler : Character
{
    private Waypoint _targetWP = null;
    private Waypoint _queuedWP = null;

    private SpriteRenderer _spriteRenderer = null;

    // temporary variable to test movement
    // will change to proper fsm structure in the future
    // e.g. PLAYER_CONTROL, AI_CONTROL(RUN), AI_CONTROL(CHASE), AI_CONTROL(COLLECT)
    public string state = "";

    void Start()
    {
        _spriteRenderer = transform.GetComponent<SpriteRenderer>();

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
                if (_queuedWP)
                {
                    _dir = (Direction)_targetWP.GetWaypointDirection(_queuedWP);
                    TurnPlayer(GetDirectionVector());
                }
                _targetWP = _queuedWP;
                _queuedWP = null;
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
        int layermask = ~(LayerMask.GetMask("Player", "Ghost"));
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position+dir, dir, Mathf.Infinity, layermask);

        if (hit.collider && hit.collider.CompareTag(SimulationManager.WAYPOINT_TAG))
        {
            SetDirectionByVector(dir);
            TurnPlayer(dir);
            _targetWP = hit.transform.GetComponent<Waypoint>();
        }
        else
        {
            if (!_targetWP)
                return false;

            // add queueing here
            switch (GetDirection(dir))
            {
                case Direction.RIGHT:
                    _queuedWP = _targetWP.east ? _targetWP.east : null;
                    break;
                case Direction.LEFT:
                    _queuedWP = _targetWP.west ? _targetWP.west : null;
                    break;
                case Direction.UP:
                    _queuedWP = _targetWP.north ? _targetWP.north : null;
                    break;
                case Direction.DOWN:
                    _queuedWP = _targetWP.south ? _targetWP.south : null;
                    break;
                default:
                    break;
            }
            return false;
        }
        return true;
    }

    private void TurnPlayer(Vector2 dir)
    {
        if (dir.x >= 1)
        {
            _spriteRenderer.flipX = false;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (dir.x <= -1)
        {
            _spriteRenderer.flipX = true;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (dir.y >= 1)
        {
            _spriteRenderer.flipX = false;
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (dir.y <= -1)
        {
            _spriteRenderer.flipX = false;
            transform.localRotation = Quaternion.Euler(0, 0, -90);
        }
    }

}
