using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase : MonoBehaviour
{
    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}