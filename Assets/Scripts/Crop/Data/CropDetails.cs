
using UnityEngine;


//[System.Serializable] 如果不加这个序列化，那么在DataSO是看不到这个DetailsList数据的
[System.Serializable]
public class CropDetails
{
    public int seedItemID;
    [Header("不同阶段需要的天数")]
    public int[] growthDays;
    public int TotalGrowthDays
    {
        get
        {
            int amount = 0;
            foreach(var days in growthDays)
            {
                amount += days;
            }
            return amount;
        }
    }
    [Header("不同生长阶段物品Prefab")]
    public GameObject[] growthPrefabs;

    [Header("不同阶段的图片")]
    public Sprite[] growthSprites;

    [Header("可种植的季节")]
    public Season[] seasons;

    [Space]
    [Header("收割工具")]
    public int[] harvestToolItemID;
    [Header("每种工具使用次数")]
    public int[] requireActionCount;

    [Header("转换新物品ID")]
    public int transferItemID;

    [Space]
    [Header("收割果实信息")]
    public int[] producedItemID;
    public int[] producedMinAmount;
    public int[] producedMaxAmount;
    public Vector2 spawnRadius;

    [Header("再次生长时间")]
    public int daysToRegrow;
    public int regrowTimes;

    [Header("Options")]
    public bool generateAtPlayerPosition; //在头顶生成
    public bool hasAnimation;
    public bool hasParticalEffect;
    //TODO: 特效音效 等
    public ParticalEffectType effectType;
    //粒子特效坐标
    public Vector3 effectPos;

    /// <summary>
    /// 检查当前工具是否可以使用
    /// </summary>
    /// <param name="toolID">工具id</param>
    /// <returns></returns>
    public bool CheckToolAvailable(int toolID)
    {
        foreach(var tool in harvestToolItemID)
        {
            if(tool == toolID)
            {
                return true;
            }

        }
        return false;
    }

    /// <summary>
    /// 获取工具需要使用的次数 （写一个根据id查数组里的值)
    /// </summary>
    /// <param name="toolID">工具ID</param>
    /// <returns></returns>
    public int GetTotalRequireCount(int toolID)
    {
        for (int i = 0; i < harvestToolItemID.Length; i++)
        {
            if (harvestToolItemID[i] == toolID)
                return requireActionCount[i];
        }
        return -1;
    }
}
