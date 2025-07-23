using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeatherManager : Singleton<WeatherManager>
{
    [Header("天气控制")]
    public WeatherDataList_SO weatherDataList_SO;
    public GameObject weatherInPlayer;

    [Header("天气粒子程度")]
    public int small;
    public int medium;
    public int large;

    private bool inHouseScene = false;
    private bool isSceneLoaded = false;

    // 记录状态用于比较和恢复
    private Weather lastWeather = Weather.None;
    private bool lastInHouseScene = false;
    // 保存每个粒子系统的当前速率，使用InstanceID作为键
    private Dictionary<int, float> particleRates = new Dictionary<int, float>();

    Weather currentWeather = Weather.None;
    SoundName soundName = SoundName.none;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        bool newInHouseScene = currentScene == "02Home";

        if (newInHouseScene != inHouseScene)
        {
            inHouseScene = newInHouseScene;

            DirectUpdateWeatherEffects();
        }

        isSceneLoaded = true;
        lastInHouseScene = inHouseScene;
    }

    private void OnGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        if (weatherDataList_SO == null || weatherDataList_SO.weatherDetailsList == null)
        {
            Debug.LogError("未获取到天气数据weatherDataList_SO");
            return;
        }

        int currentTime = hour * 100 + minute;
        Weather newWeather = Weather.None;
        SoundName newSoundName = SoundName.none;

        //检查更新
        if (!inHouseScene)
        {
            foreach (WeatherDetails weatherData in weatherDataList_SO.weatherDetailsList)
            {
                if (weatherData.season != season) continue;
                if (day < weatherData.dayStart || day > weatherData.dayEnd) continue;

                int startTime = weatherData.hourStart * 100 + weatherData.minuteStart;
                int endTime = weatherData.hourEnd * 100 + weatherData.minuteEnd;

                bool isTimeInRange = startTime <= endTime
                    ? currentTime >= startTime && currentTime <= endTime
                    : currentTime >= startTime || currentTime <= endTime;

                if (isTimeInRange)
                {
                    newWeather = weatherData.weather;
                    newSoundName = weatherData.soundName;
                    break;
                }
            }
        }

        //只有在天气发生改变时，才会运行
        if (!inHouseScene && newWeather != currentWeather)
        {
            currentWeather = newWeather;
            soundName = newSoundName;
            UpdateWeatherWithTransition();

            if (currentWeather == Weather.None)
            {
                EventHandler.CallWeatherEvent(Weather.None, SoundName.none, false);
            }
            else
            {
                EventHandler.CallWeatherEvent(currentWeather, soundName, true);
            }

        }
        //检查并重置音效
        if (isSceneLoaded && currentWeather != Weather.None)
        {
            Debug.Log("重置音效");
            isSceneLoaded = false;//只有在切换到新场景才会为true
            EventHandler.CallWeatherEvent(currentWeather, soundName, true);
        }
    }


    private void UpdateWeatherWithTransition()
    {
        if (weatherInPlayer == null) return;

        StopAllCoroutines();

        foreach (Transform child in weatherInPlayer.transform)
        {
            bool isMatch = child.name.Equals(currentWeather.ToString(), System.StringComparison.OrdinalIgnoreCase);

            SaveParticleRates(child);

            if (currentWeather == Weather.None)
            {
                if (child.gameObject.activeSelf)
                {
                    // 获取所有子对象中的粒子系统
                    ParticleSystem[] particles = child.GetComponentsInChildren<ParticleSystem>();
                    if (particles.Length > 0)
                    {
                        // 使用第一个粒子系统的当前速率作为起始点
                        float startRate = GetCurrentParticleRate(particles[0]);
                        StartCoroutine(FadeParticleEmission(child, startRate, small, 10f, true));
                    }
                    else
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
            else if (isMatch)
            {
                child.gameObject.SetActive(true);
                StartCoroutine(FadeParticleEmission(child, small, large, 10f, false));
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }


        lastWeather = currentWeather;
    }

    private void DirectUpdateWeatherEffects()
    {
        if (weatherInPlayer == null) return;

        StopAllCoroutines();

        foreach (Transform child in weatherInPlayer.transform)
        {
            bool isMatch = child.name.Equals(currentWeather.ToString(), System.StringComparison.OrdinalIgnoreCase);

            if (inHouseScene)
            {
                SaveParticleRates(child);
                child.gameObject.SetActive(false);
            }
            else
            {
                if (isMatch && currentWeather != Weather.None)
                {
                    child.gameObject.SetActive(true);
                    Debug.Log("直接获取成功，还差粒子存储完成");
                    RestoreParticleRates(child);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

    }

    /// <summary>
    /// 保存父对象下所有粒子系统的当前速率
    /// </summary>
    private void SaveParticleRates(Transform parent)
    {
        ParticleSystem[] particles = parent.GetComponentsInChildren<ParticleSystem>();
        foreach (var particle in particles)
        {
            int instanceId = particle.GetInstanceID();
            float currentRate = particle.emission.rateOverTime.constant;

            if (particleRates.ContainsKey(instanceId))
                particleRates[instanceId] = currentRate;
            else
                particleRates.Add(instanceId, currentRate);
        }
    }

    /// <summary>
    /// 恢复父对象下所有粒子系统之前保存的速率
    /// </summary>
    private void RestoreParticleRates(Transform parent)
    {
        ParticleSystem[] particles = parent.GetComponentsInChildren<ParticleSystem>();
        foreach (var particle in particles)
        {
            int instanceId = particle.GetInstanceID();

            Debug.Log("粒子转换完成instanceId:" + instanceId);
            if (particleRates.TryGetValue(instanceId, out float rate))
            {
                var emission = particle.emission;
                var rateOverTime = emission.rateOverTime;
                rateOverTime.constant = rate;
                emission.rateOverTime = rateOverTime;
            }
            else
            {
                // 如果没有保存的速率，使用默认的large
                var emission = particle.emission;
                var rateOverTime = emission.rateOverTime;
                rateOverTime.constant = large;
                emission.rateOverTime = rateOverTime;
            }
        }
    }

    /// <summary>
    /// 获取粒子系统当前的发射速率
    /// </summary>
    private float GetCurrentParticleRate(ParticleSystem particle)
    {
        if (particle != null)
        {
            return particle.emission.rateOverTime.constant;
        }
        return small;
    }

    /// <summary>
    /// 协程：平滑过渡所有子对象中粒子系统的发射速率
    /// </summary>
    private IEnumerator FadeParticleEmission(Transform parent, float startRate, float endRate, float duration, bool disableAtEnd)
    {
        float elapsedTime = 0f;
        // 获取父对象下所有的粒子系统（包括子对象）
        ParticleSystem[] particles = parent.GetComponentsInChildren<ParticleSystem>();

        if (particles.Length == 0)
        {
            Debug.LogWarning($"在{parent.name}及其子对象下未找到粒子系统组件");
            yield break;
        }

        // 保存原始的发射速率并设置初始速率
        float[] originalRates = new float[particles.Length];
        for (int i = 0; i < particles.Length; i++)
        {
            originalRates[i] = particles[i].emission.rateOverTime.constant;

            var emission = particles[i].emission;
            var rateOverTime = emission.rateOverTime;
            rateOverTime.constant = startRate;
            emission.rateOverTime = rateOverTime;
        }

        // 分阶段平滑过渡
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            float currentRate;
            if (t < 0.33f) // 第一阶段：0-33%
            {
                currentRate = Mathf.Lerp(startRate, medium, t / 0.33f);
            }
            else if (t < 0.66f) // 第二阶段：33%-66%
            {
                currentRate = Mathf.Lerp(medium, (medium + endRate) / 2, (t - 0.33f) / 0.33f);
            }
            else // 第三阶段：66%-100%
            {
                currentRate = Mathf.Lerp((medium + endRate) / 2, endRate, (t - 0.66f) / 0.34f);
            }

            // 更新所有粒子系统的发射速率
            foreach (var particle in particles)
            {
                var emission = particle.emission;
                var rateOverTime = emission.rateOverTime;
                rateOverTime.constant = currentRate;
                emission.rateOverTime = rateOverTime;
            }

            yield return null;
        }

        // 确保最终值准确
        foreach (var particle in particles)
        {
            var emission = particle.emission;
            var rateOverTime = emission.rateOverTime;
            rateOverTime.constant = endRate;
            emission.rateOverTime = rateOverTime;
        }

        // 保存最终速率
        SaveParticleRates(parent);

        // 过渡结束后禁用（如果需要）
        if (disableAtEnd)
        {
            parent.gameObject.SetActive(false);
        }
    }
}
