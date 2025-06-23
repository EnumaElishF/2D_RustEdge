using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private LightControl[] sceneLights;
    private LightShift currentLightShift;
    private Season currentSeason;
    private float timeDifference;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.LightShiftChangeEvent += OnLightShiftChangeEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.LightShiftChangeEvent -= OnLightShiftChangeEvent;

    }



    private void OnAfterSceneLoadedEvent()
    {
        // FindObjectsOfType被FindObjectsByType取代，以后需要使用新版
        sceneLights = FindObjectsByType<LightControl>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (LightControl light in sceneLights)
        {

            //Debug.Log("默认一次切换灯光2");
            light.ChangeLightShift(currentSeason, currentLightShift, timeDifference);
        }
    }
    private void OnLightShiftChangeEvent(Season season, LightShift lightShift, float timeDifference)
    {
        currentSeason = season;
        this.timeDifference = timeDifference;
        if (currentLightShift != lightShift) //需要进行切换灯光
        {
            currentLightShift = lightShift;
            foreach(LightControl light in sceneLights)
            {
                //lightControl 改变灯光的方法
                //Debug.Log("切换灯光1");
                //Debug.Log("currentLightShift：" + currentLightShift);
                //Debug.Log("timeDifference：" + timeDifference);
                light.ChangeLightShift(currentSeason, currentLightShift, timeDifference);

            }
        }
    }
}
