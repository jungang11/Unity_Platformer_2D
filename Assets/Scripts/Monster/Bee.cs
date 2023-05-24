using BeeState;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

/* Bee
 * 1. 플레이어가 멀리 있을 때 가만히 있기
 * 2. 플레이어가 어느 정도 가까워지면, 플레이어를 공격하도록 추적
 */

public class Bee : MonoBehaviour
{
    // 상태 패턴
    public enum State { Idle, Trace, Return, Attack, Patrol, Size }

    [SerializeField] public float detectRange;
    [SerializeField] public float attackRange;
    [SerializeField] public float moveSpeed;
    [SerializeField] public Transform[] patrolPoints;

    // 현재 상태 curState
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

        // 초기 상태 Idle
        curState = State.Idle;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        returnPosition = transform.position;    // 복귀 지점은 시작 위치
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
            // 순찰 시간이 되었을 때
            if (idleTime > 2)
            {
                idleTime = 0;
                bee.ChangeState(Bee.State.Patrol);
            }
            idleTime += Time.deltaTime;

            // 플레이어가 가까워지면
            if (Vector2.Distance(bee.player.position, bee.transform.position) < bee.detectRange)
            {
                // 현재 상태를 추적 상태로 변환
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
            // 플레이어 쫓아가기
            Vector2 dir = (bee.player.position - bee.transform.position).normalized;
            bee.transform.Translate(bee.moveSpeed * Time.deltaTime * dir);

            // 플레이어가 공격범위로부터 멀어졌을 때
            if (Vector2.Distance(bee.player.position, bee.transform.position) > bee.detectRange)
            {
                bee.ChangeState(Bee.State.Return);
            }
            // 공격범위에 들어왔을 때
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
            // 원래 자리로 돌아가기
            Vector2 dir = (bee.returnPosition - bee.transform.position).normalized;
            bee.transform.Translate(dir * bee.moveSpeed * Time.deltaTime);

            // 원래 자리에 도착했으면( == 가 아니라 근접했다 -> 0.02f 이하의 거리 )
            if (Vector2.Distance(bee.transform.position, bee.returnPosition) < 0.01f)
            {
                bee.ChangeState(Bee.State.Idle);
            }
            // 돌아가는 중에 공격범위에 들어오면 Attack
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
            // 공격하기
            if (lastAttackTime > 3)
            {
                Debug.Log("공격");
            }
            lastAttackTime += Time.deltaTime;

            // 플레이어가 공격범위로부터 멀어졌을 때
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
            // 순찰 진행
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

