using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Farm.Save
{
    //每个调用该ISaveable接口的，都要实现下面最基本的方法
    public interface ISaveable
    {
        string GUID { get; }
        void RegisterSaveable()
        {
            SaveLoadManager.Instance.RegisterSaveable(this);
        }

        //存储数据，把所有数据都加到里面，并返回
        GameSaveData GenerateSaveData();
        //生成恢复数据，通过读取GameSaveData，把数据赋值到所有的变量上
        void RestoreData(GameSaveData saveData);


    }

}
