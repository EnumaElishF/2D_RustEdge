using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    //注册事件
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    //调用事件
    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        UpdateInventoryUI?.Invoke(location, list);
    }

    /// <summary>
    /// 拖拽到在场景上生成
    /// </summary>
    public static event Action<int, Vector3> InstantiateItemInScene;
    public static void CallInstantiateItemInScene(int ID,Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(ID, pos);
    }
    /// <summary>
    /// 扔东西事件
    /// </summary>
    public static event Action<int, Vector3,ItemType> DropItemEvent;
    public static void CallDropItemEvent(int ID,Vector3 pos, ItemType itemType)
    {
        DropItemEvent?.Invoke(ID, pos, itemType);
    }

    /// <summary>
    /// 通知物品被选中的状态和信息
    /// </summary>
    public static event Action<ItemDetails, bool> ItemSelectedEvent;
    public static void CallItemSelectedEvent(ItemDetails itemDetails,bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails, isSelected);
    }
    /// <summary>
    /// 时间秒
    /// </summary>
    public static event Action<int, int,int, Season> GameMinuteEvent;
    public static void CallGameMinuteEvent(int minute,int hour,int day, Season season)
    {
        GameMinuteEvent?.Invoke(minute, hour,day, season);
    }
    /// <summary>
    /// 每日记录
    /// </summary>
    public static event Action<int, Season> GameDayEvent;
    public static void CallGameDayEvent(int day,Season season)
    {
        GameDayEvent?.Invoke(day, season);   
    }

    /// <summary>
    /// 时间和季节：小时，日，月，年，季节
    /// </summary>
    public static event Action<int,int, int, int, Season> GameDateEvent;
    public static void CallGameDateEvent(int hour,int day,int month,int year,Season season)
    {
        GameDateEvent?.Invoke(hour, day, month, year, season);
    }
    /// <summary>
    /// 场景切换
    /// </summary>
    public static event Action<string, Vector3> TransitionEvent;
    public static void CallTransitionEvent(string sceneName,Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
    }
    /// <summary>
    /// 场景卸载前需要执行的Event（保存数据，存储坐标等）
    /// </summary>
    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }
    /// <summary>
    /// 场景卸载后需要执行的Event（保存数据，存储坐标等）
    /// </summary>
    public static event Action AfterSceneLoadedEvent;
    public static void CallAfterSceneLoadedEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
    }
    /// <summary>
    /// 角色坐标转移
    /// </summary>
    public static event Action<Vector3> MoveToPosition;
    public static void CallMoveToPosition(Vector3 targetPosition)
    {
        MoveToPosition?.Invoke(targetPosition);
    }

    /// <summary>
    /// 鼠标点击事件，鼠标坐标+itemDetails
    /// </summary>
    public static event Action<Vector3, ItemDetails> MouseClickedEvent;
    public static void CallMouseClickedEvent(Vector3 pos,ItemDetails itemDetails)
    {
        MouseClickedEvent?.Invoke(pos, itemDetails);
    }
    /// <summary>
    /// 执行,实际工具或物品的功能,(扔物品）---Player动画一定是先执行，然后才会扔出物品，地面生成物品
    /// </summary>
    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimation;
    public static void CallExecuteActionAfterAnimation(Vector3 pos,ItemDetails itemDetails)
    {
        ExecuteActionAfterAnimation?.Invoke(pos, itemDetails);
    }


    /// <summary>
    /// 更新当前场景的农作物
    /// </summary>
    public static event Action<int, TileDetails> PlantSeedEvent;
    public static void CallPlantSeedEvent(int ID,TileDetails tile)
    {
        PlantSeedEvent?.Invoke(ID, tile);
    }
    /// <summary>
    /// 收割果实
    /// </summary>
    public static event Action<int> HarvestAtPlayerPosition;
    public static void CallHarvestAtPlayerPosition(int ID)
    {
        HarvestAtPlayerPosition?.Invoke(ID);
    }

    /// <summary>
    /// 刷新 作物的当前瓦片
    /// </summary>
    public static event Action RefreshCurrentMap;
    public static void CallRefreshCurrentMap()
    {
        RefreshCurrentMap?.Invoke();
    }
    /// <summary>
    /// 播放粒子特效
    /// </summary>
    public static event Action<ParticalEffectType,Vector3> ParticleEffectEvent;
    public static void CalllParticleEffectEvent(ParticalEffectType effectType,Vector3 pos)
    {
        ParticleEffectEvent?.Invoke(effectType, pos);
    }

    /// <summary>
    /// 场景的Crop生成
    /// </summary>
    public static event Action GenerateCropEvent;
    public static void CallGenerateCropEvent()
    {
        GenerateCropEvent?.Invoke();
    }

}
