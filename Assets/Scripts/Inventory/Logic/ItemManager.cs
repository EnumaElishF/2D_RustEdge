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

        //��¼����Item (������Ʒ�洢_�ֵ�����)
        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();

        private void OnEnable()
        {
            //InstantiateItemInScene�ڵ�ͼ����������
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.DropItemEvent += OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            //�����л���
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
        /// ��ָ��λ��������Ʒ
        /// </summary>
        /// <param name="ID">��Ʒid</param>
        /// <param name="pos">��Ʒ����</param>
        private void OnInstantiateItemInScene(int ID, Vector3 pos)
        {
            //Quaternion.identity ����Ԫ����תϵͳ�Ļ��������ڱ�ʾ ������ת�� ״̬
            var item = Instantiate(bounceItemPrefab, pos, Quaternion.identity, itemParent);
            item.itemID = ID;

            //ʵ�ִӸߴ����µ���Ĳ���Ч��
            item.GetComponent<ItemBounce>().InitBounceItem(pos, Vector3.up);

        }

        /// <summary>
        /// ����Ʒ��bounceItemPrefab
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="pos"></param>
        private void OnDropItemEvent(int ID, Vector3 mousePos,ItemType itemType)
        {
            if (itemType == ItemType.Seed) return; //���Ӳ���Ҫִ���ӵ��ϵ�Ч��

            //�Ӷ�����Ч��
            var item = Instantiate(bounceItemPrefab, PlayerTransform.position, Quaternion.identity, itemParent);
            item.itemID = ID;
            //����ӵķ���
            var dir = (mousePos - PlayerTransform.position).normalized;
            item.GetComponent<ItemBounce>().InitBounceItem(mousePos,dir);
        }

        /// <summary>
        /// ��ȡ��ǰ��������Item
        /// </summary>
        private void GetAllSceneItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();
            // FindObjectsOfType��FindObjectsByTypeȡ�����Ժ���Ҫʹ���°�
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
                //�ҵ����ݾ͸���item�����б�
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItems;
            }
            else  //������³���
            {
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItems);
            }
        }
        /// <summary>
        /// ˢ���ؽ���ǰ��������Ʒ
        /// </summary>
        private void RecreateAllItems()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();
            if(sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name,out currentSceneItems))
            {
                if (currentSceneItems != null)
                {
                    // �峡 - ʹ���ִ���д����FindObjectOfType��ʱ����FindObjectsByTypeȡ��
                    Item[] existingItems = Object.FindObjectsByType<Item>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                    foreach (var item in existingItems)
                    {
                        Destroy(item.gameObject);
                    }
                    //�ؽ�
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


