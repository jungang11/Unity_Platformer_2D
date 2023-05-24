using BeeState;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

/* Bee
 * 1. �÷��̾ �ָ� ���� �� ������ �ֱ�
 * 2. �÷��̾ ��� ���� ���������, �÷��̾ �����ϵ��� ����
 */

public class Bee : MonoBehaviour
{
    // ���� ����
    public enum State { Idle, Trace, Return, Attack, Patrol, Size }

    [SerializeField] public float detectRange;
    [SerializeField] public float attackRange;
    [SerializeField] public float moveSpeed;
    [SerializeField] public Transform[] patrolPoints;

    // ���� ���� curState
    public StateBase[] states;
    public State curState;

    public Transform player;
    public Vector3 returnPosition;
    public int patrolIndex = 0;

    private Animator anim;

    private void Awake()
    {
        states = new StateBase[(int)State.Size];
        states[(int)State.Idle] = new IdleState(this);
        states[(int)State.Trace] = new TraceState(this);
        states[(int)State.Return] = new ReturnState(this);
        states[(int)State.Attack] = new AttackState(this);
        states[(int)State.Patrol] = new PatrolState(this);
    }

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

namespace BeeState
{
    public class IdleState : StateBase
    {
        private Bee bee;
        private float idleTime = 0;

        public IdleState(Bee bee)
        {
            this.bee = bee;
        }

        public override void Enter()
        {
            Debug.Log("Idle Enter");
        }

        public override void Update()
        {
            // ���� �ð��� �Ǿ��� ��
            if (idleTime > 2)
            {
                idleTime = 0;
                bee.ChangeState(Bee.State.Patrol);
            }
            idleTime += Time.deltaTime;

            // �÷��̾ ���������
            if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
            {
                // ���� ���¸� ���� ���·� ��ȯ
                bee.ChangeState(Bee.State.Trace);
            }
        }

        public override void Exit()
        {
            Debug.Log("Idle Exit");
        }
    }

    public class TraceState : StateBase
    {
        private Bee bee;

        public TraceState(Bee bee)
        {
            this.bee = bee;
        }

        public override void Enter()
        {
            Debug.Log("Trace Enter");
            
        }

        public override void Update()
        {
            // �÷��̾� �Ѿư���
            Vector2 dir = (bee.player.position - bee.transform.position).normalized;
            bee.transform.Translate(bee.moveSpeed * Time.deltaTime * dir);

            // �÷��̾ ���ݹ����κ��� �־����� ��
            if (Vector2.Distance(bee.player.position, bee.transform.position) > bee.detectRange)
            {
                bee.ChangeState(Bee.State.Return);
            }
            // ���ݹ����� ������ ��
            else if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.attackRange)
            {
                bee.ChangeState(Bee.State.Attack);
            }
        }

        public override void Exit()
        {
            Debug.Log("Trace Exit");
        }
    }

    public class ReturnState : StateBase
    {
        private Bee bee;

        public ReturnState(Bee bee)
        {
            this.bee = bee;
        }

        public override void Enter()
        {
            Debug.Log("Return Enter");
        }

        public override void Update()
        {
            // ���� �ڸ��� ���ư���
            Vector2 dir = (bee.returnPosition - bee.transform.position).normalized;
            bee.transform.Translate(dir * bee.moveSpeed * Time.deltaTime);

            // ���� �ڸ��� ����������( == �� �ƴ϶� �����ߴ� -> 0.02f ������ �Ÿ� )
            if (Vector2.Distance(bee.transform.position, bee.returnPosition) < 0.01f)
            {
                bee.ChangeState(Bee.State.Idle);
            }
            // ���ư��� �߿� ���ݹ����� ������ Attack
            else if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
            {
                bee.ChangeState(Bee.State.Trace);
            }
        }

        public override void Exit()
        {
            Debug.Log("Return Exit");
        }
    }

    public class AttackState : StateBase
    {
        private Bee bee;
        private float lastAttackTime = 0;

        public AttackState(Bee bee)
        {
            this.bee = bee;
        }

        public override void Enter()
        {
            Debug.Log("Attack Enter");
        }

        public override void Update()
        {
            // �����ϱ�
            if (lastAttackTime > 3)
            {
                Debug.Log("����");
            }
            lastAttackTime += Time.deltaTime;

            // �÷��̾ ���ݹ����κ��� �־����� ��
            if (Vector2.Distance(bee.player.position, bee.transform.position) > bee.attackRange)
            {
                bee.ChangeState(Bee.State.Trace);
            }
        }

        public override void Exit()
        {
            Debug.Log("Attack Exit");
        }
    }

    public class PatrolState : StateBase
    {
        private Bee bee;

        public PatrolState(Bee bee)
        {
            this.bee = bee;
        }

        public override void Enter()
        {
            Debug.Log("Patrol Enter");
            bee.patrolIndex = (bee.patrolIndex + 1) % bee.patrolPoints.Length;
        }

        public override void Update()
        {
            // ���� ����
            Vector2 dir = (bee.patrolPoints[bee.patrolIndex].position - bee.transform.position).normalized;
            bee.transform.Translate(bee.moveSpeed * Time.deltaTime * dir);

            if (Vector2.Distance(bee.transform.position, bee.patrolPoints[bee.patrolIndex].position) < 0.01f)
            {
                bee.ChangeState(Bee.State.Idle);
            }
            else if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
            {
                bee.ChangeState(Bee.State.Trace);
            }
        }

        public override void Exit()
        {
            Debug.Log("Patrol Exit");
        }
    }
}

