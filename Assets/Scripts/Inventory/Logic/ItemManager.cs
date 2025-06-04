using Farm.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Farm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrefab;
        public Item bounceItemPrefab;
        private Transform itemParent;

        private Transform PlayerTransform => FindAnyObjectByType<Player>().transform;

        //记录创建Item (场景物品存储_字典类型)
        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();

        private void OnEnable()
        {
            //InstantiateItemInScene在地图上生成物体
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            //创建切换后
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadEvent;
        }

        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.DropItemEvent -= OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadEvent;

        }



        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItems();
        }

        private void OnAfterSceneLoadEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            RecreateAllItems();
        }

        /// <summary>
        /// 在指定位置生成物品
        /// </summary>
        /// <param name="ID">物品id</param>
        /// <param name="pos">物品坐标</param>
        private void OnInstantiateItemInScene(int ID, Vector3 pos)
        {
            //Quaternion.identity 是四元数旋转系统的基础，用于表示 “无旋转” 状态
            var item = Instantiate(bounceItemPrefab, pos, Quaternion.identity, itemParent);
            item.itemID = ID;

            //实现从高处往下掉落的产生效果
            item.GetComponent<ItemBounce>().InitBounceItem(pos, Vector3.up);

        }

        /// <summary>
        /// 扔物品，bounceItemPrefab
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="pos"></param>
        private void OnDropItemEvent(int ID, Vector3 mousePos,ItemType itemType)
        {
            if (itemType == ItemType.Seed) return; //种子不需要执行扔地上的效果

            //扔东西的效果
            var item = Instantiate(bounceItemPrefab, PlayerTransform.position, Quaternion.identity, itemParent);
            item.itemID = ID;
            //获得扔的方向
            var dir = (mousePos - PlayerTransform.position).normalized;
            item.GetComponent<ItemBounce>().InitBounceItem(mousePos,dir);
        }

        /// <summary>
        /// 获取当前场景所有Item
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();
            // FindObjectsOfType被FindObjectsByType取代，以后需要使用新版
            Item[] existingItems = Object.FindObjectsByType<Item>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var item in existingItems)
            {
                SceneItem sceneItem = new SceneItem
                {
                    itemID = item.itemID,
                    position = new SerializableVector3(item.transform.position)
                };
                currentSceneItems.Add(sceneItem);
            }
            if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                //找到数据就更新item数据列表
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItems;
            }
            else  //如果是新场景
            {
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItems);
            }
        }
        /// <summary>
        /// 刷新重建当前场景的物品
        /// </summary>
        private void RecreateAllItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();
            if(sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name,out currentSceneItems))
            {
                if (currentSceneItems != null)
                {
                    // 清场 - 使用现代的写法：FindObjectOfType过时，被FindObjectsByType取代
                    Item[] existingItems = Object.FindObjectsByType<Item>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                    foreach (var item in existingItems)
                    {
                        Destroy(item.gameObject);
                    }
                    //重建
                    foreach(var item in currentSceneItems)
                    {
                        Item newItem = Instantiate(itemPrefab, item.position.ToVector3(), Quaternion.identity, itemParent);
                        newItem.Init(item.itemID);
                    }
                }
            }
        }
    }
}


