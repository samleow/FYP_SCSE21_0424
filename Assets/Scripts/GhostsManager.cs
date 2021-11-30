using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostsManager : Singleton<GhostsManager>
{

    // Ghosts
    public Ghost red;
    public Ghost orange;
    public Ghost pink;
    public Ghost cyan;



    protected override void Awake()
    {
        base.Awake();

    }

    void Update()
    {

    }

    // Button to move red for testing
    public void MoveRed()
    {
        if (red.state != "MOVE")
        {
            red.state = "MOVE";
        }
    }
}
