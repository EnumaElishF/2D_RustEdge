using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Farm.Dialogue
{
    /// <summary>
    /// DialoguePiece 每一条对话
    /// </summary>
    [System.Serializable]
    public class DialoguePiece
    {
        [Header("对话详情")]
        public Sprite faceImage;
        public bool onLeft;
        public string name;
        [TextArea]
        public string dialogueText;
        public bool hasToPause;
        [HideInInspector] public bool isDone;
    }

}
