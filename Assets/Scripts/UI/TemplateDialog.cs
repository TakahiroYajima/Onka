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
        var yesMaster = tempDialogType == TempDialogType.YesOrNo_LanguageReverce ? "text_yes_reverce_language" : "text_yes";
        var noMaster = tempDialogType == TempDialogType.YesOrNo_LanguageReverce ? "text_no_reverce_language" : "text_no";
        yesButton.text.text = TextMaster.GetText(yesMaster);
        noButton.text.text = TextMaster.GetText(noMaster);
        yesButton.button.onClick.AddListener(() => { pressButtonCallback(true); });
        noButton.button.onClick.AddListener(() => { pressButtonCallback(false); });
        if(tempDialogType != TempDialogType.YesOrNo && tempDialogType != TempDialogType.YesOrNo_LanguageReverce)
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
    YesOrNo_LanguageReverce,//言語反転のYesOrNo
    InButtonMessage,//ボタンの方にメッセージを表示させる
}