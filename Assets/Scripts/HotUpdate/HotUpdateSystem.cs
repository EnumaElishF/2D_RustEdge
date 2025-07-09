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
    //更新目录
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
                List<object> downloadKeys = new List<object>(1000);
                for(int i = 0; i < locatorList.Count; i++)
                {
                    Debug.Log(locatorList[i].LocatorId);
                    downloadKeys.AddRange(locatorList[i].Keys);
                }
                //TODO：下载资源
                yield return DownloadAsset(downloadKeys);
            }
        }
        //最终不管成功还是失败都要Release掉
        Addressables.Release(updateCatalogsHandle);
    }
    //下载资源
    private IEnumerator DownloadAsset(List<object> downloadKeys)
    {
        for(int i = 0; i < downloadKeys.Count; i++)
        {
            Debug.Log(downloadKeys[i]);
        }
        yield return null;

    }
}
