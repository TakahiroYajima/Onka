using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class OneCharactorView : MonoBehaviour
{
    [SerializeField] private GameObject baseObject = null;
    [SerializeField] private Text nameTitleText = null;
    [SerializeField] private Text nameText = null;
    [SerializeField] private Text ageTitleText = null;
    [SerializeField] private Text ageText = null;
    [SerializeField] private Text genderTitleText = null;
    [SerializeField] private Text genderText = null;
    [SerializeField] private Text descriptionTitleText = null;
    [SerializeField] private Text descriptionText = null;
    [SerializeField] private Image charactorImage = null;
    [SerializeField] private TextButton backButton = null;
    public UnityAction onClosed = null;

    public void View(CharactorData _data, bool _showBackButton = false, UnityAction _onClosed = null)
    {
        SetTexts();
        nameTitleText.gameObject.SetActive(_data.isCharactorName);
        nameText.text = _data.Name;
        ageText.text = _data.age.ToString();
        genderText.text = EnumUtil.PerseGenderStr(_data.gender);
        descriptionText.text = _data.Description;
        //descriptionText.gameObject.GetComponent<HyphenationJpn>().GetText(_data.description);
        charactorImage.sprite = _data.sprite;

        onClosed = _onClosed;
        backButton.gameObject.SetActive(_showBackButton);
        baseObject.SetActive(true);
    }

    private void SetTexts()
    {
        nameTitleText.text = TextMaster.GetText("text_name");
        ageTitleText.text = TextMaster.GetText("text_age");
        genderTitleText.text = TextMaster.GetText("text_gender");
        descriptionTitleText.text = TextMaster.GetText("text_story");
        backButton.text.text = TextMaster.GetText("text_back");
    }

    public void CloseView()
    {
        baseObject.SetActive(false);
        if (onClosed != null)
        {
            onClosed();
        }
        Destroy(gameObject);
    }
}
