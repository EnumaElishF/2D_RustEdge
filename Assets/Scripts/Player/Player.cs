using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Farm.Save;
public class Player : MonoBehaviour,ISaveable
{
    private Rigidbody2D rb;
    public float speed;
    private float inputX;
    private float inputY;
    private Vector2 movementInput;

    private Animator[] animators;
    private bool isMoving;
    private bool inputDisable; //true时玩家不能操作

    //动画使用工具
    private float mouseX;
    private float mouseY;

    private bool useTool;

    public string GUID => GetComponent<DataGUID>().guid;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();
        inputDisable = true;
    }
    private void Start()
    {
        //ISaveable进行注册
        ISaveable saveable = this;
        saveable.RegisterSaveable();
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
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;

        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
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
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;

        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;


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
        if (!inputDisable)
            Movement();
    }

    private void OnStartNewGameEvent(int obj)
    {
        inputDisable = false; //设置成游戏一开始就可以控制人物了
        transform.position = Settings.playerStartPos;

    }

    private void OnUpdateGameStateEvent(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.GamePlay:
                inputDisable = false;
                break;
            case GameState.Pause:
                inputDisable = true; 
                break;
        }
    }

    private void OnMouseClickedEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        //！执行动画   itemDetails.itemTypep排除种子，商品，家具
        if (itemDetails.itemType!=ItemType.Seed&&itemDetails.itemType!=ItemType.Commodity&& itemDetails.itemType != ItemType.Furniture)
        {
            mouseX = mouseWorldPos.x - transform.position.x;
            mouseY = mouseWorldPos.y - (transform.position.y + 0.85f);

            //X的差值，大于Y的差值 :鼠标横向坐标的偏移，会远远大于纵向的离人物的距离；就需要优先判断左右这种情况。  
            //反之，判断上下
            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
            {
                //这样只传递一个方向的动画，让我们的Blend Tree 知道怎么变化动作
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
            //种子，商品，家具直接在地面生成
            //防止还没举起斧子，树就倒了，这种情况。所以事件控制 物品变化的执行，要在角色动画之后

            // TODO 做点击鼠标时，提示一下，为什么不能做，例如建筑材料不足等原因

            EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails); //因为很多动作都涉及到修改动作信息，所以对ExecuteActionAfterAnimation调用，放在GridMapManager
        }
    }
    /// <summary>
    /// 协程：动画执行过程中，触发一个真实的地面产生不同的效果
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
            //人物的面朝方向
            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);
        }
        yield return new WaitForSeconds(0.45f);
        EventHandler.CallExecuteActionAfterAnimation(mouseWorldPos, itemDetails);
        yield return new WaitForSeconds(0.25f);

        //等待动画结束
        useTool = false;
        inputDisable = false;
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
            anim.SetFloat("mouseX", mouseX);
            anim.SetFloat("mouseY", mouseY);
            if (isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
        }
    }
    /// <summary>
    /// 存储数据: 实现自ISaveable接口的 GenerateSaveData
    /// </summary>
    /// <returns></returns>
    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.characterPosDict = new Dictionary<string, SerializableVector3>();
        saveData.characterPosDict.Add(this.name,new SerializableVector3(transform.position)); //Player坐标

        return saveData;
    }
    /// <summary>
    /// 生成恢复数据：实现自ISaveable接口的 RestoreData
    /// </summary>
    /// <param name="saveData"></param>
    public void RestoreData(GameSaveData saveData)
    {
        var targetPosition = saveData.characterPosDict[this.name].ToVector3();//Player坐标
        transform.position = targetPosition; 
    }
}
