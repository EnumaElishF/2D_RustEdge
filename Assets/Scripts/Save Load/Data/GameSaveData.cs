using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Farm.Save
{
    //选取要存储的数据类型上，我们挑选在游戏中会发生变更的数据类型，记录在这里
    [System.Serializable]
    public class GameSaveData
    {
        public string dataSceneName;
        /// <summary>
        /// 存储人物坐标，string 人物名字
        /// </summary>
        public Dictionary<string, SerializableVector3> characterPosDict;

        //-------ItemManager下的功能，在跨场景时就已经实现，数据的存储，拿到对物体和家具的数据保存
        /// <summary>
        /// 记录当前场景所有的物体
        /// </summary>
        public Dictionary<string, List<SceneItem>> sceneItemDict;
        /// <summary>
        /// 记录场景家具
        /// </summary>
        public Dictionary<string, List<SceneFurniture>> sceneFurnitureDict;

        //-------GridMapManager
        /// <summary>
        /// 场景名称+坐标和对应的瓦片信息 
        /// </summary>
        public Dictionary<string, TileDetails> tileDetailsDict;
        /// <summary>
        /// 记录场景是否第一次加载
        /// </summary>
        public Dictionary<string, bool> firstLoadDict;

        //-------背包InventoryManager
        /// <summary>
        /// 背包格子数据+SlotUI格子数据
        /// </summary>
        public Dictionary<string, List<InventoryItem>> inventoryDict;

        //------时间
        /// <summary>
        /// 时间
        /// </summary>
        public Dictionary<string, int> timeDict;

        //------金钱
        /// <summary>
        /// Player的钱
        /// </summary>
        public int playerMoney;

        //------NPC，NPCManager+NPCMovement   注：NPC的Position已经记录在characterPosDict和Player一块，NPC当前场景在上面
        /// <summary>
        /// NPC的目标场景
        /// </summary>
        public string targetScene;
        /// <summary>
        /// NPC当前是否可以交互
        /// </summary>
        public bool interactable;
        /// <summary>
        /// 通过NPC的实例的对象id，用Resources加载的方式重新生成。因为：AnimationClip是不能被序列化的，所有我们想办法去找到他
        /// </summary>
        public int animationInstanceID;


    }

}
