using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Mouse : MonoBehaviour
{
    // �ӵ�
    [SerializeField] private float moveSpeed;

    private float applySpeed;

    // ���� Ȯ��
    [SerializeField] Transform groundCheckPoint;
    [SerializeField] LayerMask groundMask;

    private Rigidbody2D rb;
    private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
    }

    private void FixedUpdate()
    {
        if (!isGroundExist())
        {
            Turn();
        }
    }

    private void Move()
    {
        applySpeed = moveSpeed;
        rb.velocity = new Vector2(transform.right.x * -applySpeed, rb.velocity.y);
    }

    private void Turn()
    {
        // y�� �������� 180�� ����
        transform.Rotate(Vector3.up, 180);
    }

    private bool isGroundExist()
    {
        Debug.DrawRay(groundCheckPoint.position, Vector2.down, Color.red);
        return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, 1f, groundMask);
    }
}
