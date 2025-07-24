using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DisplayModeManager : MonoBehaviour
{
    public Dropdown displayModeDropdown;
    public string windowedModeText = "窗口模式";
    public string exclusiveFullscreenText = "全屏模式";

    private const int WindowedWidth = 1280;
    private const int WindowedHeight = 720;

    private FullScreenMode currentDisplayMode;

    private void Start()
    {
        InitializeDisplayModeDropdown();
        // 验证下拉框选项数量
        //if (displayModeDropdown != null)
        //{
        //    Debug.Log("下拉框选项数量: " + displayModeDropdown.options.Count); 
        //}
        LoadSettings();

        // 强制刷新当前显示模式（解决初始模式识别问题）
        currentDisplayMode = Screen.fullScreenMode;
        displayModeDropdown.value = GetDisplayModeIndex(currentDisplayMode);
        displayModeDropdown.RefreshShownValue();
    }

    private void InitializeDisplayModeDropdown()
    {
        if (displayModeDropdown != null)
        {
            displayModeDropdown.ClearOptions();

            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>
            {
                new Dropdown.OptionData(windowedModeText),
                new Dropdown.OptionData(exclusiveFullscreenText)
            };

            displayModeDropdown.AddOptions(options);
        }
        //重要：监听displayModeDropdown，从而调用方法OnDisplayModeChange
        displayModeDropdown.onValueChanged.AddListener(OnDisplayModeChange);
    }

    private int GetDisplayModeIndex(FullScreenMode mode)
    {
        // 只有明确是独占全屏时才返回1，其他情况都视为窗口模式
        return mode == FullScreenMode.ExclusiveFullScreen ? 1 : 0;
    }

    private FullScreenMode GetFullScreenModeFromIndex(int index)
    {
        return index == 0 ? FullScreenMode.Windowed : FullScreenMode.ExclusiveFullScreen;
    }

    public void OnDisplayModeChange(int index)
    {
        Debug.Log($"切换索引为: {index}");

        currentDisplayMode = GetFullScreenModeFromIndex(index);
        // 调试日志：确认选择的模式
        Debug.Log($"切换模式为: {currentDisplayMode}");
    }



    public void ApplySettings()
    {
        FullScreenMode targetMode = currentDisplayMode;

        if (targetMode == FullScreenMode.Windowed)
        {
            Screen.SetResolution(
                WindowedWidth,
                WindowedHeight,
                targetMode,
                Screen.currentResolution.refreshRateRatio
            );
            Debug.Log($"已应用窗口模式: {WindowedWidth}x{WindowedHeight}");
        }
        else
        {
            // 独占全屏需要先获取当前显示器的最佳分辨率
            Resolution bestResolution = Screen.resolutions[Screen.resolutions.Length - 1];
            Screen.SetResolution(
                bestResolution.width,
                bestResolution.height,
                targetMode,
                bestResolution.refreshRateRatio
            );
            Debug.Log($"已应用独占全屏模式: {bestResolution.width}x{bestResolution.height}");
        }

        // 强制刷新显示模式状态
        currentDisplayMode = Screen.fullScreenMode;
        SaveSettings();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("DisplayMode", GetDisplayModeIndex(currentDisplayMode));
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("DisplayMode"))
        {
            int displayModeIndex = PlayerPrefs.GetInt("DisplayMode");
            currentDisplayMode = GetFullScreenModeFromIndex(displayModeIndex);

            if (displayModeDropdown != null)
            {
                displayModeDropdown.value = displayModeIndex;
                displayModeDropdown.RefreshShownValue();
            }
        }
    }

    public void ResetToDefault()
    {
        currentDisplayMode = FullScreenMode.Windowed;

        if (displayModeDropdown != null)
        {
            displayModeDropdown.value = GetDisplayModeIndex(currentDisplayMode);
            displayModeDropdown.RefreshShownValue();
        }
    }
}
