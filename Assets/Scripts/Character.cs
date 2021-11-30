using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected BoxCollider2D _collider;

    protected Waypoint _currWP = null;

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

}
