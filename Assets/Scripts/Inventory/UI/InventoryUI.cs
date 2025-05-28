using Farm.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        public ItemToolTip itemToolTip;

        [Header("��קͼƬ")]
        public Image dragItem;

        [Header("��ұ���UI")]
        [SerializeField] private GameObject bagUI;

        private bool bagOpened;

        [SerializeField] private SlotUI[] playerSlots;
        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;

        }
        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;


        }


        private void Start()
        {
            for(int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].slotIndex = i;
            }
            // bagUI.activeInHierarchy ֱ�ӷ��ر����Ƿ�����boolֵ
            bagOpened = bagUI.activeInHierarchy;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                OpenBagUI();
            }
        }

        private void OnBeforeSceneUnloadEvent()
        {
            //�л�����ʱ��Ҫȡ���Ѿ������Ŀ�ĸ���
            UpdateSlotHightlight(-1);
        }
        private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
        {
            switch (location)
            {
                case InventoryLocation.Player:
                    for(int i = 0; i < playerSlots.Length; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            playerSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            playerSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;

            }
        }
        /// <summary>
        /// �򿪹رձ���UI�� Button�����¼�
        /// </summary>
        public void OpenBagUI()
        {
            //��B�л�״̬��true��false�л�
            bagOpened = !bagOpened;
            bagUI.SetActive(bagOpened);
        }
        /// <summary>
        /// ����slot������ʾ
        /// </summary>
        /// <param name="index">���</param>
        /// index = -1ʱ��ȡ�����еĸ���
        public void UpdateSlotHightlight(int index)
        {
            foreach(var slot in playerSlots)
            {
                if(slot.isSelected && slot.slotIndex == index)
                {
                    slot.slotHightlight.gameObject.SetActive(true);
                }
                else
                {
                    slot.isSelected = false;
                    slot.slotHightlight.gameObject.SetActive(false);

                }
            }
        }
    }
}

