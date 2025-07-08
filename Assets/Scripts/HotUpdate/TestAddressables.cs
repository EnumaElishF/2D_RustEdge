using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TestAddressables : MonoBehaviour
{
    public Sprite sprite;
    // Start is called before the first frame update
    void Start()
    {
/*        //Addressables会有边玩边下，用到什么资源，就去下载，如果不需要下载就用本地的
        //如果存在需要下载的资源，资源在服务器上，那么这些资源我们没法同步获取
        //所以给一个异步获取资源
        Addressables.InstantiateAsync("NPC_Girl05").Completed += TestAddressables_Completed;

        GameObject go = Addressables.InstantiateAsync("NPC_Girl05").WaitForCompletion();

        //Addressables的卸载，：注编辑器下是不会真的卸载的，因为资源存好了，但是Build的会卸载
        Addressables.Release(go);*/
    }

    private void TestAddressables_Completed(AsyncOperationHandle<GameObject> obj)
    {
        obj.Result.name = "TestAddressables";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Addressables.InstantiateAsync("NPC_Girl05").Completed += TestAddressables_Completed;

            GameObject go = Addressables.InstantiateAsync("NPC_Girl05").WaitForCompletion();

            Addressables.Release(go);

            //WaitForCompletion内部会有一个While循环，一直等，把异步硬生生改成同步
            //如果资源都是本地包，那么可以放心的使用WaitForCompletion。
            //但是如果是支持边玩边下，就不适合这种方案，因为这种下载会很慢
            sprite = Addressables.LoadAssetAsync<Sprite>("Girl_006_Hot").WaitForCompletion();


            Addressables.Release(sprite);

        }
    }
}
