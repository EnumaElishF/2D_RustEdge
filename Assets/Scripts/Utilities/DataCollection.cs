using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ϊ������Щ�Լ�д��class���ܱ�Unityʶ����Ҫ���л�
[System.Serializable]
public class ItemDetails
{
    public int itemID;
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon; //��ƷͼƬ
    public Sprite itemOnWorldSprite; //��Ʒ�������ͼ�ϵ�ͼƬ����Ϊ��ͼָ����
    public string itemDescription;
    public int itemUseRadius; 
    public bool canPickedup;
    public bool canDropped;
    public bool canCarried;
    public int itemPrice;
    [Range(0, 1)]
    public float sellPercentage;
}

//ʹ��struct��Ҳ������class������ʹ��struct�����������ܴ�ֵ
[System.Serializable]
public struct InventoryItem
{
    public int itemID;
    public int itemAmount;

}

[System.Serializable]
public class AnimatorType
{
    public PartType partType;
    public PartName partName;
    public AnimatorOverrideController overrideController;
}

/// <summary>
/// ���л����꣺��JSON��ʽ�洢
/// </summary>
[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;
    public SerializableVector3(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;

    }
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
    //��Ϊ��2D��Ƭ��ͼ�ģ�����Ҫʹ�����͵�V2
    public Vector2Int ToVectorInt()
    {
        return new Vector2Int((int)x, (int)y);
    }
}
/// <summary>
/// �洢������Ʒ����  ��ID������
/// </summary>
[System.Serializable]
public class SceneItem
{
    public int itemID;
    public SerializableVector3 position;
}
/// <summary>
/// ��Ƭ��������
/// </summary>
[System.Serializable]
public class TileProperty
{
    public Vector2Int tileCoordinate;
    public GridType gridType;
    public bool boolTypeValue;

}
/// <summary>
/// ��Ƭ��Ϣ
/// </summary>
[System.Serializable]
public class TileDetails
{
    public int gridX, gridY;
    public bool canDig;
    public bool canDropItem;
    public bool canPlaceFurniture;
    public bool isNPCObstacle;

    public int daySinceDug = -1; //���ڿӺ󾭹��Ķ�����
    public int daySinceWatered = -1; //����ˮ�󾭹��Ķ�����
    public int seedItemID = -1;

    public int growthDays = -1;

    public int daysSinceLastHarvest = -1;

}
[System.Serializable]
public class NPCPosition
{
    public Transform npc;
    public string startScene;
    public Vector3 position;

}