using Farm.Save;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>,ISaveable
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;

    private Season gameSeason = Season.春天;
    private int monthInSeason = 3;
    public bool gameClockPause;//时间暂停
    private float tikTime;

    //灯光时间差
    private float timeDifference;

    public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);

    public string GUID => GetComponent<DataGUID>().guid;


    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;

        EventHandler.StartNewGameEvent += OnStartNewGameEvent;

    }
    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;

        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;

    }



    private void Start()
    {
        //ISaveable进行注册
        ISaveable saveable = this;
        saveable.RegisterSaveable();

        gameClockPause = true;

        //为了能更新日期，需要在Awake和OnEnable执行
        //Awake——>OnEnable–>Start——>(FixedUpdate——>Update——>LateUpdate)——>OnGUI——>OnDisable——>OnDestroy
        //Event C# 中实现发布 - 订阅模式的语言特性 ,类似订阅+监听的模式

        //EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        //EventHandler.CallGameMinuteEvent(gameMinute, gameHour,gameDay,gameSeason);

        ////切换灯光
        //EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
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

    //需要在新游戏开始时重置的数据
    private void OnStartNewGameEvent(int obj)
    {
        //游戏时间重置
        NewGameTime();

        gameClockPause = false;

    }

    private void OnUpdateGameStateEvent(GameState gameState)
    {
        //执行是不是要暂停,如果gameState == GameState.Pause那么true，时钟暂停
        gameClockPause = gameState == GameState.Pause;
    }

    private void OnAfterSceneLoadedEvent()
    {
        gameClockPause = false; //场景加载之后，重新开启时间

        EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
        //切换灯光
        EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);

    }

    private void OnBeforeSceneUnloadEvent()
    {
        gameClockPause = true; //场景在卸载之前，先暂停时间
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
            //切换灯光
            EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
        }
        //Debug.Log("Second:" + gameSecond + "Minute:" + gameMinute);
    }
    /// <summary>
    /// 返回lightShift，同时计算时间差
    /// </summary>
    /// <returns></returns>
    private LightShift GetCurrentLightShift()
    {
        if(GameTime >= Settings.morningTime && GameTime < Settings.nightTime)
        {
            timeDifference = (float)(GameTime - Settings.morningTime).TotalMinutes;
            return LightShift.Morning;
        }
        if(GameTime<Settings.morningTime || GameTime>= Settings.nightTime)
        {
            timeDifference = Mathf.Abs((float)(GameTime - Settings.nightTime).TotalMinutes);
            //Debug.Log(timeDifference);
            return LightShift.Night;
        }
        return LightShift.Morning;
    }

    /// <summary>
    /// 存储数据: 实现自ISaveable接口的 GenerateSaveData
    /// </summary>
    /// <returns></returns>
    public GameSaveData GenerateSaveData()
    {
        //把TimeManager中数据，逐一添加到整型的字典中
        GameSaveData saveData = new GameSaveData();
        saveData.timeDict = new Dictionary<string, int>();
        saveData.timeDict.Add("gameYear", gameYear);
        saveData.timeDict.Add("gameSeason", (int)gameSeason);
        saveData.timeDict.Add("gameMonth", gameMonth);
        saveData.timeDict.Add("gameDay", gameDay);
        saveData.timeDict.Add("gameHour", gameHour);
        saveData.timeDict.Add("gameMinute", gameMinute);
        saveData.timeDict.Add("gameSecond", gameSecond);

        return saveData;
    }
    /// <summary>
    /// 生成恢复数据：实现自ISaveable接口的 RestoreData
    /// </summary>
    /// <param name="saveData"></param>
    public void RestoreData(GameSaveData saveData)
    {
        //怎么存进去的，就怎么拿出来
        gameYear = saveData.timeDict["gameYear"];
        gameSeason = (Season)saveData.timeDict["gameSeason"];
        gameMonth = saveData.timeDict["gameMonth"];
        gameDay = saveData.timeDict["gameDay"];
        gameHour = saveData.timeDict["gameHour"];
        gameMinute = saveData.timeDict["gameMinute"];
        gameSecond = saveData.timeDict["gameSecond"];

        //看OnAfterSceneLoadedEvent在我们数据做完后，进行了调用时间，灯光的更新变化(刷新)
    }
}
