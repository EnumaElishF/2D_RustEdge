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

    /// <summary>
    /// ʱ�����
    /// </summary>
    public const float secondThreshold = 0.01f;    //��ֵԽС��ʱ��Խ��
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    /// <summary>
    /// dayHold����һ�����ж����죬����������ʱ����30��һ����
    /// </summary>
    public const int dayHold = 30;
    public const int seasonHold = 3;

    /// <summary>
    /// Transition,���ɳ���
    /// </summary>
    public const float fadeDuration = 1.5f;

    /// <summary>
    /// �����������
    /// </summary>
    public const int reapAmount = 2;

    //NPC�����ƶ�
    public const float gridCellSize = 1;
    public const float gridCellDiagonalSize = 1.41f;  //Diagonalб����
    //���ؾ��� :����Ŀ����������20*20���أ�20*20ռһ��unit��1/20 = 0.05��λ���ؾ���
    public const float pixelSize = 0.05f; 

}
