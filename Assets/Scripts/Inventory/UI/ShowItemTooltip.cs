using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Farm.Inventory
{
    //IPointerEnterHandler,IPointerExitHandler�����ָ����롢�˳�UI Ԫ������ʱ������Ӧ����
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

                //�������λ��չʾ�ڴ��صĸ���
                inventoryUI.itemToolTip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                inventoryUI.itemToolTip.transform.position = transform.position + Vector3.up * 60;
            }
            else
            {
                //itemAmount��0�Ļ��ǿո��ӣ���ô�ر�
                inventoryUI.itemToolTip.gameObject.SetActive(false);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.itemToolTip.gameObject.SetActive(false);

        }


    }
}

