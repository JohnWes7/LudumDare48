using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("运动控制")]
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

    [Header("事件检测")]
    public EventController SelectEvent;
    private bool canMove = true;
    private bool isInEvent = false;

    [Header("ESC")]
    public GameObject ESCPanel;

    private void Start()
    {
        Instance = this;
    }

    void Update()
    {
        //移动
        MoveForUpdate();

        //按键检测
        EventDetect();
        ESCDetect();


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
        //如果不能运动
        if (!canMove)
        {
            Horizontal = 0;
            Vertical = 0;

            goto pool;
        }

        //检测跳跃
        {
            if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
            {
                isPressJump = true;
            }
        }

        //取得移动变量
        Horizontal = Input.GetAxisRaw(Config.HoriString);
        Vertical = Input.GetAxisRaw(Config.VerString);

        pool:

        //设置动画
        mAnimator.SetFloat(Config.HoriString, Mathf.Abs(Horizontal));
        mAnimator.SetFloat(Config.VerString, mRigibody.velocity.y);
        mAnimator.SetBool("isJump", !isOnGround);

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
        //如果不能移动直接返回
        if (!canMove)
        {
            return;
        }

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

    public void SetSelectEvent(EventController eventController)
    {
        SelectEvent = eventController;
    }

    //player可以存储当前检测到的event来触发
    private void EventDetect()
    {
        if (SelectEvent != null)
        {
            //按e触发事件(条件：不在已经触发事件中)
            if (Input.GetKeyDown(KeyCode.E) && !isInEvent)
            {
                SelectEvent.TriggerEvent();
                SetCanMove(false);
                SetIsInEvent(true);
            }
        }
    }

    public void OnEventPanelClose()
    {
        SetCanMove(true);
        SetIsInEvent(false);
    }
    

    #region ESC弹窗系统

    private void ESCDetect()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ESCPanel == null)
            {
                //生成
                GameObject esc = Resources.Load<GameObject>(Config.ESCPanelPath);
                ESCPanel = Instantiate<GameObject>(esc, GameObject.Find("Canvas").transform);
                SetCanMove(false);
                SetIsInEvent(true);
            }
            else
            {
                OnECSPanelClose();
                Destroy(ESCPanel);
                ESCPanel = null;
            }
        }
    }

    public void ESCYesCallBack()
    {
        string laber = "Going Back...";
        GameManager.Instance.BeginFadeAni(() => {

            //完成开始动画后进行场景跳转
            GameManager.Instance.BeginAsyncLoadSence(Config.StartSceneName, () => {

                //完成跳转回调
                //结束动画
                GameManager.Instance.EndFadeAni(() => { Destroy(gameObject); });
            });

        }, laber);
    }
    public void OnECSPanelClose()
    {
        OnEventPanelClose();
    }
    #endregion

    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }
    public void SetIsInEvent(bool isInEvent)
    {
        this.isInEvent = isInEvent;
    }
}
