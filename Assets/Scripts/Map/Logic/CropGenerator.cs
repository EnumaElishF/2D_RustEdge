using Farm.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.CropPlant
{
    public class CropGenerator : MonoBehaviour
    {
        //希望摆在场景里的东西能更新地图的网格，前提是，找到临近的对应的网格，然后在里面生成
        private Grid currentGrid;

        public int seedItemID;
        public int growthDays;

        private void Awake()
        {
            currentGrid = FindAnyObjectByType<Grid>();
            GenerateCrop();
        }
        private void OnEnable()
        {
            //使用EventHandler去控制GenerateCrop的好处，在这里很明显：
            //如果不使用这个Event去集中调用这个功能，那就需要在生成前用观察者模型把每一个当前场景的脚本CropGenerator全拿到，然后
            //场景加载出来之后，再通知他们去生成对应的，种子更新地图的信息GenerateCrop，这样很麻烦
            EventHandler.GenerateCropEvent += GenerateCrop;
        }
        private void OnDisable()
        {
            EventHandler.GenerateCropEvent -= GenerateCrop;

        }

        private void GenerateCrop()
        {
            Vector3Int cropGridPos = currentGrid.WorldToCell(transform.position);

            if(seedItemID != 0)
            {
                var tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(cropGridPos);
                if(tile == null)
                {
                    tile = new TileDetails();
                }
                tile.daySinceWatered = -1;
                tile.seedItemID = seedItemID;
                tile.growthDays = growthDays;

                GridMapManager.Instance.UpdateTileDetails(tile);
            }
        }

    }

}
