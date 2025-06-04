using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ͳһ�洢ö�ٱ���
/// </summary>
public enum ItemType
{
    Seed,Commodity,Furniture,
    HoeTool,ChopTool,BreakTool,ReapTool,WaterTool,CollectTool,  //���ֹ���
    ReapableScenery //reap���ո��
}
/// <summary>
/// ������ͣ������������������̵꣬��
/// </summary>
public enum SlotType
{
    Bag,Box,Shop
}
/// <summary>
/// ������ڵ�λ��
/// </summary>
public enum InventoryLocation
{
    Player,Box
}
/// <summary>
/// ��������͡�None�ǿգ�ʲôҲû���á�Carry����Hoe�����ھ�Break����,
/// </summary>
public enum PartType
{
    None, Carry, Hoe, Break, Water, Chop, Collect, Reap
}
/// <summary>
/// ���岿��
/// </summary>
public enum PartName
{
    Body,Hair,Arm,Tool
}
/// <summary>
/// ����
/// </summary>
public enum Season
{
    ����,����,����,����
}
/// <summary>
/// ��ͼ��Ƭ����������
/// </summary>
public enum GridType
{
    Diggable,DropItem,PlaceFurniture,NPCObstacle
}
/// <summary>
/// ����Ч������
/// </summary>
public enum ParticalEffectType
{
    None,LeaveFalling01, LeaveFalling02,Rock,ReapableScenery
}