using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
//每一条轨道上的小方块的Clip
public class DialogueClip : PlayableAsset,ITimelineClipAsset
{
    public ClipCaps clipCaps => ClipCaps.None;
    public DialogueBehaviour dialogue = new DialogueBehaviour();
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<DialogueBehaviour>.Create(graph, dialogue);
        return playable;
    }
}
