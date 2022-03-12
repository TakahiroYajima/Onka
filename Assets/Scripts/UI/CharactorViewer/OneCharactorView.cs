using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class OneCharactorView : MonoBehaviour
{
    [SerializeField] private GameObject baseObject = null;
    [SerializeField] private GameObject nameTitleObj = null;
    [SerializeField] private Text nameText = null;
    [SerializeField] private Text ageText = null;
    [SerializeField] private Text genderText = null;
    [SerializeField] private Text descriptionText = null;
    [SerializeField] private Image charactorImage = null;
    [SerializeField] private Button backButton = null;
    public UnityAction onClosed = null;

    public void View(CharactorData _data, bool _showBackButton = false, UnityAction _onClosed = null)
    {
        nameTitleObj.SetActive(_data.isCharactorName);
        nameText.text = _data.name;
        ageText.text = _data.age.ToString();
        genderText.text = EnumUtil.PerseGenderStr(_data.gender);
        descriptionText.text = _data.description;
        //descriptionText.gameObject.GetComponent<HyphenationJpn>().GetText(_data.description);
        charactorImage.sprite = _data.sprite;

        onClosed = _onClosed;
        backButton.gameObject.SetActive(_showBackButton);
        baseObject.SetActive(true);
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
