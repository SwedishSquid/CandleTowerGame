using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    private float WalkSpeedBlocksPerSecond = 10;

    private float FallSpeedBlocksPerSecond = 20;

    private float JumpSpeedBlocksPerSecond = 20;

    private float JumpHeightBlocks = 6;

    private float DashSpeedBlocksPerSecond = 50;
    private float DashDistanceBlocks = 6;

    private float GroundCheckHeight = 0.1f;

    private Vector2 desiredMoveDirection;

    private Vector2 lastNonzeroDesiredMoveDirection = Vector2.right;

    private Vector2 dashDirection = Vector2.right;

    private Vector2 lastPlayerPosition = Vector2.zero;
    private int theSameCount = 0;

    private Rigidbody2D rb;

    [SerializeField]
    public Transform LeftFoot;
    [SerializeField]
    public Transform RightFoot;

    private int GroundLayerMask;

    private PlayerState state;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GroundLayerMask = LayerMask.GetMask("Ground");
        state = PlayerState.Regular;
    }

    private void Start()
    {

    }

    private void Update()
    {
        rb.velocity = Vector2.zero;
        if (state == PlayerState.Regular)
        {
            PerformWalk();
            PerformFall();
        }
        else if(state == PlayerState.Dashing)
        {
            PerformDash();
        }
        else if (state == PlayerState.Jumping)
        {
            PerformJump();
            PerformWalk();
            StopJumpFromCollision();
        }
        else if (state == PlayerState.NotFalling)
        {
            PerformWalk();
        }
        lastPlayerPosition = transform.position;
    }

    private void PerformWalk()
    {
        rb.velocity += (new Vector2(desiredMoveDirection[0], 0).normalized) * WalkSpeedBlocksPerSecond;
    }

    private void PerformFall()
    {
        rb.velocity -= new Vector2(0, 1) * FallSpeedBlocksPerSecond;
    }

    private void PerformJump()
    {
        rb.velocity += new Vector2(0, 1) * JumpSpeedBlocksPerSecond;
    }

    private void PerformDash()
    {
        rb.velocity += dashDirection * DashSpeedBlocksPerSecond;
    }

    private bool IsGrounded()
    {
        var p1 = (Vector2)LeftFoot.position;
        var p2 = (Vector2)RightFoot.position - new Vector2(0, GroundCheckHeight);

        //additional check of intersected area may be helpful
        return Physics2D.OverlapArea(p1, p2, GroundLayerMask);
    }

    public void OnJumpChanged(bool start)
    {
        Debug.Log("jump you dog!");
        if (start && state == PlayerState.Regular && IsGrounded())
        {
            state = PlayerState.Jumping;
            StartCoroutine(StopJumpFromTimeout());
        }
    }

    private IEnumerator StopJumpFromTimeout()
    {
        yield return new WaitForSeconds(JumpHeightBlocks / JumpSpeedBlocksPerSecond);
        if (state == PlayerState.Jumping)
        {
            state = PlayerState.NotFalling;
            StartCoroutine(ResumeFallingFromTimeout());
        }
    }

    private void StopJumpFromCollision()
    {
        
        if (state == PlayerState.Jumping && Mathf.Abs(lastPlayerPosition.y - transform.position.y) < 1e-2)
        {
            Debug.Log(theSameCount);
            //use time instead
            theSameCount++;
            if (theSameCount > 10)
            {
                state = PlayerState.Regular;
                theSameCount = 0;
            }
        }
        else
        {
            theSameCount = 0;
        }
    }

    public void OnMoveChanged(Vector2 direction)
    {
        desiredMoveDirection = direction.normalized;
        if (desiredMoveDirection.sqrMagnitude > 0)
        {
            lastNonzeroDesiredMoveDirection = desiredMoveDirection;
        }
    }

    public void OnDashChanged()
    {
        Debug.Log("Dash it");
        dashDirection = lastNonzeroDesiredMoveDirection;
        if (state == PlayerState.Regular || state == PlayerState.Jumping || state == PlayerState.NotFalling)
        {
            state = PlayerState.Dashing;
            StartCoroutine(StopDashFromTimeout());
        }
    }

    public IEnumerator StopDashFromTimeout()
    {
        yield return new WaitForSeconds(DashDistanceBlocks / DashSpeedBlocksPerSecond);
        if (state == PlayerState.Dashing)
        {
            state = PlayerState.NotFalling;
            StartCoroutine(ResumeFallingFromTimeout());
        }
    }

    private IEnumerator ResumeFallingFromTimeout()
    {
        yield return new WaitForSeconds(0.1f);
        if (state == PlayerState.NotFalling)
        {
            state = PlayerState.Regular;
        }
    }
}



public enum PlayerState
{
    Regular,
    Dashing,
    Jumping,
    NotFalling,
}
