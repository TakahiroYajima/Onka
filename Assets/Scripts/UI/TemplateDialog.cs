using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TemplateDialog : DialogBase
{
    [SerializeField] private Text messageText = null;
    [SerializeField] private TextButton yesButton = null;
    [SerializeField] private TextButton noButton = null;

    public void Open(string message, TempDialogType tempDialogType, UnityAction<bool> pressButtonCallback)
    {
        yesButton.button.onClick.AddListener(() => { pressButtonCallback(true); });
        noButton.button.onClick.AddListener(() => { pressButtonCallback(false); });
        if(tempDialogType != TempDialogType.YesOrNo)
        {
            noButton.gameObject.SetActive(false);
        }
        if(tempDialogType == TempDialogType.InButtonMessage)
        {
            messageText.text = string.Empty;
            yesButton.text.text = message;
        }
        else
        {
            messageText.text = message;
        }
        base.Open();
    }

    public override void Close()
    {
        base.Close();
    }
}

public enum TempDialogType
{
    OK,
    YesOrNo,
    InButtonMessage,//ボタンの方にメッセージを表示させる
}