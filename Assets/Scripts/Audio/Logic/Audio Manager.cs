using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AudioManager : MonoBehaviour
{
    [Header("音乐数据库")]
    public SoundDetailsList_SO soundDetailsData;
    public SceneSoundList_SO sceneSoundData;
    public AudioSource ambientSource;
    public AudioSource gameSource;

    private Coroutine soundRoutine;
    public float MusicStartSecond => Random.Range(5f, 15f);

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;

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
        soundRoutine = StartCoroutine(PlaySoundRoutinne(music, ambient));

    }
    /// <summary>
    /// 通过协程让我们的背景音乐等待一段随机时间，再去播放，一开始只播放环境音效
    /// </summary>
    /// <param name="music"></param>
    /// <param name="ambient"></param>
    /// <returns></returns>
    private IEnumerator PlaySoundRoutinne(SoundDetails music,SoundDetails ambient)
    {
        if(music!=null && ambient!= null)
        {
            PlayAmbientClip(ambient);
            //随机暂停协程的时间，协程等待MusicStartSecond秒
            yield return new WaitForSeconds(MusicStartSecond);
            //暂停结束后，执行背景音乐
            PlayMusicClip(music);
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayMusicClip(SoundDetails soundDetails)
    {
        gameSource.clip = soundDetails.soundClip;
        if (gameSource.isActiveAndEnabled) //因为在进入房间等情况会有关闭背景音乐的情况，所有需要控制在可用的时候启动音乐
            gameSource.Play();
    }
    /// <summary>
    /// 播放环境音效
    /// </summary>
    /// <param name="soundDetails"></param>
    private void PlayAmbientClip(SoundDetails soundDetails)
    {
        ambientSource.clip = soundDetails.soundClip;
        if (ambientSource.isActiveAndEnabled)
            ambientSource.Play();
    }
}
