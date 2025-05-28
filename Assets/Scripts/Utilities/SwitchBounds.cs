using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SwitchBounds : MonoBehaviour
{
    ////TODO: �л���������ĵ��� :OnEnable�����¼�����Ϊ�����л�
    //private void Start()
    //{
    //    SwitchConfineShape();
    //}

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += SwitchConfineShape;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= SwitchConfineShape;
    }

    private void SwitchConfineShape()
    {
        //����ͨ���������ñ�ǩ��ȡ��������һ����������Ϸ�����Bounds
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();

        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = confinerShape;
        confiner.InvalidatePathCache();
    }
}
