using HybridCLR;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Linq;
using System.Data.SqlTypes;
public class HotUpdateSystem : MonoBehaviour
{
    public const string DllConfigKey = "DllConfig";
    public const string hotUpdateWindowKey = "HotUpdateWindow";
    private IHotUpdateWindow hotUpdateWindow;
    private DllConfig dllConfig;
    //持久化目录的路径
    private string persistentDataPath_addressables => $"{Application.persistentDataPath}/com.unity.addressables";
    private string catalogPath => $"{persistentDataPath_addressables}/catalog_1.0.json";
    //HashSet存已经加载过的dll的名称。防止多塞进去
    private HashSet<string> loadedDlls = new HashSet<string>();
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HotUpdate());
    }

    private IEnumerator HotUpdate()
    {
        //断点续传
        int hotUpdateState = PlayerPrefs.GetInt("HotUpdateSucceed", 0); //标记:0
        if (hotUpdateState == 0)//代表上一次热更没有成功
        {
            Debug.Log("断点续传");
            if (Directory.Exists(persistentDataPath_addressables)) Directory.Delete(persistentDataPath_addressables, true);
        }
        //PlayerPrefs 是 Unity 提供的一种轻量级本地数据存储工具，用于保存简单的键值对
        //这里的键 HotUpdateSucceed 是一个自定义的状态标记，用于记录 “热更新是否成功完成”：
        PlayerPrefs.SetInt("HotUpdateSucceed", 0);

        //初始化
        yield return Addressables.InitializeAsync();

        //检查目录更新
        //yield return协程会暂停执行，然后去执行checkForCatalogUpdatesHandle直到结束，再恢复协程
        yield return CheckForCatalogUpdates();
        PlayerPrefs.SetInt("HotUpdateSucceed", 1);


        //无论是否热更都会走到此处
        //加载热更新程序集
        LoadHotUpdateAssembly();
        //加载AOT程序集元数据
        LoadMetadataForAOTAssembly();

        //跳转到游戏场景，方式一
        //JumpToGameScene();
        yield return StartCoroutine(JumpToGameScene());

        //跳转到游戏场景，方式二
        //Addressables.InitializeAsync("XXX").WaitForCompletion();

    }

    //检查目录更新
    private IEnumerator CheckForCatalogUpdates()
    {
        AsyncOperationHandle<List<string>> checkForCatalogUpdatesHandle = Addressables.CheckForCatalogUpdates(false);
        yield return checkForCatalogUpdatesHandle;
        if (checkForCatalogUpdatesHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"CheckForCatalogueUpdates失败:{checkForCatalogUpdatesHandle.OperationException}");
        }
        else
        {
            Debug.Log($"CheckForCatalogueUpdates成功");
            List<string> catalogResult = checkForCatalogUpdatesHandle.Result;
            if (catalogResult.Count > 0)
            {
                //要更新
                for (int i = 0; i < catalogResult.Count; i++)
                {
                    Debug.Log(catalogResult[i]);
                }
                yield return UpdateCatalogs(catalogResult);
            }
            else
            {
                Debug.Log("无需更新");
            }

        }
        Addressables.Release(checkForCatalogUpdatesHandle);
    }
    //更新目录 : 获得目录也就几k的量，很快就能解决；下载量多的地方重点是下面的资源，可能要几百兆
    private IEnumerator UpdateCatalogs(List<string> catalogResult)
    {
        //false 控制是否自动下载更新的资源，如果不加判断bool可能自动销毁
        AsyncOperationHandle<List<IResourceLocator>> updateCatalogsHandle = Addressables.UpdateCatalogs(catalogResult,false);
        yield return updateCatalogsHandle;
        if (updateCatalogsHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"updateCatalogs失败:{updateCatalogsHandle.OperationException}");
        }
        else
        {
            yield return PriorityHotUpdate();

            Debug.Log($"updateCatalogs成功");
            List<IResourceLocator> locatorList =  updateCatalogsHandle.Result;
            if (locatorList.Count > 0)
            {
                List<object> keys = new List<object>(1000);
                for(int i = 0; i < locatorList.Count; i++)
                {
                    Debug.Log(locatorList[i].LocatorId);
                    keys.AddRange(locatorList[i].Keys);
                }
                //TODO：下载资源
                yield return GetDownloadSize(keys);
            }
        }
        //最终不管成功还是失败都要Release掉
        Addressables.Release(updateCatalogsHandle);
    }
    //获取下载量     ：给用户展示当前的下载进度
    private IEnumerator GetDownloadSize(IEnumerable<object> keys)
    {
        AsyncOperationHandle<long> sizeHandle  = Addressables.GetDownloadSizeAsync(keys);
        yield return sizeHandle;
        if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"GetDownloadSize失败:{sizeHandle.OperationException}");
        }
        else
        {
            //下载进度：downLoadSize
            long downLoadSize = sizeHandle.Result;
            if (downLoadSize > 0)
            {
                hotUpdateWindow.Show(downLoadSize);

                yield return DownLoadDependencies(keys, downLoadSize);
            }
            else
            {
                Debug.Log("无需更新");
            }
        }
        Addressables.Release(sizeHandle);

    }
    //下载具体的资源
    private IEnumerator DownLoadDependencies(IEnumerable<object> keys,long downLoadSize)
    {
        //Addressables.MergeMode.Union 是最常用的合并模式，因为它符合大多数场景下的优化需求.如果有重复的依赖项，只下载一次
        AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(keys,Addressables.MergeMode.Union, false);
        //因为涉及到下载的资源会有几百兆到上G的情况，不能适应yield return downloadHandle;这样就直接等死机了。
        //yield return downloadHandle;
        //所以使用：----循环查看下载进度
        while (!downloadHandle.IsDone)
        {
            if(downloadHandle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"DownLoadDependencies失败:{downloadHandle.OperationException}");
                break;
            }

            // 分发下载进度
            float percentage = downloadHandle.GetDownloadStatus().Percent; //百分比下载进度(0到1)
            long currentDownloadSize = (long) (downLoadSize * percentage); // 当前的下载量(字节)
            Debug.Log($"下载进度:{currentDownloadSize}/{downLoadSize}");

            hotUpdateWindow.UpdateDownloadedProgress(downloadHandle.GetDownloadStatus().Percent );
            yield return null;
        }
        Debug.Log("热更完成");

    }

    //优先热更新
    private IEnumerator PriorityHotUpdate()
    {
        //下载并加载DllConfig文件
        //先下载dll配置文件
        yield return Addressables.DownloadDependenciesAsync(DllConfigKey, true);
        //加载DllConfig文件
        dllConfig = Addressables.LoadAssetAsync<DllConfig>(DllConfigKey).WaitForCompletion();

        //下载优先热更程序集
        yield return Addressables.DownloadDependenciesAsync(dllConfig.priorityHotUpdate, Addressables.MergeMode.Union, true);

#if !UNITY_EDITOR
        //加载优先热更程序集
        foreach (string dllName in dllConfig.priorityHotUpdate)
        {
            TextAsset dllText = Addressables.LoadAssetAsync<TextAsset>(dllName).WaitForCompletion();
            LoadDll(dllText);
        }
        ReloadContentCatalog(); //重新加载目录
#endif

        //当目录拿到以后，先下载HotUpdateWindow相关的内容并加载出窗口
        yield return Addressables.DownloadDependenciesAsync(hotUpdateWindowKey, true);
        hotUpdateWindow = Addressables.InstantiateAsync(hotUpdateWindowKey).WaitForCompletion().GetComponent<IHotUpdateWindow>();

    }


    /// <summary>
    /// dll工具加载函数
    /// </summary>
    private void LoadDll(TextAsset dllTextAsset)
    {
        if (!loadedDlls.Contains(dllTextAsset.name))
        {
            System.Reflection.Assembly.Load(dllTextAsset.bytes);
            loadedDlls.Add(dllTextAsset.name);
            Debug.Log($"加载程序集:{dllTextAsset.name}");
        }
        Addressables.Release(dllTextAsset);
    }

    private void ReloadContentCatalog()
    {
        Addressables.LoadContentCatalogAsync(catalogPath).WaitForCompletion();
    }

    /// <summary>
    /// 加载AOT程序集元数据
    /// </summary>
    public  void LoadMetadataForAOTAssembly()
    {
        //不在Editor下运行
#if UNITY_EDITOR
        return;
#endif

        //补充元数据
        foreach (string dllName in dllConfig.aot)
        {
            TextAsset textAsset = Addressables.LoadAssetAsync<TextAsset>(dllName).WaitForCompletion();
            // 执行补充元数据时内部会自动将dllBytes复制一份，调用完成后请不要将dllBytes保存，造成无谓的内存浪费
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(textAsset.bytes, HomologousImageMode.SuperSet);
            Debug.Log($"LoadMetadataForAOTAssembly:{textAsset.name}. ret:{err}");
        }
        
        
    }
    /// <summary>
    /// 加载热更新程序集
    /// </summary>
    private void LoadHotUpdateAssembly()
    {
        //不需要在Editor下运行
        // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if UNITY_EDITOR
        return;
#endif
        //加载优先热更程序集
        foreach (string dllName in dllConfig.priorityHotUpdate)
        {
            TextAsset dllText = Addressables.LoadAssetAsync<TextAsset>(dllName).WaitForCompletion();
            LoadDll(dllText);
        }
        //加载热更程序集
        foreach (string dllName in dllConfig.hotUpdate)
        {
            TextAsset dllText = Addressables.LoadAssetAsync<TextAsset>(dllName).WaitForCompletion();
            LoadDll(dllText);
        }

        ReloadContentCatalog(); //重新加载目录

    }




    /// <summary>
    /// 最后跳转到游戏场景前：需要考虑做的事情！
    /// </summary>
    private IEnumerator JumpToGameScene()
    {

        if (hotUpdateWindow != null)
        {
            //延迟两秒后再关
            //
            yield return new WaitForSeconds(2f); // 等待2秒


            //hotUpdateWindow转换成MonoBehaviour或者更上层的Component，因为他一定个组件
            Addressables.Release(((Component)hotUpdateWindow).gameObject);
        }
        //最后要释放掉dll配置文件
        if (dllConfig != null)
        {
            Addressables.Release(dllConfig);
        }

        //热更完成可以去跳转到需要的场景
        //SceneManager.LoadScene("Game");
    }
}
