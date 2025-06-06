using Farm.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.CropPlant
{
    public class CropGenerator : MonoBehaviour
    {
        //ϣ�����ڳ�����Ķ����ܸ��µ�ͼ������ǰ���ǣ��ҵ��ٽ��Ķ�Ӧ������Ȼ������������
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
            //ʹ��EventHandlerȥ����GenerateCrop�ĺô�������������ԣ�
            //�����ʹ�����Eventȥ���е���������ܣ��Ǿ���Ҫ������ǰ�ù۲���ģ�Ͱ�ÿһ����ǰ�����Ľű�CropGeneratorȫ�õ���Ȼ��
            //�������س���֮����֪ͨ����ȥ���ɶ�Ӧ�ģ����Ӹ��µ�ͼ����ϢGenerateCrop���������鷳
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
