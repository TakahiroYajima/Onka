using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TemplateDialog : DialogBase
{
    [SerializeField] private Text messageText = null;
    [SerializeField] private Button yesButton = null;
    [SerializeField] private Button noButton = null;

    public void Open(string message, TempDialogType tempDialogType, UnityAction<bool> pressButtonCallback)
    {
        messageText.text = message;
        yesButton.onClick.AddListener(() => { pressButtonCallback(true); });
        noButton.onClick.AddListener(() => { pressButtonCallback(false); });
        if(tempDialogType == TempDialogType.OK)
        {
            noButton.gameObject.SetActive(false);
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
}