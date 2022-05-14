using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UpdateFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage = null;

    private bool isStarted = false;
    private float fadeTime = 1f;

    private FadeType fadeType = FadeType.In;
    private UnityAction onCompleted = null;

    private Color initColor;
    private Color updateColor;
    private float currentTime = 0f;

    public void FadeStart(FadeType _fadeType, float _fadeTime = 1f, UnityAction _onCompleted = null)
    {
        initColor = fadeImage.color;
        if (_fadeType == FadeType.In)
        {
            initColor.a = 0f;
        }
        else
        {
            initColor.a = 1f;
        }
        fadeImage.color = initColor;
        fadeType = _fadeType;
        fadeTime = _fadeTime;
        currentTime = 0f;
        onCompleted = _onCompleted;

        isStarted = true;
    }
    public void FadeCansel()
    {
        currentTime = 0f;
        isStarted = false;
        onCompleted = null;
    }
    public void InitColor()
    {
        initColor = fadeImage.color;
        initColor.a = 0f;
        fadeImage.color = initColor;
    }

    public void SetSprite(Sprite sprite, bool isSetNativeSize = false)
    {
        fadeImage.sprite = sprite;
        if (isSetNativeSize)
        {
            fadeImage.SetNativeSize();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStarted) return;
        
        updateColor = fadeImage.color;
        float dir = Time.deltaTime / fadeTime;
        if (fadeType == FadeType.In)
        {
            updateColor.a += dir;
        }
        else
        {
            updateColor.a -= dir;
        }
        fadeImage.color = updateColor;
        currentTime += dir;
        if (currentTime >= 1f)
        {
            if (onCompleted != null)
            {
                onCompleted();
            }
            isStarted = false;
        }
    }

    public void SetActive(bool _isActive)
    {
        fadeImage.gameObject.SetActive(_isActive);
    }
}
