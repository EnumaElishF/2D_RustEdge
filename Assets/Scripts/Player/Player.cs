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

    //����ʹ�ù���
    private float mouseX;
    private float mouseY;

    private bool useTool;

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

    private void OnMouseClickedEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        //��ִ�ж���   itemDetails.itemTypep�ų����ӣ���Ʒ���Ҿ�
        if (itemDetails.itemType!=ItemType.Seed&&itemDetails.itemType!=ItemType.Commodity&& itemDetails.itemType != ItemType.Furniture)
        {
            mouseX = mouseWorldPos.x - transform.position.x;
            mouseY = mouseWorldPos.y - transform.position.y;

            //X�Ĳ�ֵ������Y�Ĳ�ֵ :�����������ƫ�ƣ���ԶԶ���������������ľ��룻����Ҫ�����ж��������������  
            //��֮���ж�����
            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
            {
                //����ֻ����һ������Ķ����������ǵ�Blend Tree ֪����ô�仯����
                mouseY = 0;
            }
            else
            {
                mouseX = 0;
            }
            StartCoroutine(UseToolRoutinue(mouseWorldPos, itemDetails));
        }
        else
        {
            //���ӣ���Ʒ���Ҿ�ֱ���ڵ�������
            //��ֹ��û�����ӣ����͵��ˣ���������������¼����� ��Ʒ�仯��ִ�У�Ҫ�ڽ�ɫ����֮��
            EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails); //��Ϊ�ܶද�����漰���޸Ķ�����Ϣ�����Զ�ExecuteActionAfterAnimation���ã�����GridMapManager
        }
    }
    /// <summary>
    /// Э�̣�����ִ�й����У�����һ����ʵ�ĵ��������ͬ��Ч��
    /// </summary>
    /// <param name="mouseWorldPos"></param>
    /// <param name="itemDetails"></param>
    /// <returns></returns>
    private IEnumerator UseToolRoutinue(UnityEngine.Vector3 mouseWorldPos,ItemDetails itemDetails)
    {
        useTool = true;
        inputDisable = true;
        yield return null;
        foreach(var anim in animators)
        {
            anim.SetTrigger("useTool");
            //������泯����
            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);
        }
        yield return new WaitForSeconds(0.45f);
        EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);
        yield return new WaitForSeconds(0.25f);

        //�ȴ���������
        useTool = false;
        inputDisable = false;
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
            anim.SetFloat("mouseX", mouseX);
            anim.SetFloat("mouseY", mouseY);
            if (isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
        }
    }
}
