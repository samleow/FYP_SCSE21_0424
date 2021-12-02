using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    public enum Direction
    {
        RIGHT,
        LEFT,
        UP,
        DOWN
    }
    protected Direction _dir = Direction.RIGHT;

    protected BoxCollider2D _collider;

    protected Waypoint _currWP = null;

    protected float _speed = 2;



    protected void Awake()
    {
        _collider = this.GetComponent<BoxCollider2D>();
    }

    protected void OnTriggerEnter2D(Collider2D col)
    {
        if (_currWP = col.GetComponent<Waypoint>())
        {
            //Debug.Log(this.name + " entered WP - " + _currWP.name);
        }
    }

    protected Vector2 GetDirectionVector()
    {
        return _dir switch
        {
            Direction.RIGHT => Vector2.right,
            Direction.LEFT => Vector2.left,
            Direction.UP => Vector2.up,
            Direction.DOWN => Vector2.down,
            _ => Vector2.zero,
        };
    }

    protected void SetDirectionByVector(Vector2 dir)
    {
        if (dir.x >= 1)
            _dir = Direction.RIGHT;
        else if (dir.x <= -1)
            _dir = Direction.LEFT;
        else if (dir.y >= 1)
            _dir = Direction.UP;
        else if (dir.y <= -1)
            _dir = Direction.DOWN;
    }

    protected bool MoveTo(Transform target)
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

}
