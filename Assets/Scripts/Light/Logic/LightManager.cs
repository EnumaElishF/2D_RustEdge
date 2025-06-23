using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private LightControl[] sceneLights;
    private LightShift currentLightShift;
    private Season currentSeason;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        // FindObjectsOfType被FindObjectsByType取代，以后需要使用新版
        sceneLights = FindObjectsByType<LightControl>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach(LightControl light in sceneLights)
        {
            //lightControl 改变灯光的方法
        }
    }
}
