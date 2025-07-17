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

    private long totalBytes;
    private float currentProgress;
    private float progress;
    private Action onEnd;
    public void Show(long totalBytes,Action onEnd)
    {
        gameObject.SetActive(true);
        this.totalBytes = totalBytes;
        this.onEnd = onEnd;
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
