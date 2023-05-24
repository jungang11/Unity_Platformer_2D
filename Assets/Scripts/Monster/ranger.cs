using BeeState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ranger : MonoBehaviour
{
    public enum State { Idle, Trace, Return, Attack, Runaway, Size }

    [SerializeField] public float detectRange;
    [SerializeField] public float attackRange;

    // 현재 상태 curState
    public StateBase[] states;
    public State curState;
    public Transform player;

    private void Awake()
    {
        states = new StateBase[(int)State.Size];
        states[(int)State.Idle] = new IdleState();
        states[(int)State.Trace] = new TraceState();
        states[(int)State.Return] = new ReturnState();
        states[(int)State.Attack] = new AttackState();
        states[(int)State.Runaway] = new RunawayState();
    }

    private void Start()
    {
        // 초기 상태 Idle
        curState = State.Idle;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        states[(int)curState].Update();
    }

    public void ChangeState(State state)
    {
        curState = state;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

namespace RangerState
{
    public class IdleState : StateBase
    {

    }
}

