using Farm.Inventory;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Farm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        public ItemToolTip itemToolTip;

        [Header("拖拽图片")]
        public Image dragItem;

        [Header("玩家背包UI")]
        [SerializeField] private GameObject bagUI;
        private bool bagOpened;

        [Header("通用背包")]
        [SerializeField] private GameObject baseBag;
        public GameObject shopSlotPrefab;


        [Header("交易UI")]
        public TradeUI tradeUI;
        public TextMeshProUGUI playerMoneyText;

        [SerializeField] private SlotUI[] playerSlots;
        [SerializeField] private List<SlotUI> baseBagSlots;

        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent += OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI += OnShowTradeUI;
        }
        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent -= OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI -= OnShowTradeUI;

        }



        private void Start()
        {
            for(int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].slotIndex = i;
            }
            // bagUI.activeInHierarchy 直接返回背包是否开启的bool值
            bagOpened = bagUI.activeInHierarchy;
            playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                OpenBagUI();
            }
        }
        private void OnShowTradeUI(ItemDetails item, bool isSell)
        {
            tradeUI.gameObject.SetActive(true);
            tradeUI.SetupTradeUI(item, isSell);
        }
        /// <summary>
        /// 打开通用包裹ui事件
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bagData"></param>
        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            // TODO :通用箱子 Prefab
            GameObject prefab = slotType switch
            {
                SlotType.Shop => shopSlotPrefab,
                _ => null,
            };
            //生成背包UI
            baseBag.SetActive(true);

            baseBagSlots = new List<SlotUI>();
            for(int i = 0; i < bagData.itemList.Count; i++)
            {
                var slot = Instantiate(prefab, baseBag.transform.GetChild(0)).GetComponent<SlotUI>();
                slot.slotIndex = i;
                baseBagSlots.Add(slot);
            }
            //添加好了格子后，强制刷新baseBag
            LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.GetComponent<RectTransform>());
            if(slotType == SlotType.Shop)
            {
                //通过变更主角背包ui的坐标向右移动-1原点位置，让两个面板到一块
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(-1, 0.5f);
                bagUI.SetActive(true);
                bagOpened = true;
            }
            //更新UI显示
            OnUpdateInventoryUI(InventoryLocation.Box, bagData.itemList);
        }
        /// <summary>
        /// 关闭通用包裹ui事件
        /// </summary>
        /// <param name="slotType"></param>
        /// <param name="bagData"></param>
        private void OnBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bagData)
        {
            baseBag.SetActive(false);
            itemToolTip.gameObject.SetActive(false);
            UpdateSlotHightlight(-1);

            foreach(var slot in baseBagSlots)
            {
                Destroy(slot.gameObject);
            }
            baseBagSlots.Clear();

            if (slotType == SlotType.Shop)
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                bagUI.SetActive(false);
                bagOpened = false;
            }
        }
        private void OnBeforeSceneUnloadEvent()
        {
            //切换场景时，要取消已经点击栏目的高亮
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
                case InventoryLocation.Box:
                    for (int i = 0; i < baseBagSlots.Count; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            baseBagSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            baseBagSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }
            playerMoneyText.text = InventoryManager.Instance.playerMoney.ToString();
        }
        /// <summary>
        /// 打开关闭背包UI， Button调用事件
        /// </summary>
        public void OpenBagUI()
        {
            //按B切换状态，true和false切换
            bagOpened = !bagOpened;
            bagUI.SetActive(bagOpened);
        }
        /// <summary>
        /// 更新slot高亮显示
        /// </summary>
        /// <param name="index">序号</param>
        /// index = -1时，取消所有的高亮
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

