using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject menuCanvas;
    public GameObject menuPrefab;

    public Button settingsBtn;
    public GameObject pausePanel;
    public Slider volumeSlider;

    private void Awake()
    {
        settingsBtn.onClick.AddListener(TogglePausePanel);//点击
    }
    private void OnEnable()
    {
        //游戏场景加载出来之后，我们要把这个主页的MenuCanvas给关闭
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;

    }



    private void Start()
    {
        menuCanvas = GameObject.FindWithTag("MenuCanvas");

        //在运行的时候生成,Prefab。也就是Panel
        Instantiate(menuPrefab, menuCanvas.transform);
    }

    private void OnAfterSceneLoadedEvent()
    {
        if (menuCanvas.transform.childCount > 0)
        {
            //有子物体，说明Panel还在,关掉他
            Destroy(menuCanvas.transform.GetChild(0).gameObject);
        }
    }
    /// <summary>
    /// 暂停游戏，面板控制
    /// </summary>
    private void TogglePausePanel()
    {
        bool isOpen = pausePanel.activeInHierarchy;
        if (isOpen)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            System.GC.Collect(); //在游戏暂停时，进行垃圾回收，是一个好的方法
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }
    public void ReturnMenuCanvas()
    {
        Time.timeScale = 1;
        StartCoroutine(BackToMenu());
    }
    private IEnumerator BackToMenu()
    {
        pausePanel.SetActive(false);
        yield return new WaitForSeconds(1f);
        Instantiate(menuPrefab, menuCanvas.transform);
    }
}
