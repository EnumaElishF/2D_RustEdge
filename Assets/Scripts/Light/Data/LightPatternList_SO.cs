using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LightPatternList_SO",menuName = "Light/Light Pattern")]
public class LightPatternList_SO : ScriptableObject
{
    public List<LightDetails> lightPatternList;
    /// <summary>
    /// 根据季节和周期返回灯光详情
    /// </summary>
    /// <param name="season">季节</param>
    /// <param name="lightShift">周期</param>
    /// <returns></returns>
    public LightDetails GetLightDetails(Season season, LightShift lightShift)
    {
        LightDetails list = lightPatternList.Find(l => l.season == season && l.lightShift == lightShift);
        //！注意中文的检查情况 ，因季节Season的中文导致搜索为null的情况
        //LightDetails list = lightPatternList.Find(l => l.lightShift == lightShift);  

        if (list == null)
        {
            Debug.Log("没有查询到灯光，season："+ season+ ",lightShift:"+ lightShift);
        }
        return list;
    }
}
[System.Serializable]
public class LightDetails
{
    public Season season;
    public LightShift lightShift;
    public Color lightColor;
    public float lightAmount;
}
