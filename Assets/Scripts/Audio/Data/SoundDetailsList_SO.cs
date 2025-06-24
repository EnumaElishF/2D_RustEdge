using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 包括所有的声音的信息
[CreateAssetMenu(fileName = "SoundDetailsList_SO",menuName = "Sound/SoundDetailsList")]
public class SoundDetailsList_SO : ScriptableObject
{
    public List<SoundDetails> soundDetailsList;
    public SoundDetails GetSoundDetails(SoundName name)
    {
        return soundDetailsList.Find(s => s.soundName == name);
    }
}

//对应的数据的类型
[System.Serializable]
public class SoundDetails
{
    public SoundName soundName; //写为枚举类型，方便选择
    public AudioClip soundClip;
    //让音阶（音高）在 0.8f和1.2f之间变化，这样在砍树等动作时，声音会比较有意思
    [Range(0.1f,1.5f)]
    public float soundPitchMin;
    [Range(0.1f, 1.5f)]
    public float soundPitchMax;
    //音量
    [Range(0.1f, 1f)]
    public float soundVolume;
}