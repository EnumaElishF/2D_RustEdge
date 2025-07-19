using UnityEngine;
using UnityEngine.AddressableAssets;

public class UpdateAnnouncement : MonoBehaviour
{
    // 游戏物体放入AssetBundle中，物体上挂这个脚本
    //这个脚本不需要知道，他是热更新的，因为他不会和HotUpdateSystem有联系，只需要把他实例化出来就行
    private void Awake()
    {
        Debug.Log("游戏开始，游戏公告展示");
        Debug.Log("游戏公告进行一次更新");

        //用完把自己释放掉就行了
        //Addressables.ReleaseInstance(gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
