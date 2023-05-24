using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Bee
 * 1. 플레이어가 멀리 있을 때 가만히 있기
 * 2. 플레이어가 어느 정도 가까워지면, 플레이어를 공격하도록 추적
 */

public class Bee : MonoBehaviour
{
    // 상태 패턴
    public enum State { Idle, Trace, Return, Attack, Patrol }

    [SerializeField] private float detectRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform[] patrolPoints;

    // 현재 상태 curState
    [SerializeField]
    private State curState;

    private Transform player;
    private Vector3 returnPosition;
    private int patrolIndex = 0;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();

        // 초기 상태 Idle 
        curState = State.Idle;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        returnPosition = transform.position;    // 복귀 지점은 시작 위치
    }

    private void Update()
    {
        switch (curState)
        {
            case State.Idle:
                IdleUpdate();
                break;
            case State.Trace:
                TraceUpdate();
                break;
            case State.Return:
                ReturnUpdate();
                break;
            case State.Attack:
                AttackUpdate();
                break;
            case State.Patrol:
                PatrolUpdate();
                break;
        }
    }

    private float idleTime = 0;
    // 가만히 있을 때
    private void IdleUpdate()
    {
        // 아무것도 안하기
        
        // 순찰 시간이 되었을 때
        if (idleTime > 2)
        {
            idleTime = 0;
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            curState = State.Patrol;
        }
        idleTime += Time.deltaTime;

        // 플레이어가 가까워지면
        if(Vector2.Distance(player.position, transform.position) < detectRange)
        {
            // 현재 상태를 추적 상태로 변환
            curState = State.Trace;
        }
    }

    // 추적
    private void TraceUpdate()
    {
        // 플레이어 쫓아가기
        Vector2 dir = (player.position - transform.position).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime);

        // 플레이어가 공격범위로부터 멀어졌을 때
        if(Vector2.Distance(player.position, transform.position) > attackRange)
        {
            curState = State.Return;
        }
        // 공격범위에 들어왔을 때
        else if(Vector2.Distance(player.position, transform.position) < attackRange)
        {
            curState = State.Attack;
        }
    }

    // 플레이어를 놓쳐 돌아감
    private void ReturnUpdate()
    {
        // 원래 자리로 돌아가기
        Vector2 dir = (returnPosition - transform.position).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime);

        // 원래 자리에 도착했으면( == 가 아니라 근접했다 -> 0.02f 이하의 거리 )
        if (Vector2.Distance(transform.position, returnPosition) < 0.01f )
        {
            curState = State.Idle;
        }
        // 돌아가는 중에 공격범위에 들어오면 Attack
        else if (Vector2.Distance(player.position, transform.position) < detectRange)
        {
            curState = State.Trace;
        }
    }

    // 공격 쿨타임
    float lastAttackTime = 0;
    // 가까워지면 공격
    private void AttackUpdate()
    {
        // 공격하기
        if(lastAttackTime > 3)
        {
            Debug.Log("공격");
        }
        lastAttackTime += Time.deltaTime;

        // 플레이어가 공격범위로부터 멀어졌을 때
        if (Vector2.Distance(player.position, transform.position) > attackRange)
        {
            curState = State.Trace;
        }
    }

    private void PatrolUpdate()
    {
        // 순찰 진행
        Vector2 dir = (patrolPoints[patrolIndex].position - transform.position).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, patrolPoints[patrolIndex].position) < 0.01f)
        {
            curState = State.Idle;
        }
        else if (Vector2.Distance(player.position, transform.position) < detectRange)
        {
            curState = State.Trace;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
