using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class HotUpdateSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HotUpdate());
    }

    private IEnumerator HotUpdate()
    {
        //初始化
        yield return Addressables.InitializeAsync();

        //检查目录更新
        //yield return协程会暂停执行，然后去执行checkForCatalogUpdatesHandle直到结束，再恢复协程
        yield return CheckForCatalogUpdates();

        //下载最新的目录

        //下载资源

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
    //获取下载量 ：给用户展示当前的下载进度
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
            // TODO : 分发下载进度
            float percentage = downloadHandle.GetDownloadStatus().Percent; //百分比下载进度
            float currentDownloadSize = downLoadSize * percentage; // 当前的下载量(字节)
            Debug.Log($"下载进度:{currentDownloadSize}/{downLoadSize}");
            yield return null;
        }
        Debug.Log("热更完成");
    }
}
