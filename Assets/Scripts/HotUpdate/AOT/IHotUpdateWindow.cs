using System;

public interface IHotUpdateWindow
{
    //public string GetGameSceneName();
    public void Show(long totalBytes,Action onEnd);
    public void UpdateDownloadedProgress(float progress);


}
