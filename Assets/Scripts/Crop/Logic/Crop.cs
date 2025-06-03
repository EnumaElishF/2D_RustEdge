using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    //执行收割的所有的逻辑
    public CropDetails cropDetails;
    private TileDetails tileDetails;
    private int harvestActionCount;

    private Animator anim;
    private Transform PlayerTransform = FindAnyObjectByType<Player>().transform;


    public void ProcessToolAction(ItemDetails tool,TileDetails tile)
    {
        tileDetails = tile;

        //工具使用次数
        int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemID);   
        if (requireActionCount == -1) return;

        anim = GetComponentInChildren<Animator>();

        //点击计数器
        if (harvestActionCount < requireActionCount)
        {
            harvestActionCount++;
            //判断是否有动画 树木
            if(anim != null && cropDetails.hasAnimation)
            {
                if (PlayerTransform.position.x < transform.position.x)
                {
                    //小于代表人在树左侧
                    anim.SetTrigger("RotateRight");
                }
                else
                {
                    anim.SetTrigger("RotateLeft");
                }
            }
            //播放粒子效果
            //播放声音
        }

        if (harvestActionCount>= requireActionCount)
        {
            if (cropDetails.generateAtPlayerPosition)
            {
                //生成农作物
                SpawnHarvestItems();

            }
            else if (cropDetails.hasAnimation)  //有单独的动画，还不勾选generateAtPlayerPosition的
            {

            }
        }
    }

    public void SpawnHarvestItems()
    {
        for(int i = 0; i < cropDetails.producedItemID.Length; i++)
        {
            int amountToProduce;
            if (cropDetails.producedMinAmount[i] == cropDetails.producedMaxAmount[i])
            {
                //代表只生成指定数量
                amountToProduce = cropDetails.producedMinAmount[i];
            }
            else //物品随机数量
            {
                amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i]+1);
            }

            //执行生成指定数量的物品
            for(int j = 0; j < amountToProduce; j++)
            {
                if (cropDetails.generateAtPlayerPosition)
                {
                    EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                }
                else //世界地图上生成物品
                {

                }
            }

        }
        if (tileDetails != null)
        {
            tileDetails.daysSinceLastHarvest++;

            //作物是否可以重复生长,重生
            if(cropDetails.daysToRegrow>0 && tileDetails.daysSinceLastHarvest < cropDetails.regrowTimes)
            {
                tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.daysToRegrow;
                //刷新种子
                EventHandler.CallRefreshCurrentMap();
            }
            else //不可以重复生长
            {
                //种子拔出
                tileDetails.daysSinceLastHarvest = -1;
                tileDetails.seedItemID = -1;
                
            }
            Destroy(gameObject);
        }
    }
}
