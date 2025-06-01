using Farm.CropPlant;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Farm.Map;
public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed, item;
    private Sprite currentSprite;  //存储当前图片
    private Image cursorImage;
    private RectTransform cursorCanvas;

    //鼠标检测
    private Camera mainCamera; //屏幕坐标转换成世界坐标
    private Grid currentGrid; //世界坐标转换成网格坐标Grid：切换场景的时候需要的是当前场景的Grid，所以需要

    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private bool cursorEnable;//鼠标可用性控制

    private bool cursorPositionValid; //鼠标当前坐标位置是否可用

    private ItemDetails currentItem;
    private Transform PlayerTransform => FindAnyObjectByType<Player>().transform;
    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }
    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;

    }



    private void Start()
    {
        //ui组件的Transform都是RectTransform
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();//找固定位置的第一个孩子，记得位置不要拖拽错了更新他的位置
        currentSprite = normal;
        SetCursorImage(normal);

        mainCamera = Camera.main;
    }
    private void Update()
    {
        if(cursorCanvas == null)
        {
            return;
        }
        //模拟鼠标图标，简单粗暴的跟随鼠标移动
        cursorImage.transform.position = Input.mousePosition;

        if (!InteractWithUI() && cursorEnable) //同时要求鼠标可用
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();
            CheckPlayerInput();
        }
        else
        {
            SetCursorImage(normal);
        }

    }

    /// <summary>
    /// // 检测玩家输入并触发事件
    /// </summary>
    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionValid)
        {
            //点击的时候，传递，进行鼠标点击事件，-》传递给Player的开始订阅，和取消订阅此事件之间。
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);
        }
    }
    /// <summary>
    /// 场景卸载之前，鼠标Cursor要被禁用掉
    /// </summary>
    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;
    }
    /// <summary>
    /// 注册 切换场景后加载，函数方法
    /// </summary>
    private void OnAfterSceneLoadedEvent()
    {
        //在切换场景之后 ，要拿到网格
        //FindAnyObjectByType还是
        currentGrid = FindAnyObjectByType<Grid>();
    }

    #region 设置鼠标样式

    /// <summary>
    /// 设置鼠标突图片
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }
    /// <summary>
    /// 设置鼠标可用
    /// </summary>
    private void SetCursorValid() 
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// 设置鼠标不可用
    /// </summary>
    private void SetCursorInValid()
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.4f);
    }

    #endregion

    /// <summary>
    /// 物品选择事件函数,事件控制，鼠标图片种类变更
    /// </summary>
    /// <param name="itemDetails"></param>
    /// <param name="isSelected"></param>
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (!isSelected)
        {
            currentItem = null;
            //物品没有被选中。也就是鼠标没有选中任何东西
            cursorEnable = false;
            currentSprite = normal;
        }
        else
        {
            currentItem = itemDetails;
            // WORKFLOW: 添加所有类型对应图片
            currentSprite = itemDetails.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.Commodity => item,
                ItemType.ChopTool => tool,
                ItemType.HoeTool => tool,
                ItemType.WaterTool => tool,
                ItemType.BreakTool => tool,
                ItemType.ReapTool => tool,
                ItemType.Furniture => tool,
                ItemType.CollectTool => tool,
                _ => normal
            };
            cursorEnable = true;

        }

    }

    private void CheckCursorValid()
    {
        //检测鼠标指针是否可用
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,-mainCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        var playerGridPos = currentGrid.WorldToCell(PlayerTransform.position);

        //判断 物品的放置，不能超出 在设定可在地上的 范围内
        if(Mathf.Abs(mouseGridPos.x - playerGridPos.x)>currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInValid(); // 调用这个方法，鼠标指针就会变 半透明，代表不可扔
            //同时，如果想要实现像饥荒那样，在这个范围外，那么角色会说话提醒，不能朝这里扔
            //在这里写Player说话的接口，提醒即可  ：就像这样就行，新建类 playerSpeaking
            return;
        }

        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);

        if (currentTile != null)
        {
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currentTile.seedItemID);
            // WORKFLOW: 补充所有物品类型的判断
            switch (currentItem.itemType)
            {
                case ItemType.Seed:
                    if (currentTile.daySinceDug > -1 && currentTile.seedItemID == -1)
                    { //挖坑时间要大于-1，有挖坑；种子为-1，就是不能已经有其他种子
                        //Debug.Log("鼠标通过" + currentTile);
                        SetCursorValid(); 
                    }
                    else SetCursorInValid();
                    break;
                case ItemType.Commodity:
                    if (currentTile.canDropItem && currentItem.canDropped) SetCursorValid(); else SetCursorInValid(); //商品能扔就鼠标可用
                    break;
                case ItemType.HoeTool:
                    if (currentTile.canDig) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.WaterTool:
                    if (currentTile.daySinceDug > -1 && currentTile.daySinceWatered == -1) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.CollectTool:
                    if (currentCrop != null)
                    {
                        if (currentCrop.CheckToolAvailable(currentItem.itemID))
                        {
                            //要在此格子上生长的日期大于设定的植物成长总日期，才能再点击
                            if (currentTile.growthDays >= currentCrop.TotalGrowthDays) SetCursorValid(); else SetCursorInValid();
                        }


                    }else
                        SetCursorInValid(); //没有种子
                    break;


            }
        }
        else
        {
            SetCursorInValid();
        }
    }
    /// <summary>
    /// 是否与ui互动
    /// </summary>
    /// <returns></returns>
    private bool InteractWithUI()
    {
        if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
