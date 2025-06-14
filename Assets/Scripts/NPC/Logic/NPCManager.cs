using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    public SceneRouteDataList_SO sceneRouteDate;
    public List<NPCPosition> npcPositionList;

    private Dictionary<string, SceneRoute> sceneRouteDict = new Dictionary<string, SceneRoute>();

    //����ģʽ�µ�Awake��Ҫʹ�� protected override
    protected override void Awake()
    {
        base.Awake();
        InitSceneRouteDict();
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
    /// �����������֮���·��
    /// </summary>
    /// <param name="fromSceneName">��ʼ����</param>
    /// <param name="gotoSceneName">Ŀ�곡��</param>
    /// <returns></returns>
    public SceneRoute GetSceneRoute(string fromSceneName,string gotoSceneName)
    {
        return sceneRouteDict[fromSceneName + gotoSceneName];
    }
}
