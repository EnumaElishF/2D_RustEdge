using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public Text dataTime, dataScene;
    private Button currentButton;

    //游戏进度的Index: GetSiblingIndex获得当前兄弟同科的排序
    private int Index => transform.GetSiblingIndex();
    private void Awake()
    {
        currentButton = GetComponent<Button>();
        currentButton.onClick.AddListener(LoadGameData);
    }
    private void LoadGameData()
    {
        Debug.Log(Index);
    }
}
