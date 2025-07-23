using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
public class AudioManager : Singleton<AudioManager>
{
    [Header("音乐数据库")]
    public SoundDetailsList_SO soundDetailsData;
    public SceneSoundList_SO sceneSoundData;
    public AudioSource ambientSource;
    public AudioSource gameSource;

    private Coroutine soundRoutine;
    private Coroutine fadeRoutine; // 新增：用于控制音量渐变的协程

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Snapshots")]
    public AudioMixerSnapshot normalSnapShot;
    public AudioMixerSnapshot ambientSnapShot;
    public AudioMixerSnapshot muteSnapShot; //Mute的时候，直接把Master关掉就没有声音了

    private float musicTransitionSecond = 3f;
    private float volumeFadeDuration = 2f; // 音量渐变持续时间

    public float MusicStartSecond => Random.Range(0f, 1f);

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.PlaySoundEvent += OnPlaySoundEvent;
        EventHandler.EndGameEvent += OnEndGameEvent;
        EventHandler.WeatherEvent += OnWeatherEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.PlaySoundEvent -= OnPlaySoundEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
        EventHandler.WeatherEvent -= OnWeatherEvent;

    }

    private void OnEndGameEvent()
    {
        if (soundRoutine != null)
        {
            StopCoroutine(soundRoutine); //关闭协程soundRoutine
        }
        if (fadeRoutine != null) // 停止音量渐变协程
        {
            StopCoroutine(fadeRoutine);
        }
        muteSnapShot.TransitionTo(1f); //在1f内过渡到静音
    }

    /// <summary>
    /// 实现一个音效的接力，通过音效名称，去调用本次音效的生成
    /// </summary>
    /// <param name="soundName"></param>
    private void OnPlaySoundEvent(SoundName soundName)
    {
        var soundDetails = soundDetailsData.GetSoundDetails(soundName);
        if (soundDetails != null)
        {
            EventHandler.CallInitSoundEffect(soundDetails);
        }
    }

    private void OnAfterSceneLoadedEvent()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneSoundItem sceneSound = sceneSoundData.GetSceneSoundItem(currentScene);
        if (sceneSound == null)
        {
            return;
        }
        SoundDetails ambient = soundDetailsData.GetSoundDetails(sceneSound.ambient);
        SoundDetails music = soundDetailsData.GetSoundDetails(sceneSound.music);

        if (soundRoutine != null) //协程如果不为空，那么就把他停掉，把之前播放的音效停掉
            StopCoroutine(soundRoutine);

        //播放音效
        soundRoutine = StartCoroutine(PlaySoundRoutine(music, ambient));

    }
    /// <summary>
    /// 天气音效
    /// </summary>
    private void OnWeatherEvent(Weather weather, SoundName soundName, bool weatherActive)
    {
        //加入在切换音乐前先逐渐减小之前的音乐的音量
        string currentScene = SceneManager.GetActiveScene().name;
        SceneSoundItem sceneSound = sceneSoundData.GetSceneSoundItem(currentScene);
        if (sceneSound == null)
            return;

        // 停止任何正在进行的渐变协程
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        // 获取当前播放的音乐和环境音效
        SoundDetails currentAmbient = soundDetailsData.GetSoundDetails(sceneSound.ambient);
        SoundDetails targetMusic = weatherActive ?
            soundDetailsData.GetSoundDetails(soundName) :
            soundDetailsData.GetSoundDetails(sceneSound.music);

        // 开始音量渐变协程，完成后切换音乐
        fadeRoutine = StartCoroutine(FadeVolumeAndSwitchMusic(currentAmbient, targetMusic, weatherActive));

    }

    /// <summary>
    /// 音量渐变并切换音乐的协程
    /// </summary>
    private IEnumerator FadeVolumeAndSwitchMusic(SoundDetails ambient, SoundDetails targetMusic, bool weatherActive)
    {
        // 保存当前音量设置
        float originalMusicVolume;
        float originalAmbientVolume;
        audioMixer.GetFloat("MusicVolume", out originalMusicVolume);
        audioMixer.GetFloat("AmbientVolume", out originalAmbientVolume);

        // 逐渐降低当前音乐音量
        float elapsedTime = 0f;
        while (elapsedTime < volumeFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / volumeFadeDuration;

            // 音乐音量从原始值降到最低
            audioMixer.SetFloat("MusicVolume", Mathf.Lerp(originalMusicVolume, -80, t));

            yield return null;
        }

        // 停止当前的音效协程
        if (soundRoutine != null)
        {
            StopCoroutine(soundRoutine);
        }

        // 播放新的音乐
        soundRoutine = StartCoroutine(PlaySoundRoutine(targetMusic, ambient));

        // 重置渐变协程引用
        fadeRoutine = null;
    }

    /// <summary>
    /// 通过协程让我们的背景音乐等待一段随机时间，再去播放，一开始只播放环境音效
    /// 音效过渡！
    /// </summary>
    /// <param name="music"></param>
    /// <param name="ambient"></param>
    /// <returns></returns>
    private IEnumerator PlaySoundRoutine(SoundDetails music,SoundDetails ambient)
    {
        if(music!=null && ambient!= null)
        {
            PlayAmbientClip(ambient,1f); //1秒就切换到AmbientOnly，这样就立马暂停了Music

            //随机暂停协程的时间，协程等待MusicStartSecond秒。在此期间音乐不开启
            yield return new WaitForSeconds(MusicStartSecond);

            //暂停结束后，执行主音乐，经过几秒缓慢从-80，涨到指定的音量。控制等待参数的随机时间参数：要求MusicStartSecond与天气特效开始的时间对应：目前统一到musicTransitionSecond = 10s
            PlayMusicClip(music,musicTransitionSecond);
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayMusicClip(SoundDetails soundDetails,float transitionTime)
    {
        audioMixer.SetFloat("MusicVolume", ConvertSoundVolume(soundDetails.soundVolume));
        gameSource.clip = soundDetails.soundClip;
        if (gameSource.isActiveAndEnabled) //因为在进入房间等情况会有关闭背景音乐的情况，所有需要控制在可用的时候启动音乐
            gameSource.Play();

        normalSnapShot.TransitionTo(transitionTime);
    }
    /// <summary>
    /// 播放环境音效
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayAmbientClip(SoundDetails soundDetails, float transitionTime)
    {
        audioMixer.SetFloat("AmbientVolume", ConvertSoundVolume(soundDetails.soundVolume));

        ambientSource.clip = soundDetails.soundClip;
        if (ambientSource.isActiveAndEnabled)
            ambientSource.Play();

        ambientSnapShot.TransitionTo(transitionTime);
    }

    private float ConvertSoundVolume(float amount)
    {
        return (amount * 100 - 80);
    }
    /// <summary>
    /// 设置音量
    /// </summary>
    /// <param name="value"></param>
    public void SetMasterVolume(float value)
    {
        //给一个-80到+20的值给到MasterVolume
        audioMixer.SetFloat("MasterVolume", (value * 100 - 80));
    }
    /// <summary>
    /// 获取音量
    /// </summary>
    /// <returns></returns>
    public float GetMasterVolume()
    {
        float currentVolume;
        // 尝试获取混音器中的MasterVolume值
        if (audioMixer.GetFloat("MasterVolume", out currentVolume))
        {
            // 将混音器的音量值(-80到20)转换回原始音量范围(0到1)
            // 对应ConvertSoundVolume方法的反向计算
            return (currentVolume + 80) / 100f;
        }
        // 如果获取失败，返回默认值0
        return 0f;
    }


}
