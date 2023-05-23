using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    [SerializeField] LayerMask groundMask;

    private Rigidbody2D rb;
    private Animator anim;
    public bool isGround;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Fly();
    }

    private void FixedUpdate()
    {
        GroundCheck();
    }

    private void Fly()
    {
        if(isGround)
            rb.AddForce(Vector2.up * moveSpeed, ForceMode2D.Impulse);
    }

    // 지면 체크
    private void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, groundMask);

        if (hit.collider != null) // 충돌
        {
            isGround = true;
            Debug.DrawRay(transform.position, new Vector3(hit.point.x, hit.point.y, 0) - transform.position, Color.red, 2f);
        }
        else
        {
            isGround = false;
            Debug.DrawRay(transform.position, Vector2.down * 2f, Color.green);
        }
    }
}
