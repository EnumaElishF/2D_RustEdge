using Farm.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public Text dataTime, dataScene;
    private Button currentButton;
    private DataSlot currentData;

    //游戏进度的Index: GetSiblingIndex获得当前兄弟同科的排序
    private int Index => transform.GetSiblingIndex();
    private void Awake()
    {
        currentButton = GetComponent<Button>();
        //点击监听，载入游戏
        currentButton.onClick.AddListener(LoadGameData);
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
            dataScene.text = "还去去过任何地方";
        }
    }
    private void LoadGameData()
    {
        if (currentData != null)
        {
            SaveLoadManager.Instance.Load(Index);
        }
        else
        {
            Debug.Log("新游戏");
            EventHandler.CallStartNewGameEvent(Index);
        }
    }
}
