using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;

    private Season gameSeason = Season.春天;
    private int monthInSeason = 3;
    public bool gameClockPause;//时间暂停
    private float tikTime;

    public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);

    protected override void Awake()
    {
        base.Awake();
        NewGameTime();
    }
    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }
    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;

    }

    private void OnAfterSceneLoadedEvent()
    {
        gameClockPause = false; //场景加载之后，重新开启时间

    }

    private void OnBeforeSceneUnloadEvent()
    {
        gameClockPause = true; //场景在卸载之前，先暂停时间
    }

    private void Start()
    {
        //为了能更新日期，需要在Awake和OnEnable执行
        //Awake——>OnEnable–>Start——>(FixedUpdate——>Update——>LateUpdate)——>OnGUI——>OnDisable——>OnDestroy
        //Event C# 中实现发布 - 订阅模式的语言特性 ,类似订阅+监听的模式
        EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour,gameDay,gameSeason);
    }
    private void Update()
    {
        if (!gameClockPause)
        {
            tikTime += Time.deltaTime;
            if (tikTime >= Settings.secondThreshold)
            {
                //每次达到秒的阈值，就重新去掉阈值，重新开始计时
                tikTime -= Settings.secondThreshold;
                UpdateGameTime();
            }
        }
        //T快速跳过时间，作为时间调试
        if (Input.GetKey(KeyCode.T))
        {
            for(int i = 0; i < 60; i++)
            {
                UpdateGameTime();
            }
        }
        //G快速加天数
        if (Input.GetKeyDown(KeyCode.G))
        {
            gameDay++;
            EventHandler.CallGameDayEvent(gameDay, gameSeason);
            EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        }
    }
    private void NewGameTime()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 7;
        gameDay = 1;
        gameMonth = 1;
        gameYear = 2025;
        gameSeason = Season.春天;
    }
    private void UpdateGameTime()
    {
        gameSecond++;
        if (gameSecond > Settings.secondHold)
        {
            gameMinute++;
            gameSecond = 0;
            if (gameMinute > Settings.minuteHold)
            {
                gameHour++;
                gameMinute = 0;
                if (gameHour > Settings.hourHold)
                {
                    gameDay++;
                    gameHour = 0;
                    if (gameDay > Settings.dayHold)
                    {
                        gameDay = 1;
                        gameMonth++;
                        if (gameMonth > 12)
                            gameMonth = 1;

                        monthInSeason--;
                        if (monthInSeason == 0)
                        {
                            monthInSeason = 3;
                            int seasonNumber = (int)gameSeason;
                            seasonNumber++;

                            if (seasonNumber > Settings.seasonHold)
                            {
                                seasonNumber = 0;
                                gameYear++;
                            }
                            gameSeason = (Season)seasonNumber;
                            if (gameYear > 9999)
                            {
                                gameYear = 2025;
                            }
                        }
                        //对每天的变化，需要刷新地图：用来刷新地图和农作物生长
                        EventHandler.CallGameDayEvent(gameDay, gameSeason);
                    }
                }
                //日期变化
                EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
            }
            //秒的变化
            EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
        }
        //Debug.Log("Second:" + gameSecond + "Minute:" + gameMinute);
    }
}
