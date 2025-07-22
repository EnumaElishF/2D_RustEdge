using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeatherDataList_SO", menuName = "Weather/WeatherDataList")]
public class WeatherDataList_SO : ScriptableObject
{
    public List<WeatherDetails> weatherDetailsList;
    //public SoundDetails GetSoundDetails(SoundName name)
    //{
    //    return soundDetailsList.Find(s => s.soundName == name);
    //}
}

//对应的数据的类型
[System.Serializable]
public class WeatherDetails : IComparable<ScheduleDetails>
{
    
    public Weather weather;//默认情况天气为None
    public SoundName soundName;//播放天气音乐，替代主音乐
    [Header("天气开始时间")]
    public int hourStart;
    public int minuteStart;
    public int dayStart;
    [Header("天气结束时间")]
    public int hourEnd;
    public int minuteEnd;
    public int dayEnd; // 到达结束时间，天气粒子效果降低，缓慢消失
    [Space(10)]
    public int priority; //优先级越小，越先执行
    public Season season;
    public string currentScene; 
    public bool inHouseScene;

    public WeatherDetails(Weather weather, SoundName soundName, int hourStart, int minuteStart, int dayStart, int hourEnd, int minuteEnd, int dayEnd, int priority, Season season, string currentScene,bool inHouseScene)
    {
        this.weather = weather;
        this.soundName = soundName;
        this.hourStart = hourStart;
        this.minuteStart = minuteStart;
        this.dayStart = dayStart;
        this.hourEnd = hourEnd;
        this.minuteEnd = minuteEnd;
        this.dayEnd = dayEnd;
        this.priority = priority;
        this.season = season;
        this.currentScene = currentScene;
        this.inHouseScene = inHouseScene;

    }
    public int TimeStart => (hourStart * 100) + minuteStart;
    public int CompareTo(ScheduleDetails other)
    {
        if (TimeStart == other.Time)
        {
            if (priority > other.priority)
                return 1;
            else
                return -1;

        }
        else if (TimeStart > other.Time)
        {
            return 1;
        }
        else if (TimeStart < other.Time)
        {
            return -1;
        }
        return 0;
    }
}