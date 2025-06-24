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

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Snapshots")]
    public AudioMixerSnapshot normalSnapShot;
    public AudioMixerSnapshot ambientSnapShot;
    public AudioMixerSnapshot muteSnapShot; //Mute的时候，直接把Master关掉就没有声音了
    private float musicTransitionSecond = 8f;

    public float MusicStartSecond => Random.Range(5f, 15f);

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.PlaySoundEvent += OnPlaySoundEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.PlaySoundEvent -= OnPlaySoundEvent;

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
            //随机暂停协程的时间，协程等待MusicStartSecond秒
            yield return new WaitForSeconds(MusicStartSecond);
            //暂停结束后，执行背景音乐，经过几秒缓慢从-80，涨到指定的音量
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
}
