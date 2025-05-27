using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Farm.Inventory
{
    //IPointerEnterHandler,IPointerExitHandler在鼠标指针进入、退出UI 元素区域时触发相应操作
    [RequireComponent(typeof(SlotUI))]
    public class ShowItemTooltip : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        private SlotUI slotUI;
        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();
        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if(slotUI.itemDetails != null)
            {
                inventoryUI.itemToolTip.gameObject.SetActive(true);
                inventoryUI.itemToolTip.SetupTooltip(slotUI.itemDetails, slotUI.slotType);

                //详情面板位置展示在触控的附件
                inventoryUI.itemToolTip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                inventoryUI.itemToolTip.transform.position = transform.position + Vector3.up * 60;
            }
            else
            {
                //itemAmount是0的话是空格子，那么关闭
                inventoryUI.itemToolTip.gameObject.SetActive(false);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.itemToolTip.gameObject.SetActive(false);

        }


    }
}

