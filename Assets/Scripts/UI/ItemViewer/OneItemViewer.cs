﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class OneItemViewer : MonoBehaviour
{
    [SerializeField] private GameObject baseObject = null;
    [SerializeField] private Image itemImage = null;
    [SerializeField] private Button detailButton = null;
    [SerializeField] private Text itemText = null;
    [SerializeField] private Text contentText = null;
    [SerializeField] private Button backButton = null;
    public UnityAction onClosed = null;

    public void ViewItem(Sprite _itemSprite, string _itemName, string _content = "", bool _showBackButton = false, UnityAction _onClosed = null, UnityAction _onPressDetailButton = null)
    {
        itemImage.sprite = _itemSprite;
        itemText.text = _itemName;
        contentText.text = _content;
        onClosed = _onClosed;
        backButton.gameObject.SetActive(_showBackButton);
        if(_onPressDetailButton != null)
        {
            detailButton.onClick.AddListener(_onPressDetailButton);
            detailButton.gameObject.SetActive(true);
        }
        else
        {
            detailButton.gameObject.SetActive(false);
        }
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
