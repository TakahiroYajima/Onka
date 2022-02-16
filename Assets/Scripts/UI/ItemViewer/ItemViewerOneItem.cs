using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using SoundSystem;

public class ItemViewerOneItem : MonoBehaviour
{
    [SerializeField] private Button itemButton = null;
    [SerializeField] private Image itemImage = null;
    [SerializeField] private Text itemText = null;

    public void Initialize(Sprite itemSprite, string itemName, UnityAction onPress = null)
    {
        itemImage.sprite = itemSprite;
        itemText.text = itemName;
        itemButton.onClick.RemoveAllListeners();
        if (onPress != null)
        {
            itemButton.onClick.AddListener(onPress);
            itemButton.onClick.AddListener(() => { SoundManager.Instance.PlaySeWithKey("menuse_book_page"); });
            itemButton.transition = Selectable.Transition.ColorTint;
        }
        else
        {
            //ボタンのコールバックが無ければただのビューとして扱うため、ボタンの色変化を無くす
            itemButton.transition = Selectable.Transition.None;
        }
    }
}
