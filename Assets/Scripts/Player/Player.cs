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
    private bool inputDisable; //��Ҳ��ܲ���

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
    }
    /// <summary>
    /// �������ʱע���¼�����
    /// </summary>
    public void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;
    }


    /// <summary>
    /// �������ʱע���¼�����
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
        //ToDO ִ�ж���
        //��ֹ��û�����ӣ����͵��ˣ���������������¼����� ��Ʒ�仯��ִ�У�Ҫ�ڽ�ɫ����֮��
        EventHandler.CallExecuteActionAfterAnimation(pos, itemDetails); //��Ϊ�ܶද�����漰���޸Ķ�����Ϣ�����Զ�ExecuteActionAfterAnimation���ã�����GridMapManager
    }
    private void OnMoveToPosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }
    private void OnBeforeSceneUnloadEvent()
    {
        inputDisable = true; //���볡�����أ������ƶ�

    }
    private void OnAfterSceneLoadEvent()
    {
        inputDisable = false; //�����������غ󣬻ָ���ɫ���ƶ�

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

        //����б���ٶȣ�xy��ͬʱ+1(���־�һ����б������)���ᵼ��б���ƶ���������б�ߣ������Ҫ��������
        if(inputX != 0 && inputY != 0)
        {
            inputX = inputX * 0.6f;
            inputY = inputY * 0.6f;
        }

        //��·״̬�ٶ�   LeftShift��ס��������
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX = inputX * 0.5f;
            inputY = inputY * 0.5f;
        }

        movementInput = new Vector2(inputX, inputY);

        //�ж϶����Ƿ��л���isMoving��״̬����ʹ�õ�movementInput��ֵ�ı䶯�����������zero��ô
        isMoving = movementInput != Vector2.zero;
    }
    private void Movement()
    {
        //Time.deltaTime��Ҫ����������������ͬ�豸�ϲ�ͬ֡���������ٶ�Ϊ��ͬ
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
