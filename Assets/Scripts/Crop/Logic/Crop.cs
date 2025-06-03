using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    //ִ���ո�����е��߼�
    public CropDetails cropDetails;
    private TileDetails tileDetails;
    private int harvestActionCount;

    private Animator anim;
    private Transform PlayerTransform = FindAnyObjectByType<Player>().transform;


    public void ProcessToolAction(ItemDetails tool,TileDetails tile)
    {
        tileDetails = tile;

        //����ʹ�ô���
        int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemID);   
        if (requireActionCount == -1) return;

        anim = GetComponentInChildren<Animator>();

        //���������
        if (harvestActionCount < requireActionCount)
        {
            harvestActionCount++;
            //�ж��Ƿ��ж��� ��ľ
            if(anim != null && cropDetails.hasAnimation)
            {
                if (PlayerTransform.position.x < transform.position.x)
                {
                    //С�ڴ������������
                    anim.SetTrigger("RotateRight");
                }
                else
                {
                    anim.SetTrigger("RotateLeft");
                }
            }
            //��������Ч��
            //��������
        }

        if (harvestActionCount>= requireActionCount)
        {
            if (cropDetails.generateAtPlayerPosition)
            {
                //����ũ����
                SpawnHarvestItems();

            }
            else if (cropDetails.hasAnimation)  //�е����Ķ�����������ѡgenerateAtPlayerPosition��
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
                if (cropDetails.generateAtPlayerPosition)
                {
                    EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                }
                else //�����ͼ��������Ʒ
                {

                }
            }

        }
        if (tileDetails != null)
        {
            tileDetails.daysSinceLastHarvest++;

            //�����Ƿ�����ظ�����,����
            if(cropDetails.daysToRegrow>0 && tileDetails.daysSinceLastHarvest < cropDetails.regrowTimes)
            {
                tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.daysToRegrow;
                //ˢ������
                EventHandler.CallRefreshCurrentMap();
            }
            else //�������ظ�����
            {
                //���Ӱγ�
                tileDetails.daysSinceLastHarvest = -1;
                tileDetails.seedItemID = -1;
                
            }
            Destroy(gameObject);
        }
    }
}
