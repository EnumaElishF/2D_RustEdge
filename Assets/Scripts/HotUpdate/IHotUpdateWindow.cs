public interface IHotUpdateWindow
{
    //public string GetGameSceneName();
    public void Show(long totalBytes);
    public void UpdateDownloadedProgress(float progress);


}
