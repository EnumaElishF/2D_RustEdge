using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class LightControl : MonoBehaviour
{
    public LightPatternList_SO lightData;
    private Light2D currentLight;
    private LightDetails currentLightDetails;

    private void Awake()
    {
        currentLight = GetComponent<Light2D>();

    }
    /// <summary>
    /// 实际切换灯光,灯光变化的方法
    /// </summary>
    /// <param name="season"></param>
    /// <param name="lightShift"></param>
    /// <param name="timeDifference"></param>
    public void ChangeLightShift(Season season, LightShift lightShift, float timeDifference)
    {
        //缓慢切换灯光效果，需要利用到DOTween
        currentLightDetails = lightData.GetLightDetails(season, lightShift); //能够得到光对应的颜色

        if (timeDifference < Settings.lightChangeDuration)
        {
            // 时间差值，占有lightChangeDuration的百分比，得到对应Color的offset
            var colorOffset = (currentLightDetails.lightColor - currentLight.color) / Settings.lightChangeDuration * timeDifference;
            currentLight.color += colorOffset;

            //color的随时间变更
            //Settings.lightChangeDuration - timeDifference,因为游戏是以分钟的变化，计算，时间变化非常快了，把分钟当成秒来走，结果的float值是用的分钟。
            //如果有需要调整的地方，可以变更这里的时间float
            DOTween.To(() => currentLight.color, c => currentLight.color = c, currentLightDetails.lightColor, Settings.lightChangeDuration - timeDifference);

            //intensity的随时间变更
            DOTween.To(() => currentLight.intensity, i => currentLight.intensity = i, currentLightDetails.lightAmount, Settings.lightChangeDuration - timeDifference);


        }
        if (timeDifference >= Settings.lightChangeDuration)
        {
            currentLight.color = currentLightDetails.lightColor;
            currentLight.intensity = currentLightDetails.lightAmount;
        }
    }
}
