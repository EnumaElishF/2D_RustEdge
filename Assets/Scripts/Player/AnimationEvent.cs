using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public void FootstepSound()
    {
        //通过音效的接力，只要音效名称，就能进行本次音效的生成
        EventHandler.CallPlaySoundEvent(SoundName.FootStepSoft);
    }
}
