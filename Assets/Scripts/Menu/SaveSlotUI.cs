using Farm.Save;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public Text dataTime, dataScene;

    [Header("存档删除按钮")]
    public Button deleteButton;
    public GameObject deleteCheckCanvas;
    public Button deleteButtonY;
    public Button deleteButtonN;

    private Button currentButton;
    private DataSlot currentData;


    //游戏进度的Index: GetSiblingIndex获得当前兄弟同科的排序
    private int Index => transform.GetSiblingIndex();


    private void Awake()
    {
        currentButton = GetComponent<Button>();
        //点击监听，载入游戏
        currentButton.onClick.AddListener(LoadGameData);

        //删除存档
        deleteButton.onClick.AddListener(OpenDeleteWindow);
        deleteButtonY.onClick.AddListener(DeleteArchive);
        deleteButtonN.onClick.AddListener(CloseDeleteWindow);

    }
    private void OnEnable()
    {
        SetUpSlotUI();
    }
    private void SetUpSlotUI()
    {
        currentData = SaveLoadManager.Instance.dataSlots[Index];
        if (currentData != null)
        {
            dataTime.text = currentData.DataTime;
            dataScene.text = currentData.DataScene;
        }
        else
        {
            //如果没有存档，
            dataTime.text = "这个世界还没开始";
            dataScene.text = "还没有去过任何地方";
        }
    }
    private void LoadGameData()
    {
        if (currentData != null)
        {
            Debug.Log("载入Index:" + Index);

            SaveLoadManager.Instance.Load(Index);
        }
        else
        {
            Debug.Log("新游戏");
            EventHandler.CallStartNewGameEvent(Index);
        }
    }
    private void OpenDeleteWindow()
    {
        //打开删除窗口：
        deleteCheckCanvas.SetActive(true);
    }
    /// <summary>
    /// 删除存档
    /// </summary>
    private void DeleteArchive()
    {
        string jsonFolder = Application.persistentDataPath + "/SAVE DATA/";
        string filePath = jsonFolder + "data" + Index + ".json";
        //解决走了三次的问题
        Debug.Log("filePath:" + filePath);

        try
        {
            // 检查文件夹是否存在
            if (Directory.Exists(jsonFolder))
            {
                // 检查文件是否存在
                if (File.Exists(filePath))
                {
                    // 删除文件
                    File.Delete(filePath);
                    Debug.Log($"成功删除存档: {filePath}");

                    // 从内存中移除存档数据
                    SaveLoadManager.Instance.dataSlots[Index] = null;

                }
                else
                {
                    Debug.LogWarning($"存档文件不存在: {filePath}");
                }
            }
            else
            {
                Debug.LogWarning($"存档文件夹不存在: {jsonFolder}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"删除存档时发生错误: {e.Message}");
        }
        finally
        {
            //删除完成后，关闭此Window,
            CloseDeleteWindow();
            //然后重新读取一次ui
            SetUpSlotUI();
        }
    }
    private void CloseDeleteWindow()
    {
        deleteCheckCanvas.SetActive(false);
    }
}
