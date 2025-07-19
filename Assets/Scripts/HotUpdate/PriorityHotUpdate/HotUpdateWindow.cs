using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HotUpdateWindow : MonoBehaviour,IHotUpdateWindow
{
    public Image progressBarFill;
    public Text progressText;
    public float updateSpeed = 0.5f;
    [Header("版本更新提示文本")]
    public Text versionText;


    private long totalBytes;
    private float currentProgress;
    private float progress;
    private Action onEnd;
    public void Show(long totalBytes,Action onEnd)
    {
        gameObject.SetActive(true);
        this.totalBytes = totalBytes;
        this.onEnd = onEnd;

        // 设置公告文本内容
        if (versionText != null)
        {
            //versionText.text = "版本更新过程中,请不要关闭游戏。请稍后，即将开始游戏。";
            versionText.text = "健康游戏声明: 抵制不良游戏，拒绝盗版游戏。 注意自我保护，谨防受骗上当。 适度游戏益脑，沉迷游戏伤身。 合理安排时间，享受健康生活。";
        }
    }
    public void UpdateDownloadedProgress(float progress)
    {
        this.progress = progress;

    }
    /// <summary>
    /// 已下载进度
    /// </summary>
    /// <param name="downloadedBytes"></param>
    public void UpdateDownloadedBytes(long downloadedBytes)
    {
        //要让进度条平滑一些,在updateSpeed上作为限制,  progress是实际的下载总进度
        //渐变性的拉满进度条
        progress = (float)downloadedBytes / (float)totalBytes;

    }
    private void Update()
    {
        //渐变性的拉满进度条的progress

        currentProgress = Mathf.MoveTowards(currentProgress, progress, Time.deltaTime * updateSpeed);
        progressBarFill.fillAmount = currentProgress;
        //除两次1024从Byte变成KB，MB
        progressText.text = $"{totalBytes * currentProgress / 1024f / 1024f}MB/{totalBytes / 1024f / 1024f}MB";
        if (currentProgress >= 1)
        {
            onEnd?.Invoke();
            onEnd = null;
        }
    }

}
