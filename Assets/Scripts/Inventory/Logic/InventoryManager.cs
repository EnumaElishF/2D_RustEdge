using Farm.Save;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

//所有和背包数据有关的内容和物品都放在这个同一个命名空间中Farm.Inventory;除非代码使用using Farm.Inventory,否则无法调用
//继承Singleton实现单例
namespace Farm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>,ISaveable
    {
        [Header("物品数据")]
        public ItemDataList_SO itemDataList_SO;
        [Header("建造蓝图")]
        public BluePrintDataList_SO bluePrintData;
        [Header("背包数据")]
        public InventoryBag_SO playerBag;
        private InventoryBag_SO currentBoxBag; //当前箱子打开的数据
        [Header("交易")]
        public int playerMoney;

        private Dictionary<string, List<InventoryItem>> boxDataDict = new Dictionary<string, List<InventoryItem>>();
        public int BoxDataAmount => boxDataDict.Count;

        public string GUID => GetComponent<DataGUID>().guid;


        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;
            //建造
            EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
            EventHandler.BaseBagOpenEvent += OnBaseBagOpenEvent;
        }
        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;
            EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
            EventHandler.BaseBagOpenEvent -= OnBaseBagOpenEvent;

        }

        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bag_SO)
        {
            currentBoxBag = bag_SO;
        }

        private void Start()
        {
            //ISaveable进行注册
            ISaveable saveable = this;
            saveable.RegisterSaveable();

            //游戏开始时，更新一次ui
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);


        }
        /// <summary>
        /// OnBuildFurnitureEvent：建造事件的移除物品
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="mousePos"></param>
        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            RemoveItem(ID, 1);
            BluePrintDetails bluePrint = bluePrintData.GetBluePrintDetails(ID);
            foreach(var item in bluePrint.resourceItem)
            {
                RemoveItem(item.itemID, item.itemAmount);
            }
        }


        /// <summary>
        /// 拖拽扔一次物品
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="pos"></param>
        private void OnDropItemEvent(int ID, Vector3 pos, ItemType itemType)
        {
            RemoveItem(ID, 1);
        }

        /// <summary>
        /// 背包生成物品
        /// </summary>
        /// <param name="ID"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OnHarvestAtPlayerPosition(int ID)
        {
            //是否已经有该物品
            var index = GetItemIndexInBag(ID);
            //添加物品
            AddItemAtIndex(ID, index, 1);
            //更新ui
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
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

            //Debug.Log(GetItemDetails(item.itemID).itemID + "Name:" + GetItemDetails(item.itemID).itemName);
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
        /// 跨背包交换数据
        /// </summary>
        /// <param name="locationFrom"></param>
        /// <param name="fromIndex"></param>
        /// <param name="locationTarget"></param>
        /// <param name="targetIndex"></param>
        public void SwapItem(InventoryLocation locationFrom,int fromIndex,InventoryLocation locationTarget,int targetIndex)
        {
            var currentList = GetItemList(locationFrom);
            var targetList = GetItemList(locationTarget);
            InventoryItem currentItem = currentList[fromIndex];
            if (targetIndex < targetList.Count)
            {
                InventoryItem targetItem = targetList[targetIndex];
                if(targetItem.itemID != 0 && currentItem.itemID != targetItem.itemID)//有不相同的两个物品
                {
                    currentList[fromIndex] = targetItem;
                    targetList[targetIndex] = currentItem;
                }else if(currentItem.itemID == targetItem.itemID) //相同的两个物品
                {
                    targetItem.itemAmount += currentItem.itemAmount;
                    targetList[targetIndex] = targetItem;
                    currentList[fromIndex] = new InventoryItem();
                }else //目标是空格子
                {
                    targetList[targetIndex] = currentItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                //更新一下ui，自己的ui和对方的ui
                EventHandler.CallUpdateInventoryUI(locationFrom, currentList); 
                EventHandler.CallUpdateInventoryUI(locationTarget,targetList);
            }
        }

        /// <summary>
        /// 根据位置返回背包数据列表
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private List<InventoryItem> GetItemList(InventoryLocation location)
        {
            return location switch 
            { 
                InventoryLocation.Player => playerBag.itemList,
                InventoryLocation.Box => currentBoxBag.itemList,
                _ => null,
            };
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
        /// <summary>
        /// 交易物品 （Player交易金币变化)
        /// </summary>
        /// <param name="itemDetails"></param>
        /// <param name="amount"交易数量></param>
        /// <param name="isSellTrade"></param>
        public void TradeItem(ItemDetails itemDetails,int amount,bool isSellTrade)
        {
            int cost = itemDetails.itemPrice * amount;
            //获得物品背包位置
            int index = GetItemIndexInBag(itemDetails.itemID);
            if (isSellTrade) //卖
            {
                if (playerBag.itemList[index].itemAmount >= amount)
                {
                    RemoveItem(itemDetails.itemID, amount);
                    //卖出总价
                    cost = (int)(cost * itemDetails.sellPercentage);
                    playerMoney += cost;
                }
            }else if(playerMoney - cost >= 0) //买 :前提判断是要求，玩家现有金额大于cost
            {
                if (CheckBagCapacity())
                {
                    AddItemAtIndex(itemDetails.itemID, index, amount);
                }
                playerMoney -= cost;
            }
            //刷新ui
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
        /// <summary>
        /// 检查建筑资源所需物品库存情况
        /// </summary>
        /// <param name="ID">图纸ID</param>
        /// <returns></returns>
        public bool CheckStock(int ID)
        {
            var bluePrintDetails = bluePrintData.GetBluePrintDetails(ID);
            foreach(var resourceItem in bluePrintDetails.resourceItem)
            {
                var itemStock = playerBag.GetInventoryItem(resourceItem.itemID);
                if (itemStock.itemAmount >= resourceItem.itemAmount)
                {
                    continue;
                }
                else
                {
                    //TODO :角色提醒对话上,提示材料不足 (做一个角色说话的提示录，在这里编入字符串，直接展示到上面，但是好像不适合非程序员做？)
                    Debug.Log("建筑所需材料不足");
                    return false;
                }
            }
            //如果蓝图所需材料的：每一个现有库存itemStock数量都是足够的，那么返回true
            return true;
        }

        /// <summary>
        /// 查找箱子数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<InventoryItem> GetBoxDataList(string key)
        {
            if (boxDataDict.ContainsKey(key))
                return boxDataDict[key];
            else 
                return null;
        }
        /// <summary>
        /// 加入箱子数据字典
        /// </summary>
        /// <param name="box"></param>
        public void AddBoxDataDict(Box box)
        {
            var key = box.name + box.index;
            if (!boxDataDict.ContainsKey(key))
            {
                boxDataDict.Add(key, box.boxBagData.itemList);
            }
            Debug.Log(key);
        }

        /// <summary>
        /// 存储数据: 实现自ISaveable接口的 GenerateSaveData
        /// </summary>
        /// <returns></returns>
        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.playerMoney = this.playerMoney; //钱
            saveData.inventoryDict.Add(playerBag.name, playerBag.itemList); //背包
            foreach(var item in boxDataDict) //箱子
            {
                saveData.inventoryDict.Add(item.Key, item.Value);
            }

            return saveData;
        }
        /// <summary>
        /// 生成恢复数据：实现自ISaveable接口的 RestoreData
        /// </summary>
        /// <param name="saveData"></param>
        public void RestoreData(GameSaveData saveData)
        {
            this.playerMoney = saveData.playerMoney; //钱
            playerBag.itemList = saveData.inventoryDict[playerBag.name]; //背包
            
            foreach(var item in saveData.inventoryDict)//箱子
            {
                if (boxDataDict.ContainsKey(item.Key))
                {
                    boxDataDict[item.Key] = item.Value;
                }
            }
            //重新生成图标的时候，要刷新他的ui，才能正常显示 :
            //这里只刷新了Player的背包ui，箱子的数据ActionBar在每次打开箱子就写好了刷新
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
    }
}

