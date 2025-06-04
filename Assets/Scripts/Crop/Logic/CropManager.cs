using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.CropPlant
{
    /// <summary>
    /// 单例模式确保整个游戏中只有一个CropManager实例，并且任何脚本都可以通过CropManager.Instance轻松访问它。
    /// 这避免了通过FindObjectOfType或手动引用查找管理器的开销，提高了代码效率。
    /// </summary>
    public class CropManager : Singleton<CropManager> //改成单例模式，方便其他调用
    {
        //
        public CropDataList_SO cropData;
        private Transform cropParent;
        private Grid currentGrid;
        private Season currentSeason;



        private void OnEnable()
        {
            //在CropManager注册，注册这个 植物更新事件， 然后在使用的GridMapManager上会调用他。
            EventHandler.PlantSeedEvent += OnPlantSeedEvent;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
        }
        private void OnDisable()
        {
            //停止注册
            EventHandler.PlantSeedEvent -= OnPlantSeedEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;

        }

        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;
        }

        private void OnAfterSceneLoadedEvent()
        {
            currentGrid = FindAnyObjectByType<Grid>();
            cropParent = GameObject.FindWithTag("CropParent").transform;

        }

        private void OnPlantSeedEvent(int ID, TileDetails tileDetails)
        {
            CropDetails currentCrop = GetCropDetails(ID);
            //种子currentCrop是要有的，季节是可种植的，场地的瓦片是没有被种东西的
            if (currentCrop != null && SeasonAvailable(currentCrop) && tileDetails.seedItemID == -1) //用于第一次种植
            {
                tileDetails.seedItemID = ID;
                tileDetails.growthDays = 0;
                //显示农作物
                //Debug.Log("显示农作物前 通过" + tileDetails.seedItemID);

                DisplayCropPlant(tileDetails, currentCrop);
            }
            else if (tileDetails.seedItemID != -1) //以前种过，重新刷出来
            {
                //刷新场景回来，原本各自是有农作物的，所以要重新生成这个
                //显示农作物
                DisplayCropPlant(tileDetails, currentCrop);
            }

        }

        /// <summary>
        /// 显示农作物
        /// </summary>
        /// <param name="tileDetails">瓦片地图信息</param>
        /// <param name="cropDetails">种子信息</param>
        private void DisplayCropPlant(TileDetails tileDetails, CropDetails cropDetails)
        {
            //成长阶段
            int growthStages = cropDetails.growthDays.Length;
            int currentStage = 0;
            int dayCounter = cropDetails.TotalGrowthDays;
            //倒序计算当前的成长阶段
            for (int i = growthStages - 1; i >= 0; i--)
            {
                if (tileDetails.growthDays >= dayCounter)
                {
                    currentStage = i;
                    break;
                }
                dayCounter -= cropDetails.growthDays[i];
            }
            //获取当前阶段Prefab
            GameObject cropPrefab = cropDetails.growthPrefabs[currentStage];
            Sprite cropSprite = cropDetails.growthSprites[currentStage];

            //物品在世界地图上，物品的生成一个在网格的中间，需要加上0.5f
            Vector3 pos = new Vector3(tileDetails.gridX + 0.5f, tileDetails.gridY + 0.5f, 0);

            GameObject cropInstance = Instantiate(cropPrefab, pos, Quaternion.identity, cropParent);

            //为生成的农作物，赋予图片
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;

            cropInstance.GetComponent<Crop>().cropDetails = cropDetails;

            cropInstance.GetComponent<Crop>().tileDetails = tileDetails;
        }
        /// <summary>
        /// 通过物品ID查找种子信息
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public CropDetails GetCropDetails(int ID)
        {
            return cropData.cropDetailsList.Find(c => c.seedItemID == ID);
        }
        /// <summary>
        /// 判断当前季节是否可以种植
        /// </summary>
        /// <param name="crop"></param>
        /// <returns></returns>
        private bool SeasonAvailable(CropDetails crop)
        {
            for (int i = 0; i < crop.seasons.Length; i++)
            {
                if (crop.seasons[i] == currentSeason)
                    return true;
            }
            return false;
        }
    }

}
