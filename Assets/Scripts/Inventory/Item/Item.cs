using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Farm.CropPlant;

namespace Farm.Inventory {

    public class Item : MonoBehaviour
    {
        public int itemID;
        private SpriteRenderer spriteRenderer;
        private BoxCollider2D coll;
        public  ItemDetails itemDetails;
        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            coll = GetComponent<BoxCollider2D>();

        }
        private void Start()
        {
            if (itemID != 0)
            {
                Init(itemID);
            }
        }
        public void Init(int ID)
        {
            itemID = ID;
            //Inventory获取当前数据
            itemDetails = InventoryManager.Instance.GetItemDetails(itemID);
            if (itemDetails != null)
            {
                spriteRenderer.sprite = itemDetails.itemOnWorldSprite !=null ? itemDetails.itemOnWorldSprite:itemDetails.itemIcon;
                //自动修改碰撞体size和偏移offset，符合原始图
                Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
                coll.size = newSize;
                coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
            }

            if(itemDetails.itemType == ItemType.ReapableScenery)
            {
                //如果是可收割的，ReapableScenery物品挂上ReapItem脚本
                gameObject.AddComponent<ReapItem>();
                gameObject.GetComponent<ReapItem>().InitCropData(itemDetails.itemID);
                gameObject.AddComponent<ItemInteractive>();
            }
        }
    }
}