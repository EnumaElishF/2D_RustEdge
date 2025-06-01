using Farm.CropPlant;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Farm.Map;
public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed, item;
    private Sprite currentSprite;  //�洢��ǰͼƬ
    private Image cursorImage;
    private RectTransform cursorCanvas;

    //�����
    private Camera mainCamera; //��Ļ����ת������������
    private Grid currentGrid; //��������ת������������Grid���л�������ʱ����Ҫ���ǵ�ǰ������Grid��������Ҫ

    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private bool cursorEnable;//�������Կ���

    private bool cursorPositionValid; //��굱ǰ����λ���Ƿ����

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
        //ui�����Transform����RectTransform
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();//�ҹ̶�λ�õĵ�һ�����ӣ��ǵ�λ�ò�Ҫ��ק���˸�������λ��
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
        //ģ�����ͼ�꣬�򵥴ֱ��ĸ�������ƶ�
        cursorImage.transform.position = Input.mousePosition;

        if (!InteractWithUI() && cursorEnable) //ͬʱҪ��������
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
    /// // ���������벢�����¼�
    /// </summary>
    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionValid)
        {
            //�����ʱ�򣬴��ݣ�����������¼���-�����ݸ�Player�Ŀ�ʼ���ģ���ȡ�����Ĵ��¼�֮�䡣
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);
        }
    }
    /// <summary>
    /// ����ж��֮ǰ�����CursorҪ�����õ�
    /// </summary>
    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;
    }
    /// <summary>
    /// ע�� �л���������أ���������
    /// </summary>
    private void OnAfterSceneLoadedEvent()
    {
        //���л�����֮�� ��Ҫ�õ�����
        //FindAnyObjectByType����
        currentGrid = FindAnyObjectByType<Grid>();
    }

    #region ���������ʽ

    /// <summary>
    /// �������ͻͼƬ
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }
    /// <summary>
    /// ����������
    /// </summary>
    private void SetCursorValid() 
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// ������겻����
    /// </summary>
    private void SetCursorInValid()
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.4f);
    }

    #endregion

    /// <summary>
    /// ��Ʒѡ���¼�����,�¼����ƣ����ͼƬ������
    /// </summary>
    /// <param name="itemDetails"></param>
    /// <param name="isSelected"></param>
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (!isSelected)
        {
            currentItem = null;
            //��Ʒû�б�ѡ�С�Ҳ�������û��ѡ���κζ���
            cursorEnable = false;
            currentSprite = normal;
        }
        else
        {
            currentItem = itemDetails;
            // WORKFLOW: ����������Ͷ�ӦͼƬ
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
        //������ָ���Ƿ����
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,-mainCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        var playerGridPos = currentGrid.WorldToCell(PlayerTransform.position);

        //�ж� ��Ʒ�ķ��ã����ܳ��� ���趨���ڵ��ϵ� ��Χ��
        if(Mathf.Abs(mouseGridPos.x - playerGridPos.x)>currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInValid(); // ����������������ָ��ͻ�� ��͸������������
            //ͬʱ�������Ҫʵ���񼢻��������������Χ�⣬��ô��ɫ��˵�����ѣ����ܳ�������
            //������дPlayer˵���Ľӿڣ����Ѽ���  �������������У��½��� playerSpeaking
            return;
        }

        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);

        if (currentTile != null)
        {
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currentTile.seedItemID);
            // WORKFLOW: ����������Ʒ���͵��ж�
            switch (currentItem.itemType)
            {
                case ItemType.Seed:
                    if (currentTile.daySinceDug > -1 && currentTile.seedItemID == -1)
                    { //�ڿ�ʱ��Ҫ����-1�����ڿӣ�����Ϊ-1�����ǲ����Ѿ�����������
                        //Debug.Log("���ͨ��" + currentTile);
                        SetCursorValid(); 
                    }
                    else SetCursorInValid();
                    break;
                case ItemType.Commodity:
                    if (currentTile.canDropItem && currentItem.canDropped) SetCursorValid(); else SetCursorInValid(); //��Ʒ���Ӿ�������
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
                            //Ҫ�ڴ˸��������������ڴ����趨��ֲ��ɳ������ڣ������ٵ��
                            if (currentTile.growthDays >= currentCrop.TotalGrowthDays) SetCursorValid(); else SetCursorInValid();
                        }


                    }else
                        SetCursorInValid(); //û������
                    break;


            }
        }
        else
        {
            SetCursorInValid();
        }
    }
    /// <summary>
    /// �Ƿ���ui����
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
