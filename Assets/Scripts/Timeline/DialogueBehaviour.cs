using Farm.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
[System.Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    private PlayableDirector director;
    public DialoguePiece dialoguePiece;

    public override void OnPlayableCreate(Playable playable)
    {
        //通过正在播放的Graph，反向拿到当前的PlayableDirector，也就是Timeline
        director = (playable.GetGraph().GetResolver() as PlayableDirector);

    }
    /// <summary>
    /// OnBehaviourPlay是一旦开始播放Clip，就调用此方法
    /// </summary>
    /// <param name="playable"></param>
    /// <param name="info"></param>
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        //弹出Dialogue对话框
        EventHandler.CallShowDialogueEvent(dialoguePiece);
        if (Application.isPlaying)
        {
            if (dialoguePiece.hasToPause)
            {
                //暂停Timeline
                TimelineManager.Instance.PauseTimeline(director);
            }
            else
            {
                //结束对话
                EventHandler.CallShowDialogueEvent(null);
            }
        }
    }
    // 在Timeline播放期间，每帧执行
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (Application.isPlaying)
            TimelineManager.Instance.IsDone = dialoguePiece.isDone;
    }
    //在Clip结束后执行
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
       //结束对话
        EventHandler.CallShowDialogueEvent(null);
    }
    public override void OnGraphStart(Playable playable)
    {
        EventHandler.CallUpdateGameStateEvent(GameState.Pause);

    }
    public override void OnGraphStop(Playable playable)
    {
        EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);

    }
}
