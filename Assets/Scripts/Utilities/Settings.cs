using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    /// <summary>
    /// 在Settings里存放我们经常使用的参数，方便调用：在这里存放很多常量和变量，然后自己修改这里就对应修改数值
    /// </summary>
    public const float itemFadeDuration = 0.35f;
    public const float targetAlpha = 0.45f;

    //时间相关
    public const float secondThreshold = 0.01f;    //数值越小，时间越快
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    //dayHold控制一个月有多少天，我们这里暂时设置30天一个月
    public const int dayHold = 30;
    public const int seasonHold = 3;

    //Transition,过渡场景
    public const float fadeDuration = 1.5f;
}
