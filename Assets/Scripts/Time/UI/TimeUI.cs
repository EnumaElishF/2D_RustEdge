using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TimeUI : MonoBehaviour
{
    public RectTransform dayNightImage;
    public RectTransform clockParent;
    public Image seasonImage;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;

    public Sprite[] seasonSprites;

    private List<GameObject> clockBlocks = new List<GameObject>();

    private void Awake()
    {
        for(int i = 0; i < clockParent.childCount; i++)
        {
            clockBlocks.Add(clockParent.GetChild(i).gameObject);
            clockParent.GetChild(i).gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
        EventHandler.GameDateEvent += OnGameDateEvent;
    }

    private void OnDisable()
    {
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
        EventHandler.GameDateEvent -= OnGameDateEvent;
    }
    private void OnGameMinuteEvent(int minute, int hour)
    {
        timeText.text = hour.ToString("00") +":"+ minute.ToString("00");
    }
    private void OnGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        dateText.text = year + "年" + month.ToString("00") + "月" + day.ToString("00") + "日";
        seasonImage.sprite = seasonSprites[(int)season]; //枚举类转int，0春天，1夏天....

        SwitchHourImage(hour);

        DayNightImageRotate(hour);
    }
    /// <summary>
    ///  根据小时切换时间块显示
    ///      //按照ui的设计，有6格时间格子，作为24小时，每格代表4小时
    /// </summary>
    /// <param name="hour"></param>

    private void SwitchHourImage(int hour)
    {
        int index = hour / 4;

        if(index == 0)
        {
            foreach(var item in clockBlocks)
            {
                item.SetActive(false);
            }
        }
        else
        {
            for(int i = 0; i < clockBlocks.Count; i++)
            {
                //按时间点亮格子
                if (i < index+1)
                    clockBlocks[i].SetActive(true);
                else
                    clockBlocks[i].SetActive(false);
            }
        }
    }
    //日夜的图片旋转
    private void DayNightImageRotate(int hour)
    {
        //每小时对应旋转增加，角度15度;  -90保证从黑夜0点开始按小时开始
        var target = new Vector3(0, 0, hour * 15 - 90);
        dayNightImage.DORotate(target, 1f, RotateMode.Fast);
    }

}
