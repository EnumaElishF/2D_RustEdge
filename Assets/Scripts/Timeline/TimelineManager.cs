using Farm.Save;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimelineManager : Singleton<TimelineManager>,ISaveable
{
    [Header("00_Start开始场景")]
    public PlayableDirector startDirector;
    [SceneName] public string startScene;
    private int countComeStartScene; //来到此场景的次数

    [Header("01_Filed场景")]
    public PlayableDirector fieldDirector;
    [SceneName] public string fieldScene;
    private int countComeFieldScene;


    private PlayableDirector currentDirector;

    private bool isDone;
    public bool IsDone { set => isDone = value; }

    public string GUID => GetComponent<DataGUID>().guid;

    private bool isPause;
    protected override void Awake()
    {
        base.Awake();
        currentDirector = startDirector;
    }

    private void OnEnable()
    {
        //作为游戏Timeline的Playable使用委托类型Action的played和stopped这里不需要再去退出订阅
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;

        //！取消使用下面的写法
        //startDirector.played += OnPlayed;
        //startDirector.stopped += OnStopped;

    }



    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;

    }

    private void Start()
    {
        //ISaveable进行注册
        ISaveable saveable = this;
        saveable.RegisterSaveable();

    }

    private void Update()
    {
        if(isPause && Input.GetKeyDown(KeyCode.Space) && isDone)
        {
            isPause = false;
            currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
        }
    }


    private void OnAfterSceneLoadedEvent()
    {
        //场景加载之后，如果场景有多个Timeline，那么我需要找到当前场景对应的Timeline
        //currentDirector = FindAnyObjectByType<PlayableDirector>();

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == startScene && countComeStartScene==0)
        {
            currentDirector = startDirector;
            if (currentDirector != null)
                currentDirector.Play();
            countComeStartScene++;
        }
        else if (currentScene == fieldScene && countComeFieldScene==0)
        {
            currentDirector = fieldDirector;
            if (currentDirector != null)
                currentDirector.Play();
            countComeFieldScene++;
        }


    }


    public void PauseTimeline(PlayableDirector director)
    {
        currentDirector = director;
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
        isPause = true;
    }


    /// <summary>
    /// 存储数据: 实现自ISaveable接口的 GenerateSaveData
    /// </summary>
    /// <returns></returns>
    public GameSaveData GenerateSaveData()
    {
        // WORKFLOW: 添加每个场景, 对应动画的加载次数 (用于根据进入场景的不同次数，加入特别的场景动画事件)

        //游戏存档时，拿取这个游戏启动时，走过每个场景的次数
        GameSaveData saveData = new GameSaveData();
        saveData.sceneTimelineCount = new Dictionary<string, int>();
        string currentScene = SceneManager.GetActiveScene().name;

        saveData.sceneTimelineCount.Add(startScene, countComeStartScene);
        saveData.sceneTimelineCount.Add(fieldScene, countComeFieldScene);

        return saveData;
    }
    /// <summary>
    /// 生成恢复数据：实现自ISaveable接口的 RestoreData
    /// </summary>
    /// <param name="saveData"></param>
    public void RestoreData(GameSaveData saveData)
    {
        //怎么存进去的，就怎么拿出来
        countComeStartScene = saveData.sceneTimelineCount[startScene];
        countComeFieldScene = saveData.sceneTimelineCount[startScene];
        
    }

}
