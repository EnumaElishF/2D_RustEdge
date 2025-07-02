using Farm.Save;
using Farm.Transition;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Farm.Save
{
    public class DataSlot
    {
        /// <summary>
        /// 进度条，String是GUID
        /// </summary>
        public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();

        #region 用到ui显示进度详情
        public string DataTime
        {
            get
            {
                var key = TimeManager.Instance.GUID;
                if (dataDict.ContainsKey(key))
                {
                    var timeData = dataDict[key];
                    //存档时间
                    return timeData.timeDict["gameYear"] + "年/" + (Season)timeData.timeDict["gameSeason"] + "/" + timeData.timeDict["gameMonth"] + "月/" + timeData.timeDict["gameDay"] + "日/"+timeData.timeDict["gameHour"]+":"+timeData.timeDict["gameMinute"];
                }
                else return string.Empty;
            }
        }

        public string DataScene
        {
            get
            {
                var key = TransitionManager.Instance.GUID;
                if (dataDict.ContainsKey(key))
                {
                    var transitionData = dataDict[key];
                    //根据场景名称，返回对应中文名称
                    return transitionData.dataSceneName switch
                    {
                        "00_Start" => "海边",
                        "01_Field" => "农场",
                        "02Home" => "小木屋",
                        "03_Stall"=>"市场",
                        _ => string.Empty
                    };
                }
                else return string.Empty;
            }
        }
        #endregion

    }

}
