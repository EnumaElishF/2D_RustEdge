using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Farm.CropPlant
{
    public class ReapItem : MonoBehaviour
    {
        private CropDetails cropDetails;

        private Transform PlayerTransform => FindAnyObjectByType<Player>().transform;

        public void InitCropData(int ID)
        {
            cropDetails = CropManager.Instance.GetCropDetails(ID);
        }

        /// <summary>
        /// 生成果实
        /// </summary>
        public void SpawnHarvestItems()
        {
            for (int i = 0; i < cropDetails.producedItemID.Length; i++)
            {
                int amountToProduce;
                if (cropDetails.producedMinAmount[i] == cropDetails.producedMaxAmount[i])
                {
                    //代表只生成指定数量
                    amountToProduce = cropDetails.producedMinAmount[i];
                }
                else //物品随机数量
                {
                    amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i] + 1);
                }

                //执行生成指定数量的物品
                for (int j = 0; j < amountToProduce; j++)
                {
                    if (cropDetails.generateAtPlayerPosition)
                    {
                        EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                    }
                    else //世界地图上生成物品
                    {
                        //判断应该生成的物品的方向
                        var dirX = transform.position.x > PlayerTransform.position.x ? 1 : -1;
                        //一定范围内的随机 :因为这是在for循环里生成，所以每次的spawnPos作为随机的
                        var spawnPos = new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX),
                        transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y), 0);

                        EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i], spawnPos);
                    }
                }

            }
            //if (tileDetails != null)
            //{
            //    tileDetails.daysSinceLastHarvest++;

            //    //作物是否可以重复生长,重生
            //    if (cropDetails.daysToRegrow > 0 && tileDetails.daysSinceLastHarvest < cropDetails.regrowTimes)
            //    {
            //        tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.daysToRegrow;
            //        //刷新种子
            //        EventHandler.CallRefreshCurrentMap();
            //    }
            //    else //不可以重复生长
            //    {
            //        //种子拔出
            //        tileDetails.daysSinceLastHarvest = -1;
            //        tileDetails.seedItemID = -1;

            //    }
            //    Destroy(gameObject);
            //}
        }
    }


}
