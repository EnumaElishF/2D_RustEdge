using DG.Tweening;
using Farm.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialogueBox;
    public Text dialogueText;
    public Image faceRight, faceLeft;
    public Text nameRight, nameLeft;
    public GameObject continueBox;

    private void Awake()
    {
        continueBox.SetActive(false);
    }
    private void OnEnable()
    {
        EventHandler.ShowDialogueEvent += OnShowDialogueEvent;
    }
    private void OnDisable()
    {
        EventHandler.ShowDialogueEvent -= OnShowDialogueEvent;
    }

    private void OnShowDialogueEvent(DialoguePiece piece)
    {
        StartCoroutine(ShowDialogue(piece));
    }
    private IEnumerator ShowDialogue(DialoguePiece piece)
    {
        if (piece != null)
        {
            piece.isDone = false;
            dialogueBox.SetActive(true);
            continueBox.SetActive(false);

            dialogueText.text = string.Empty;
            if (piece.name != string.Empty)
            {
                if (piece.onLeft) //根据左右显示名称，或者对话人物图
                {
                    faceRight.gameObject.SetActive(false);
                    faceLeft.gameObject.SetActive(true);
                    faceLeft.sprite = piece.faceImage;
                    nameLeft.text = piece.name;
                }
                else
                {
                    faceLeft.gameObject.SetActive(false);
                    faceRight.gameObject.SetActive(true);
                    faceRight.sprite = piece.faceImage;
                    nameRight.text = piece.name;
                }
            }
            else
            {
                faceLeft.gameObject.SetActive(false);
                faceRight.gameObject.SetActive(false);
                nameLeft.gameObject.SetActive(false);
                nameRight.gameObject.SetActive(false);
            }
            yield return dialogueText.DOText(piece.dialogueText, 1f).WaitForCompletion();

            piece.isDone = true;
            if (piece.hasToPause && piece.isDone)
                continueBox.SetActive(true);
        }
        else
        {
            dialogueBox.SetActive(false); //对话piece没有，对话结束
            yield break; //跳出协程
        }
    }
}
