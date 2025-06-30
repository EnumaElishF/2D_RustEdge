using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    public SceneRouteDataList_SO sceneRouteDate;
    public List<NPCPosition> npcPositionList;

    private Dictionary<string, SceneRoute> sceneRouteDict = new Dictionary<string, SceneRoute>();

    //单例模式下的Awake需要使用 protected override
    protected override void Awake()
    {
        base.Awake();
        InitSceneRouteDict();
    }
    private void OnEnable()
    {
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;

    }
    private void OnDisable()
    {
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;

    }
    //需要在新游戏开始时重置的数据
    private void OnStartNewGameEvent(int obj)
    {
        foreach(var character in npcPositionList)
        {
            //每次新游戏的时候，每一个NPC都去到他开始的位置，以及开始的场景
            character.npc.position = character.position;
            character.npc.GetComponent<NPCMovement>().currentScene = character.startScene;
        }
    }

    private void InitSceneRouteDict()
    {
        if(sceneRouteDate.sceneRouteList.Count > 0)
        {
            foreach(SceneRoute route in sceneRouteDate.sceneRouteList)
            {
                var key = route.fromSceneName + route.gotoSceneName;
                if (sceneRouteDict.ContainsKey(key))
                    continue;
                else
                    sceneRouteDict.Add(key, route);
            }
        }
    }
    /// <summary>
    /// 获得两个场景之间的路径
    /// </summary>
    /// <param name="fromSceneName">起始场景</param>
    /// <param name="gotoSceneName">目标场景</param>
    /// <returns></returns>
    public SceneRoute GetSceneRoute(string fromSceneName,string gotoSceneName)
    {
        return sceneRouteDict[fromSceneName + gotoSceneName];
    }
}
