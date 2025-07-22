using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeatherManager : Singleton<WeatherManager>
{
    [Header("天气控制")]
    public WeatherDataList_SO weatherDataList_SO;
    public GameObject weatherInPlayer;

    private bool inHouseScene = false;
    private bool isSceneLoaded = false; // 场景是否加载完成

    // 新增：记录上一次的状态用于比较
    private Weather lastWeather = Weather.None;
    private bool lastInHouseScene = false;

    Weather currentWeather = Weather.None;//初始天气为None
    SoundName soundName = SoundName.none;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
    }



    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;


    }


    private void OnAfterSceneLoadedEvent()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        bool newInHouseScene = currentScene == "02Home";

        // 场景切换时，根据是否在室内重置当前天气
        if (newInHouseScene)
        {
            currentWeather = Weather.None; // 室内默认无天气
            soundName = SoundName.none;
        }
        else
        {
            // 室外需要重新计算天气（后续由GameMinuteEvent触发）
            currentWeather = Weather.None;
            soundName = SoundName.none;
        }

        inHouseScene = newInHouseScene;
        isSceneLoaded = true;

        // 检查更新
        CheckAndUpdateWeather(currentWeather, soundName);
    }

    private void OnGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        // 空引用保护
        if (weatherDataList_SO == null || weatherDataList_SO.weatherDetailsList == null)
        {
            Debug.LogError("未获取到天气数据weatherDataList_SO:"+ weatherDataList_SO);
            return;
        }

        int currentTime = hour * 100 + minute; // 转换为时分整数（如9:30 → 930）
        currentWeather = Weather.None; // 先默认重置为无天气
        soundName = SoundName.none;


        if (!inHouseScene)
        {
            bool haveWeather = false;
            // 遍历所有天气数据
            foreach (WeatherDetails weatherData in weatherDataList_SO.weatherDetailsList)
            {
                // 1. 检查季节是否匹配
                if (weatherData.season != season)
                    continue;

                // 2. 检查日期是否在开始和结束范围之间（包含首尾）
                if (day < weatherData.dayStart || day > weatherData.dayEnd)
                    continue;

                // 3. 计算开始和结束时间的整数表示
                int startTime = weatherData.hourStart * 100 + weatherData.minuteStart;
                int endTime = weatherData.hourEnd * 100 + weatherData.minuteEnd;

                // 4. 检查当前时间是否在时间范围内
                bool isTimeInRange;
                if (startTime <= endTime)
                {
                    // 正常情况：开始时间 <= 结束时间（如8:00-18:00）
                    isTimeInRange = currentTime >= startTime && currentTime <= endTime;
                }
                else
                {
                    // 跨天情况：开始时间 > 结束时间（如22:00-06:00）
                    isTimeInRange = currentTime >= startTime || currentTime <= endTime;
                }

                if (!isTimeInRange)
                    continue;

                currentWeather = weatherData.weather;
                soundName = weatherData.soundName;

                haveWeather = true;

                //Debug.Log("天气时间得到:当前时间 分钟" + minute + "小时" + hour + "day" + day + "Season" + season + "currentWeather" + currentWeather);

            }

        }

        // 检查是否需要更新天气
        CheckAndUpdateWeather(currentWeather, soundName);

    }


    /// <summary>
    /// 检查并更新天气（仅在状态变化时执行）
    /// </summary>
    private void CheckAndUpdateWeather(Weather checkCurrentWeather = Weather.None, SoundName checkSoundName = SoundName.none)
    {
        // 判断是否需要更新：天气变化 或 屋内屋外状态变化
        bool needUpdate = checkCurrentWeather != lastWeather || inHouseScene != lastInHouseScene;
        Debug.Log("判断是否需要更新needUpdate:" + needUpdate);
        if (needUpdate && isSceneLoaded)
        {
            Debug.Log("天气变化成功"+ checkCurrentWeather);

            UpdateWeatherEffects(checkCurrentWeather);

            if (checkCurrentWeather != Weather.None && !inHouseScene)
            {
                Debug.Log("进入雨天");
                EventHandler.CallWeatherEvent(checkCurrentWeather, checkSoundName, true);
            }
            else
            {
                EventHandler.CallWeatherEvent(Weather.None, SoundName.none, false);
            }

            // 更新记录的状态为当前状态
            lastWeather = currentWeather;
        }
        //无论是否更新，都同步屋内状态（避免残留旧值）
        lastInHouseScene = inHouseScene;
    }

    /// <summary>
    /// 更新天气效果显示状态
    /// </summary>
    private void UpdateWeatherEffects(Weather currentWeather)
    {
        if (weatherInPlayer == null)
        {
            Debug.LogError("未找到weatherInPlayer组件");
        }
        // 遍历所有天气效果子对象
        foreach (Transform child in weatherInPlayer.transform)
        {
            // 检查子对象名称是否与当前天气匹配
            bool isMatch = child.name.Equals(currentWeather.ToString(), System.StringComparison.OrdinalIgnoreCase);

            // 激活匹配的天气效果，禁用不匹配的
            child.gameObject.SetActive(isMatch);
        }
    }

}
