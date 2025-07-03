using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimelineManager : Singleton<TimelineManager>
{
    [Header("00_Start开始场景")]
    public PlayableDirector startDirector;
    [SceneName] public string startScene;

    [Header("01_Filed场景")]
    public PlayableDirector fieldDirector;
    [SceneName] public string fieldScene;



    private PlayableDirector currentDirector;

    private bool isDone;
    public bool IsDone { set => isDone = value; }

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

        if (currentScene == startScene)
        {
            currentDirector = startDirector;
            if (currentDirector != null)
                currentDirector.Play();
        }else if (currentScene == fieldScene)
        {
            currentDirector = fieldDirector;
            if (currentDirector != null)
                currentDirector.Play();
        }


    }


    public void PauseTimeline(PlayableDirector director)
    {
        currentDirector = director;
        currentDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
        isPause = true;
    }



}
