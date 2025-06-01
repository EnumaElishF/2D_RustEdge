using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    //ִ���ո�����е��߼�
    public CropDetails cropDetails;
    private int harvestActionCount;


    public void ProcessToolAction(ItemDetails tool)
    {
        //����ʹ�ô���
        int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemID);
        if (requireActionCount == -1) return;

        //�ж��Ƿ��ж��� ��ľ

        //���������
        if (harvestActionCount < requireActionCount)
        {
            harvestActionCount++;

            //��������Ч��
            //��������
        }

        if(harvestActionCount>= requireActionCount)
        {
            if (cropDetails.generateAtPlayPosition)
            {
                //����ũ����
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
                //����ֻ����ָ������
                amountToProduce = cropDetails.producedMinAmount[i];
            }
            else //��Ʒ�������
            {
                amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i]+1);
            }

            //ִ������ָ����������Ʒ
            for(int j = 0; j < amountToProduce; j++)
            {
                if (cropDetails.generateAtPlayPosition)
                    EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
            }

        }
    }
}
