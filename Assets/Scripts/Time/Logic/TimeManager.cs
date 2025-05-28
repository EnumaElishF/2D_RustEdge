using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;

    private Season gameSeason = Season.����;
    private int monthInSeason = 3;
    public bool gameClockPause;//ʱ����ͣ
    private float tikTime;

    private void Awake()
    {
        NewGameTime();
    }
    private void Start()
    {
        //Ϊ���ܸ������ڣ���Ҫ��Awake��OnEnableִ��
        //Awake����>OnEnable�C>Start����>(FixedUpdate����>Update����>LateUpdate)����>OnGUI����>OnDisable����>OnDestroy
        //Event C# ��ʵ�ַ��� - ����ģʽ���������� ,���ƶ���+������ģʽ
        EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour);
    }
    private void Update()
    {
        if (!gameClockPause)
        {
            tikTime += Time.deltaTime;
            if (tikTime >= Settings.secondThreshold)
            {
                //ÿ�δﵽ�����ֵ��������ȥ����ֵ�����¿�ʼ��ʱ
                tikTime -= Settings.secondThreshold;
                UpdateGameTime();
            }
        }
        //T��������ʱ�䣬��Ϊʱ�����
        if (Input.GetKey(KeyCode.T))
        {
            for(int i = 0; i < 60; i++)
            {
                UpdateGameTime();
            }
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
        gameSeason = Season.����;
    }
    private void UpdateGameTime()
    {
        gameSecond++;
        if (gameSecond > Settings.seasonHold)
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
                    }
                }
                //���ڱ仯
                EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
            }
            //��ı仯
            EventHandler.CallGameMinuteEvent(gameMinute, gameHour);
        }
        //Debug.Log("Second:" + gameSecond + "Minute:" + gameMinute);
    }
}
