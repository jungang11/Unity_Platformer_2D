using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Bee
 * 1. �÷��̾ �ָ� ���� �� ������ �ֱ�
 * 2. �÷��̾ ��� ���� ���������, �÷��̾ �����ϵ��� ����
 */

public class Bee : MonoBehaviour
{
    // ���� ����
    public enum State { Idle, Trace, Return, Attack, Patrol }

    [SerializeField] private float detectRange;
    [SerializeField] private float attackRange;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform[] patrolPoints;

    // ���� ���� curState
    [SerializeField]
    private State curState;

    private Transform player;
    private Vector3 returnPosition;
    private int patrolIndex = 0;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();

        // �ʱ� ���� Idle 
        curState = State.Idle;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        returnPosition = transform.position;    // ���� ������ ���� ��ġ
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
    // ������ ���� ��
    private void IdleUpdate()
    {
        // �ƹ��͵� ���ϱ�
        
        // ���� �ð��� �Ǿ��� ��
        if (idleTime > 2)
        {
            idleTime = 0;
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            curState = State.Patrol;
        }
        idleTime += Time.deltaTime;

        // �÷��̾ ���������
        if(Vector2.Distance(player.position, transform.position) < detectRange)
        {
            // ���� ���¸� ���� ���·� ��ȯ
            curState = State.Trace;
        }
    }

    // ����
    private void TraceUpdate()
    {
        // �÷��̾� �Ѿư���
        Vector2 dir = (player.position - transform.position).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime);

        // �÷��̾ ���ݹ����κ��� �־����� ��
        if(Vector2.Distance(player.position, transform.position) > attackRange)
        {
            curState = State.Return;
        }
        // ���ݹ����� ������ ��
        else if(Vector2.Distance(player.position, transform.position) < attackRange)
        {
            curState = State.Attack;
        }
    }

    // �÷��̾ ���� ���ư�
    private void ReturnUpdate()
    {
        // ���� �ڸ��� ���ư���
        Vector2 dir = (returnPosition - transform.position).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime);

        // ���� �ڸ��� ����������( == �� �ƴ϶� �����ߴ� -> 0.02f ������ �Ÿ� )
        if (Vector2.Distance(transform.position, returnPosition) < 0.01f )
        {
            curState = State.Idle;
        }
        // ���ư��� �߿� ���ݹ����� ������ Attack
        else if (Vector2.Distance(player.position, transform.position) < detectRange)
        {
            curState = State.Trace;
        }
    }

    // ���� ��Ÿ��
    float lastAttackTime = 0;
    // ��������� ����
    private void AttackUpdate()
    {
        // �����ϱ�
        if(lastAttackTime > 3)
        {
            Debug.Log("����");
        }
        lastAttackTime += Time.deltaTime;

        // �÷��̾ ���ݹ����κ��� �־����� ��
        if (Vector2.Distance(player.position, transform.position) > attackRange)
        {
            curState = State.Trace;
        }
    }

    private void PatrolUpdate()
    {
        // ���� ����
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
