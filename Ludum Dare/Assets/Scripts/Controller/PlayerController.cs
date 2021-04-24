using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Tooltip("自己的刚体")]
    public Rigidbody2D mRigibody;

    [Tooltip("自己的动画机")]
    public Animator mAnimator;

    private float Horizontal;
    private float Vertical;

    [Tooltip("速度")]
    public float speed = 6f;

    [Tooltip("跳跃力度")]
    public float JumpForce = 20f;

    [Tooltip("地面")]
    public LayerMask GroundLayer;

    public Transform groundCheck;

    [SerializeField]
    private bool isJump;
    [SerializeField]
    private bool isPressJump;
    [SerializeField]
    private bool isOnGround;



    void Update()
    {
        //跳跃
        {
            if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
            {
                isPressJump = true;
            }
        }

        MoveForUpdate();

    }

    private void FixedUpdate()
    {
        //地面检测
        isOnGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, GroundLayer);

        MoveForFixedUpdate();
        Jump();
    }

    private void MoveForUpdate()
    {
        //取得移动变量
        Horizontal = Input.GetAxisRaw(Config.HoriString);
        Vertical = Input.GetAxisRaw(Config.VerString);

        //设置动画
        mAnimator.SetFloat(Config.HoriString, Mathf.Abs(Horizontal));
        mAnimator.SetFloat(Config.VerString, mRigibody.velocity.y);
        mAnimator.SetBool("isJump", isJump);

        //反转
        if (Horizontal != 0)
        {
            transform.localScale = new Vector3(Horizontal, 1, 1);
        }
    }

    private void MoveForFixedUpdate()
    {
        //改变水平速度
        mRigibody.velocity = new Vector2(Horizontal * speed, mRigibody.velocity.y);
        
    }

    private void Jump()
    {
        if (isOnGround)
        {
            isJump = false;
        }
        if (isPressJump && isOnGround)
        {
            isJump = true;
            isPressJump = false;
            mRigibody.velocity = new Vector2(mRigibody.velocity.x, JumpForce);
        }
    }
}
