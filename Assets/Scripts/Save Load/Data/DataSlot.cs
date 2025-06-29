using Farm.Save;
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
    }

}
