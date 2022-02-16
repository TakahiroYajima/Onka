using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using SoundSystem;

public class CharactorListViewerOneItem : MonoBehaviour
{
    [SerializeField] private Button charactorButton = null;
    [SerializeField] private Image charactorImage = null;
    [SerializeField] private Text nameText = null;

    public void Initialize(CharactorData data, UnityAction onPress = null)
    {
        charactorImage.sprite = data.sprite;
        nameText.text = data.name;
        charactorButton.onClick.RemoveAllListeners();
        if (onPress != null)
        {
            charactorButton.onClick.AddListener(onPress);
            charactorButton.onClick.AddListener(() => { SoundManager.Instance.PlaySeWithKey("menuse_book_page"); });
            charactorButton.transition = Selectable.Transition.ColorTint;
        }
        else
        {
            //ボタンのコールバックが無ければただのビューとして扱うため、ボタンの色変化を無くす
            charactorButton.transition = Selectable.Transition.None;
        }
    }
}
