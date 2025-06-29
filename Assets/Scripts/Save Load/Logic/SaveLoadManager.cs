using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
namespace Farm.Save
{
    public class SaveLoadManager : Singleton<SaveLoadManager>
    {
        //for循环无法查找场景中所有的ISaveable，因为他是一个接口，那么需要一个注册的方法
        private List<ISaveable> saveableList = new List<ISaveable>();

        public List<DataSlot> dataSlots = new List<DataSlot>(new DataSlot[3]);

        private string jsonFolder;
        private int currentDataIndex;

        protected override void Awake()
        {
            base.Awake();
            //在Application.persistentDataPath的使用！，在其路径下，创建名叫/SAVE DATA/的文件夹
            jsonFolder = Application.persistentDataPath + "/SAVE DATA/";
        }
        private void Update()
        {
            //按键I保存当前档，按键O读取档，进行测试，保存和读档是否可用
            if (Input.GetKeyDown(KeyCode.I))
                Save(currentDataIndex);
            if (Input.GetKeyDown(KeyCode.O))
                Load(currentDataIndex);

        }
        public void RegisterSaveable(ISaveable saveable)
        {
            //如果这个场景中不包含该saveable，那就添加上去
            if (!saveableList.Contains(saveable))
            {
                saveableList.Add(saveable);
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="index"></param>
        private void Save(int index)
        {
            DataSlot data = new DataSlot();
            foreach(var saveable in saveableList)
            {
                //执行完后，data里就包含了所有的Manager的数据，并且附带着其对应的GUID
                data.dataDict.Add(saveable.GUID, saveable.GenerateSaveData());
            }
            dataSlots[index] = data;

            //"data" 和".json"都是可以自己的想法命名的，很多游戏也把".json"写成".sav" : ！其中index作为判断的序号是必须的
            var resultPath = jsonFolder + "data" + index + ".json";
            //>>>序列化,  序列化dataSlots[index], 然后选择Formatting的状态，Indented是按一行一行的，把数据方便查看
            var jsonData = JsonConvert.SerializeObject(dataSlots[index], Formatting.Indented);

            if (!File.Exists(resultPath))
            {
                Directory.CreateDirectory(jsonFolder);
            }
            //这里的内容就都是Text文本的String数据，拿到的就是jsonData
            File.WriteAllText(resultPath,jsonData);
        }
        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="index"></param>
        public void Load(int index)
        {
            currentDataIndex = index;
            var resultPath = jsonFolder + "data" + index + ".json";

            //读出来之后，再反序列化
            var stringData = File.ReadAllText(resultPath);
            //<<<<反序列化, 反序列化成DataSlot类型，读取stringData
            var jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);

            foreach(var saveable in saveableList)
            {
                //用saveable.GUID拿到的自己在字典里的 saveData数据，然后就能生成加载了
                saveable.RestoreData(jsonData.dataDict[saveable.GUID]);
            }
        }
    }

}
