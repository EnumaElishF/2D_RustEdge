using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    /// <summary>
    /// ��Settings�������Ǿ���ʹ�õĲ�����������ã��������źܶೣ���ͱ�����Ȼ���Լ��޸�����Ͷ�Ӧ�޸���ֵ
    /// </summary>
    public const float itemFadeDuration = 0.35f;
    public const float targetAlpha = 0.45f;

    //ʱ�����
    public const float secondThreshold = 0.01f;    //��ֵԽС��ʱ��Խ��
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    //dayHold����һ�����ж����죬����������ʱ����30��һ����
    public const int dayHold = 30;
    public const int seasonHold = 3;

    //Transition,���ɳ���
    public const float fadeDuration = 1.5f;
}
