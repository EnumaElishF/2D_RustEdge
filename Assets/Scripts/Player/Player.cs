using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    public float speed;
    private float inputX;
    private float inputY;
    private Vector2 movementInput;

    private Animator[] animators;
    private bool isMoving;
    private bool inputDisable; //玩家不能操作

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
    }
    /// <summary>
    /// 组件启用时注册事件监听
    /// </summary>
    public void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;
    }


    /// <summary>
    /// 组件禁用时注销事件监听
    /// </summary>
    public void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition -= OnMoveToPosition;
        EventHandler.MouseClickedEvent -= OnMouseClickedEvent;

    }

    private void OnMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    {
        //ToDO 执行动画
        //防止还没举起斧子，树就倒了，这种情况。所以事件控制 物品变化的执行，要在角色动画之后
        EventHandler.CallExecuteActionAfterAnimation(pos, itemDetails); //因为很多动作都涉及到修改动作信息，所以对ExecuteActionAfterAnimation调用，放在GridMapManager
    }
    private void OnMoveToPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }
    private void OnBeforeSceneUnloadEvent()
    {
        inputDisable = true; //进入场景加载，不可移动

    }
    private void OnAfterSceneLoadEvent()
    {
        inputDisable = false; //结束场景加载后，恢复角色可移动

    }



    private void Update()
    {
        if (!inputDisable)
        {
            PlayerInput();
        }
        else
        {
            isMoving = false;
        }
        SwitchAnimation();


    }
    private void FixedUpdate()
    {
        if(!inputDisable)
            Movement();
    }
    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        //修正斜向速度，xy轴同时+1(这种就一定是斜方向走)，会导致斜向移动是三角形斜边，距离大，要调整距离
        if(inputX != 0 && inputY != 0)
        {
            inputX = inputX * 0.6f;
            inputY = inputY * 0.6f;
        }

        //走路状态速度   LeftShift按住换成慢走
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX = inputX * 0.5f;
            inputY = inputY * 0.5f;
        }

        movementInput = new Vector2(inputX, inputY);

        //判断动画是否切换成isMoving的状态，按使用的movementInput数值的变动，如果下面是zero那么
        isMoving = movementInput != Vector2.zero;
    }
    private void Movement()
    {
        //Time.deltaTime主要是能做到，修正不同设备上不同帧数的运行速度为相同
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }
    private void SwitchAnimation()
    {
        foreach( var anim in animators)
        {
            anim.SetBool("isMoving", isMoving);
            if (isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
        }
    }
}
