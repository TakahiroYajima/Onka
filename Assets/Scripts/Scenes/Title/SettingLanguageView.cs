using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingLanguageView : MonoBehaviour
{
    [SerializeField] private Text titleText = null;
    [SerializeField] private TextButton japaneseButton;
    [SerializeField] private TextButton englishButton;
    public Action onChangeLanguage = null;
    public Action onClose = null;

    public void Initialize()
    {
        titleText.text = TextMaster.GetText("text_language_setting_title");
        SetSelect(TextMaster.CurrentLanguage);
    }

    private void SetSelect(Language select)
    {
        japaneseButton.button.interactable = select != Language.Ja;
        englishButton.button.interactable = select != Language.En;
    }

    public void OnClickJapanese()
    {
        DialogManager.Instance.OpenTemplateDialog("日本語に設定しますか？", TempDialogType.YesOrNo_LanguageReverce, (result) =>
        {
            GameManager.Instance.ChangeLanguage(Language.Ja);
            if (result)
            {
                OnChangeLanguage();
            }
        });
    }

    public void OnClickEnglish()
    {
        DialogManager.Instance.OpenTemplateDialog("Do you want to set it to English?", TempDialogType.YesOrNo_LanguageReverce, (result) =>
        {
            GameManager.Instance.ChangeLanguage(Language.En);
            if (result)
            {
                OnChangeLanguage();
            }
        });
    }

    private void OnChangeLanguage()
    {
        if(onChangeLanguage != null)
        {
            onChangeLanguage();
        }
        SetSelect(TextMaster.CurrentLanguage);
        OnClickBackButton();
    }

    public void OnClickBackButton()
    {
        if (onClose != null)
        {
            onClose();
        }
    }
}
