using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Farm.Save
{
    public class SaveLoadManager : Singleton<SaveLoadManager>
    {
        //for循环无法查找场景中所有的ISaveable，因为他是一个接口，那么需要一个注册的方法
        private List<ISaveable> saveableList = new List<ISaveable>();

        public void RegisterSaveable(ISaveable saveable)
        {
            //如果这个场景中不包含该saveable，那就添加上去
            if (!saveableList.Contains(saveable))
            {
                saveableList.Add(saveable);
            }
        }
    }

}
