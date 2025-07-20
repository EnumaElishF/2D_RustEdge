using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
public class Announcement : MonoBehaviour
{
    public GameObject announcement;
    public Text text;
    private void Awake()
    {
        // 设置公告文本内容
        if (text != null)
        {
            //text.text = "本次版本更新的内容是雨天效果: 雨滴分为小雨和暴雨1610bug性测试";
            text.text = "本次版本更新的内容是雨天效果: 雨天效果分为无害的雨和对人体有害的黑雨1632";
        }
    }
    public void CloseAnnouncement()
    {
        announcement.SetActive(false);
    }
}
