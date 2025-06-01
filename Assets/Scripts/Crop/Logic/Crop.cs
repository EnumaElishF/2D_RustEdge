using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    //执行收割的所有的逻辑
    public CropDetails cropDetails;
    private int harvestActionCount;


    public void ProcessToolAction(ItemDetails tool)
    {
        //工具使用次数
        int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemID);
        if (requireActionCount == -1) return;

        //判断是否有动画 树木

        //点击计数器
        if (harvestActionCount < requireActionCount)
        {
            harvestActionCount++;

            //播放粒子效果
            //播放声音
        }

        if(harvestActionCount>= requireActionCount)
        {
            if (cropDetails.generateAtPlayPosition)
            {
                //生成农作物
                SpawnHarvestItems();

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
                if (cropDetails.generateAtPlayPosition)
                    EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
            }

        }
    }
}
