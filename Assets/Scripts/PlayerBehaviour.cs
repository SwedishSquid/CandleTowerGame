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

    private bool canDash = true;

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
        if (state != PlayerState.Dashing && IsGrounded())
        {
            canDash = true;
        }

        rb.velocity = Vector2.zero;
        if (state == PlayerState.Regular)
        {
            PerformMove();
            PerformFall();
        }
        else if(state == PlayerState.Dashing)
        {
            PerformDash();
        }
        else if (state == PlayerState.Jumping)
        {
            PerformJump();
            PerformMove();
            StopJumpFromCollision();
        }
        else if (state == PlayerState.NotFalling)
        {
            PerformMove();
        }
    }

    private void PerformFall()
    {
        rb.velocity -= new Vector2(0, 1) * FallSpeedBlocksPerSecond;
    }

    private bool IsGrounded()
    {
        var p1 = (Vector2)LeftFoot.position;
        var p2 = (Vector2)RightFoot.position - new Vector2(0, GroundCheckHeight);

        //additional check of intersected area may be helpful
        return Physics2D.OverlapArea(p1, p2, GroundLayerMask);
    }

    private void SetState(PlayerState nextState)
    {
        if (state == PlayerState.Jumping)
        {
            OnJumpTermination();
        }
        else if (state == PlayerState.Dashing)
        {
            OnDashTermination();
        }
        else if (state == PlayerState.NotFalling)
        {
            OnNotFallingTermination();
        }


        if (nextState == PlayerState.Jumping)
        {
            OnJumpSetup();
        }
        else if (nextState == PlayerState.Dashing)
        {
            OnDashSetup();
        }
        else if (nextState == PlayerState.NotFalling)
        {
            OnNotFallingSetup();
        }

        state = nextState;
    }

    #region Jump

    private float beforeJumpHeight;

    private float lastAbsoluteHeight;

    private float timeSinceHeightNotChanging;

    private void OnJumpSetup()
    {
        beforeJumpHeight = transform.position.y;
        timeSinceHeightNotChanging = 0;
        StartCoroutine(StopJumpFromTimeout());
    }

    private void OnJumpTermination()
    {
        //do what needs to be done when jump is terminated
    }

    //public event
    public void OnJumpChanged(bool start)
    {
        Debug.Log("jump you dog!");
        if (start && state == PlayerState.Regular && IsGrounded())
        {
            SetState(PlayerState.Jumping);
        }
    }

    private void PerformJump()
    {
        var deltaHeight = Mathf.Abs(beforeJumpHeight - transform.position.y);
        if (deltaHeight > JumpHeightBlocks - 1e-3)
        {
            return;
        }

        rb.velocity += new Vector2(0, 1) * JumpSpeedBlocksPerSecond;
    }

    private IEnumerator StopJumpFromTimeout()
    {
        yield return new WaitForSeconds(JumpHeightBlocks / JumpSpeedBlocksPerSecond);
        if (state == PlayerState.Jumping)
        {
            SetState(PlayerState.NotFalling);
            StartCoroutine(ResumeFallingFromTimeout());
        }
    }

    private void StopJumpFromCollision()
    {
        
        if (state == PlayerState.Jumping && Mathf.Abs(lastAbsoluteHeight - transform.position.y) < 1e-2)
        {
            Debug.Log(timeSinceHeightNotChanging);
            timeSinceHeightNotChanging += Time.deltaTime;
            if (timeSinceHeightNotChanging > 0.1f)
            {
                //not a very good move though
                SetState(PlayerState.Regular);
            }
        }
        else
        {
            timeSinceHeightNotChanging = 0;
        }
        lastAbsoluteHeight = transform.position.y;
    }

    #endregion

    #region Move

    public void OnMoveChanged(Vector2 direction)
    {
        desiredMoveDirection = direction.normalized;
        if (desiredMoveDirection.sqrMagnitude > 0)
        {
            lastNonzeroDesiredMoveDirection = desiredMoveDirection;
        }
    }

    public void PerformMove()
    {
        rb.velocity += (new Vector2(desiredMoveDirection[0], 0).normalized) * WalkSpeedBlocksPerSecond;
    }

    #endregion

    #region Dash

    private Vector2 dashDirection = Vector2.right;

    private Vector2 beforeDashPosition;

    private void OnDashSetup()
    {
        /*passedDashTime = 0;
        requiredDashTime = DashDistanceBlocks / DashSpeedBlocksPerSecond;*/
        beforeDashPosition = transform.position;
        StartCoroutine(StopDashFromTimeout());
    }

    private void OnDashTermination()
    {

    }

    public void OnDashChanged()
    {
        if (!canDash)
        {
            return;
        }
        canDash = false;
        Debug.Log("Dash it");
        dashDirection = lastNonzeroDesiredMoveDirection;
        if (state == PlayerState.Regular || state == PlayerState.Jumping || state == PlayerState.NotFalling)
        {
            SetState(PlayerState.Dashing);
        }
    }

    private void PerformDash()
    {
        var delta = ((Vector2)transform.position - beforeDashPosition).magnitude;

        if (delta > DashDistanceBlocks - 1e-2)
        {
            return;
        }

        rb.velocity += dashDirection * DashSpeedBlocksPerSecond;
    }

    public IEnumerator StopDashFromTimeout()
    {
        yield return new WaitForSeconds(DashDistanceBlocks / DashSpeedBlocksPerSecond);
        if (state == PlayerState.Dashing)
        {
            SetState(PlayerState.NotFalling);
        }
    }

    #endregion

    #region NotFalling

    private void OnNotFallingSetup()
    {
        StartCoroutine(ResumeFallingFromTimeout());
    }

    private void OnNotFallingTermination()
    {
        Debug.Log("terminate not falling");
    }

    private IEnumerator ResumeFallingFromTimeout()
    {
        yield return new WaitForSeconds(0.1f);
        if (state == PlayerState.NotFalling)
        {
            SetState(PlayerState.Regular);
        }
    }

    #endregion
}



public enum PlayerState
{
    Regular,
    Dashing,
    Jumping,
    NotFalling,
}
