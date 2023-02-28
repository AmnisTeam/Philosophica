using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameState : State
{
    public GameState()
    {
        startEvents += Start;
        updateEvents += Update;
    }

    public abstract void Start();
    public abstract void Update();
}
