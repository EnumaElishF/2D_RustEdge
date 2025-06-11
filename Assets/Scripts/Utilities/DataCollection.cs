using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//为了让这些自己写的class都能被Unity识别，需要序列化
[System.Serializable]
public class ItemDetails
{
    public int itemID;
    public string itemName;
    public ItemType itemType;
    public Sprite itemIcon; //物品图片
    public Sprite itemOnWorldSprite; //物品在世界地图上的图片（作为地图指引）
    public string itemDescription;
    public int itemUseRadius; 
    public bool canPickedup;
    public bool canDropped;
    public bool canCarried;
    public int itemPrice;
    [Range(0, 1)]
    public float sellPercentage;
}

//使用struct，也可以用class，但是使用struct更轻量，且能传值
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
/// 序列化坐标：以JSON方式存储
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
    //因为是2D瓦片地图的，所以要使用整型的V2
    public Vector2Int ToVectorInt()
    {
        return new Vector2Int((int)x, (int)y);
    }
}
/// <summary>
/// 存储场景物品的类  ：ID，坐标
/// </summary>
[System.Serializable]
public class SceneItem
{
    public int itemID;
    public SerializableVector3 position;
}
/// <summary>
/// 瓦片格子属性
/// </summary>
[System.Serializable]
public class TileProperty
{
    public Vector2Int tileCoordinate;
    public GridType gridType;
    public bool boolTypeValue;

}
/// <summary>
/// 瓦片信息
/// </summary>
[System.Serializable]
public class TileDetails
{
    public int gridX, gridY;
    public bool canDig;
    public bool canDropItem;
    public bool canPlaceFurniture;
    public bool isNPCObstacle;

    public int daySinceDug = -1; //被挖坑后经过的多少天
    public int daySinceWatered = -1; //被浇水后经过的多少天
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