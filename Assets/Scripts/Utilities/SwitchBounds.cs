using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SwitchBounds : MonoBehaviour
{
    ////TODO: 切换场景后更改调用 :OnEnable开启事件，作为场景切换
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
        //可以通过代码利用标签获取到，另外一个场景的游戏对象的Bounds
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();

        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = confinerShape;
        confiner.InvalidatePathCache();
    }
}
