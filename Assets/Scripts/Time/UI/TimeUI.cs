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
        dateText.text = year + "��" + month.ToString("00") + "��" + day.ToString("00") + "��";
        seasonImage.sprite = seasonSprites[(int)season]; //ö����תint��0���죬1����....

        SwitchHourImage(hour);

        DayNightImageRotate(hour);
    }
    /// <summary>
    ///  ����Сʱ�л�ʱ�����ʾ
    ///      //����ui����ƣ���6��ʱ����ӣ���Ϊ24Сʱ��ÿ�����4Сʱ
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
                //��ʱ���������
                if (i < index+1)
                    clockBlocks[i].SetActive(true);
                else
                    clockBlocks[i].SetActive(false);
            }
        }
    }
    //��ҹ��ͼƬ��ת
    private void DayNightImageRotate(int hour)
    {
        //ÿСʱ��Ӧ��ת���ӣ��Ƕ�15��;  -90��֤�Ӻ�ҹ0�㿪ʼ��Сʱ��ʼ
        var target = new Vector3(0, 0, hour * 15 - 90);
        dayNightImage.DORotate(target, 1f, RotateMode.Fast);
    }

}
