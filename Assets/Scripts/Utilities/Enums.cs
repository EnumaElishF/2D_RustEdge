using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 统一存储枚举变量
/// </summary>
public enum ItemType
{
    Seed,Commodity,Furniture,
    HoeTool,ChopTool,BreakTool,ReapTool,WaterTool,CollectTool,  //多种工具
    ReapableScenery //reap可收割的
}
/// <summary>
/// 插槽类型，包括背包，容器，商店，等
/// </summary>
public enum SlotType
{
    Bag,Box,Shop
}
/// <summary>
/// 库存属于的位置
/// </summary>
public enum InventoryLocation
{
    Player,Box
}
/// <summary>
/// 物体的类型。None是空，什么也没有拿。Carry举起，Hoe工具挖掘，Break击碎,
/// </summary>
public enum PartType
{
    None, Carry, Hoe, Break, Water, Chop, Collect, Reap
}
/// <summary>
/// 身体部分
/// </summary>
public enum PartName
{
    Body,Hair,Arm,Tool
}
/// <summary>
/// 季节
/// </summary>
public enum Season
{
    春天,夏天,秋天,冬天
}
/// <summary>
/// 地图瓦片，网格类型
/// </summary>
public enum GridType
{
    Diggable,DropItem,PlaceFurniture,NPCObstacle
}
/// <summary>
/// 粒子效果类型
/// </summary>
public enum ParticalEffectType
{
    None,LeaveFalling01, LeaveFalling02,Rock,ReapableScenery
}