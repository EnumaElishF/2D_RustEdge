using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.CropPlant
{
    /// <summary>
    /// ����ģʽȷ��������Ϸ��ֻ��һ��CropManagerʵ���������κνű�������ͨ��CropManager.Instance���ɷ�������
    /// �������ͨ��FindObjectOfType���ֶ����ò��ҹ������Ŀ���������˴���Ч�ʡ�
    /// </summary>
    public class CropManager : Singleton<CropManager> //�ĳɵ���ģʽ��������������
    {
        //
        public CropDataList_SO cropData;
        private Transform cropParent;
        private Grid currentGrid;
        private Season currentSeason;



        private void OnEnable()
        {
            //��CropManagerע�ᣬע����� ֲ������¼��� Ȼ����ʹ�õ�GridMapManager�ϻ��������
            EventHandler.PlantSeedEvent += OnPlantSeedEvent;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
        }
        private void OnDisable()
        {
            //ֹͣע��
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
            //����currentCrop��Ҫ�еģ������ǿ���ֲ�ģ����ص���Ƭ��û�б��ֶ�����
            if (currentCrop != null && SeasonAvailable(currentCrop) && tileDetails.seedItemID == -1) //���ڵ�һ����ֲ
            {
                tileDetails.seedItemID = ID;
                tileDetails.growthDays = 0;
                //��ʾũ����
                //Debug.Log("��ʾũ����ǰ ͨ��" + tileDetails.seedItemID);

                DisplayCropPlant(tileDetails, currentCrop);
            }
            else if (tileDetails.seedItemID != -1) //��ǰ�ֹ�������ˢ����
            {
                //ˢ�³���������ԭ����������ũ����ģ�����Ҫ�����������
                //��ʾũ����
                DisplayCropPlant(tileDetails, currentCrop);
            }

        }

        /// <summary>
        /// ��ʾũ����
        /// </summary>
        /// <param name="tileDetails">��Ƭ��ͼ��Ϣ</param>
        /// <param name="cropDetails">������Ϣ</param>
        private void DisplayCropPlant(TileDetails tileDetails, CropDetails cropDetails)
        {
            //�ɳ��׶�
            int growthStages = cropDetails.growthDays.Length;
            int currentStage = 0;
            int dayCounter = cropDetails.TotalGrowthDays;
            //������㵱ǰ�ĳɳ��׶�
            for (int i = growthStages - 1; i >= 0; i--)
            {
                if (tileDetails.growthDays >= dayCounter)
                {
                    currentStage = i;
                    break;
                }
                dayCounter -= cropDetails.growthDays[i];
            }
            //��ȡ��ǰ�׶�Prefab
            GameObject cropPrefab = cropDetails.growthPrefabs[currentStage];
            Sprite cropSprite = cropDetails.growthSprites[currentStage];

            //��Ʒ�������ͼ�ϣ���Ʒ������һ����������м䣬��Ҫ����0.5f
            Vector3 pos = new Vector3(tileDetails.gridX + 0.5f, tileDetails.gridY + 0.5f, 0);

            GameObject cropInstance = Instantiate(cropPrefab, pos, Quaternion.identity, cropParent);

            //Ϊ���ɵ�ũ�������ͼƬ
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;

            cropInstance.GetComponent<Crop>().cropDetails = cropDetails;

            cropInstance.GetComponent<Crop>().tileDetails = tileDetails;
        }
        /// <summary>
        /// ͨ����ƷID����������Ϣ
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public CropDetails GetCropDetails(int ID)
        {
            return cropData.cropDetailsList.Find(c => c.seedItemID == ID);
        }
        /// <summary>
        /// �жϵ�ǰ�����Ƿ������ֲ
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
