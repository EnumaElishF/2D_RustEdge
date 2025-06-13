using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    //ע���¼�
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    //�����¼�
    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        UpdateInventoryUI?.Invoke(location, list);
    }

    /// <summary>
    /// ��ק���ڳ���������
    /// </summary>
    public static event Action<int, Vector3> InstantiateItemInScene;
    public static void CallInstantiateItemInScene(int ID,Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(ID, pos);
    }
    /// <summary>
    /// �Ӷ����¼�
    /// </summary>
    public static event Action<int, Vector3,ItemType> DropItemEvent;
    public static void CallDropItemEvent(int ID,Vector3 pos, ItemType itemType)
    {
        DropItemEvent?.Invoke(ID, pos, itemType);
    }

    /// <summary>
    /// ֪ͨ��Ʒ��ѡ�е�״̬����Ϣ
    /// </summary>
    public static event Action<ItemDetails, bool> ItemSelectedEvent;
    public static void CallItemSelectedEvent(ItemDetails itemDetails,bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails, isSelected);
    }
    /// <summary>
    /// ʱ����
    /// </summary>
    public static event Action<int, int,int, Season> GameMinuteEvent;
    public static void CallGameMinuteEvent(int minute,int hour,int day, Season season)
    {
        GameMinuteEvent?.Invoke(minute, hour,day, season);
    }
    /// <summary>
    /// ÿ�ռ�¼
    /// </summary>
    public static event Action<int, Season> GameDayEvent;
    public static void CallGameDayEvent(int day,Season season)
    {
        GameDayEvent?.Invoke(day, season);   
    }

    /// <summary>
    /// ʱ��ͼ��ڣ�Сʱ���գ��£��꣬����
    /// </summary>
    public static event Action<int,int, int, int, Season> GameDateEvent;
    public static void CallGameDateEvent(int hour,int day,int month,int year,Season season)
    {
        GameDateEvent?.Invoke(hour, day, month, year, season);
    }
    /// <summary>
    /// �����л�
    /// </summary>
    public static event Action<string, Vector3> TransitionEvent;
    public static void CallTransitionEvent(string sceneName,Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
    }
    /// <summary>
    /// ����ж��ǰ��Ҫִ�е�Event���������ݣ��洢����ȣ�
    /// </summary>
    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }
    /// <summary>
    /// ����ж�غ���Ҫִ�е�Event���������ݣ��洢����ȣ�
    /// </summary>
    public static event Action AfterSceneLoadedEvent;
    public static void CallAfterSceneLoadedEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
    }
    /// <summary>
    /// ��ɫ����ת��
    /// </summary>
    public static event Action<Vector3> MoveToPosition;
    public static void CallMoveToPosition(Vector3 targetPosition)
    {
        MoveToPosition?.Invoke(targetPosition);
    }

    /// <summary>
    /// ������¼����������+itemDetails
    /// </summary>
    public static event Action<Vector3, ItemDetails> MouseClickedEvent;
    public static void CallMouseClickedEvent(Vector3 pos,ItemDetails itemDetails)
    {
        MouseClickedEvent?.Invoke(pos, itemDetails);
    }
    /// <summary>
    /// ִ��,ʵ�ʹ��߻���Ʒ�Ĺ���,(����Ʒ��---Player����һ������ִ�У�Ȼ��Ż��ӳ���Ʒ������������Ʒ
    /// </summary>
    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimation;
    public static void CallExecuteActionAfterAnimation(Vector3 pos,ItemDetails itemDetails)
    {
        ExecuteActionAfterAnimation?.Invoke(pos, itemDetails);
    }


    /// <summary>
    /// ���µ�ǰ������ũ����
    /// </summary>
    public static event Action<int, TileDetails> PlantSeedEvent;
    public static void CallPlantSeedEvent(int ID,TileDetails tile)
    {
        PlantSeedEvent?.Invoke(ID, tile);
    }
    /// <summary>
    /// �ո��ʵ
    /// </summary>
    public static event Action<int> HarvestAtPlayerPosition;
    public static void CallHarvestAtPlayerPosition(int ID)
    {
        HarvestAtPlayerPosition?.Invoke(ID);
    }

    /// <summary>
    /// ˢ�� ����ĵ�ǰ��Ƭ
    /// </summary>
    public static event Action RefreshCurrentMap;
    public static void CallRefreshCurrentMap()
    {
        RefreshCurrentMap?.Invoke();
    }
    /// <summary>
    /// ����������Ч
    /// </summary>
    public static event Action<ParticalEffectType,Vector3> ParticleEffectEvent;
    public static void CalllParticleEffectEvent(ParticalEffectType effectType,Vector3 pos)
    {
        ParticleEffectEvent?.Invoke(effectType, pos);
    }

    /// <summary>
    /// ������Crop����
    /// </summary>
    public static event Action GenerateCropEvent;
    public static void CallGenerateCropEvent()
    {
        GenerateCropEvent?.Invoke();
    }

}
