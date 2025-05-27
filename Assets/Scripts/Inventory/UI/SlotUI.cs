using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Farm.Inventory;

namespace Farm.Inventory
{
    //IPointerClickHandler点按时间，实现该接口. 
    //IBeginDragHandler开始拖拽,IDragHandler拖拽过程,IEndDragHandler停止拖拽
    public class SlotUI : MonoBehaviour,IPointerClickHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
    {
        [Header("组件获取")]
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amountText;
        public Image slotHightlight;
        [SerializeField] private Button button;

        [Header("格子类型")]
        public SlotType slotType;
        public bool isSelected;
        public int slotIndex;

        //物品信息
        public ItemDetails itemDetails;
        public int itemAmount;
        public InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        private void Start()
        {
            isSelected = false;
            if (itemDetails == null)
            {
                UpdateEmptySlot();
            }
        }
        /// <summary>
        /// 更新格子的UI和信息
        /// </summary>
        /// <param name="item">ItemDetails</param>
        /// <param name="amount">持有的数量</param>
        public void UpdateSlot(ItemDetails item, int amount)
        {
            itemDetails = item;
            slotImage.sprite = item.itemIcon;
            itemAmount = amount;
            amountText.text = amount.ToString();
            slotImage.enabled = true;
            button.interactable = true;
        }

        /// <summary>
        /// slot更新为空
        /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;
                inventoryUI.UpdateSlotHightlight(-1);
                //通知物品被选中的状态和信息
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
            itemDetails = null;
            //格子置空，取消图片显示，文字显示，按钮交互
            slotImage.enabled = false;
            amountText.text = string.Empty;
            button.interactable = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemDetails == null) return;
            isSelected = !isSelected;
            inventoryUI.UpdateSlotHightlight(slotIndex);

            if(slotType == SlotType.Bag)
            {
                //通知物品被选中的状态和信息
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            //开始拖拽时判断，格子能不能拖拽
            if (itemAmount != 0)
            {
                inventoryUI.dragItem.enabled = true;
                inventoryUI.dragItem.sprite = slotImage.sprite;
                inventoryUI.dragItem.SetNativeSize(); //图片设置为本来的尺寸
                isSelected = true;
                inventoryUI.UpdateSlotHightlight(slotIndex);
            }
            
        }

        public void OnDrag(PointerEventData eventData)
        {
            //拖拽过程中，将移动图片位置=鼠标位置
            inventoryUI.dragItem.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.enabled = false;
            //Debug.Log(eventData.pointerCurrentRaycast.gameObject);
            if(eventData.pointerCurrentRaycast.gameObject != null)
            {
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null)
                { //检测，拖拽碰撞的gameObject是不是SlotUI，如果不是就不做反应
                    return;
                }
                var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                int targetIndex = targetSlot.slotIndex;
                //在Player自身背包范围内交换
                if(slotType == SlotType.Bag && targetSlot.slotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
                }
                //清空所有高亮显示
                inventoryUI.UpdateSlotHightlight(-1);
            }
            //else  //测试扔在地上
            //{
            //    if (itemDetails.canDropped) //必须是设定可扔的物品
            //    {
            //        //鼠标松开时，对应世界地图坐标 ：摄像机默认情况下，z轴是-10，摄像机离地面实际距离是-10，需要把这个部分补上
            //        var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

            //        EventHandler.CallInstantiateItemInScene(itemDetails.itemID, pos);
            //    }

            //}
        }
    }

}
