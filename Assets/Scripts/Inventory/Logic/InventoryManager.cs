using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//所有和背包数据有关的内容和物品都放在这个同一个命名空间中Farm.Inventory;除非代码使用using Farm.Inventory,否则无法调用
//继承Singleton实现单例
namespace Farm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("物品数据")]
        public ItemDataList_SO itemDataList_SO;
        [Header("背包数据")]
        public InventoryBag_SO playerBag;

        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
        }
        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
        }



        private void Start()
        {
            //游戏开始时，更新一次ui
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 拖拽扔一次物品
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="pos"></param>
        private void OnDropItemEvent(int ID, Vector3 pos)
        {
            RemoveItem(ID, 1);
        }

        /// <summary>
        /// 通过ID，返回物品信息    ：任何代码想找到数据详情，那么调用GetItemDetails
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
        }
        /// <summary>
        /// 添加物品到Player背包里
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestory">是否要销毁物品</param>
        public void AddItem(Item item,bool toDestroy)
        {
            //是否已经有改物品
            var index = GetItemIndexInBag(item.itemID);

            AddItemAtIndex(item.itemID, index, 1);

            Debug.Log(GetItemDetails(item.itemID).itemID + "Name:" + GetItemDetails(item.itemID).itemName);
            if (toDestroy)
            {
                Destroy(item.gameObject);
            }
            //更新ui
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        /// <summary>
        /// 检查背包还有没有容量
        /// </summary>
        /// <returns></returns>
        private bool CheckBagCapacity()
        {
            // 如果id是0那么，代表这个背包的格子有空位,返回true
            for (int i=0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == 0)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 通过物品ID找到背包已有物品位置
        /// </summary>
        /// <param name="ID"></param>
        /// <returns> 如果没有这个物品就返回-1，否则返回序号</returns>
        private int GetItemIndexInBag(int ID)
        {
            for(int i=0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                {
                    return i;
                }
            }

            return -1;
        }
        /// <summary>
        /// 在指定背包序号位置添加物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="index">序号</param>
        /// <param name="amount">数量</param>
        private void AddItemAtIndex(int ID,int index,int amount)
        {
            if (index == -1 && CheckBagCapacity())
            {
                //背包没有这个物品 ：那么生成新物品，然后赋值到背包空位
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                for (int i = 0; i < playerBag.itemList.Count; i++)
                {
                    if (playerBag.itemList[i].itemID == 0)
                    {
                        playerBag.itemList[i] = item;
                        break;
                    }
                }
            }
            else
            {
                //背包有这个物品：  那么数量+Amount ,然后更新item的数量
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };
                playerBag.itemList[index] = item;
            }
        }
        /// <summary>
        /// Player 背包范围内交换物品
        /// </summary>
        /// <param name="fromIndex">起始序号</param>
        /// <param name="targetIndex">目标数据序号</param>
        public void SwapItem(int fromIndex,int targetIndex)
        {
            InventoryItem currentItem = playerBag.itemList[fromIndex];
            InventoryItem targetItem = playerBag.itemList[targetIndex];
            if (targetItem.itemID != 0)
            {
                playerBag.itemList[fromIndex] = targetItem;
                playerBag.itemList[targetIndex] = currentItem;
            }
            else
            {
                //如果拖拽到目标的格子是空格子，
                playerBag.itemList[targetIndex] = currentItem;
                //InventoryItem作为struct结构体，初始就设定数据Int为0
                playerBag.itemList[fromIndex] = new InventoryItem();
            }
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 移除指定数量的背包物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="removeAmount">数量</param>
        private void RemoveItem(int ID,int removeAmount)
        {
            var index = GetItemIndexInBag(ID);
            if (playerBag.itemList[index].itemAmount>removeAmount)
            {
                var amount = playerBag.itemList[index].itemAmount - removeAmount;
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                playerBag.itemList[index] = item;

            }
            else if(playerBag.itemList[index].itemAmount == removeAmount)
            {
                var item = new InventoryItem();
                playerBag.itemList[index] = item;
            }
            //更新玩家身上对应的列表
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
    }
}

