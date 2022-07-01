using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UpdateFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup = null;

    private bool isStarted = false;
    private float fadeTime = 1f;

    private FadeType fadeType = FadeType.In;
    private UnityAction onCompleted = null;

    //private Color initColor;
    //private Color updateColor;
    private float currentTime = 0f;

    public void FadeStart(FadeType _fadeType, float _fadeTime = 1f, UnityAction _onCompleted = null)
    {
        //initColor = canvasGroup.color;
        if (_fadeType == FadeType.In)
        {
            canvasGroup.alpha = 0f;
        }
        else
        {
            canvasGroup.alpha = 1f;
        }
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
        canvasGroup.alpha = 0f;
    }

    //public void SetSprite(Sprite sprite, bool isSetNativeSize = false)
    //{
    //    canvasGroup.sprite = sprite;
    //    if (isSetNativeSize)
    //    {
    //        canvasGroup.SetNativeSize();
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        if (!isStarted) return;
        
        //updateColor = canvasGroup.color;
        float dir = Time.deltaTime / fadeTime;
        if (fadeType == FadeType.In)
        {
            canvasGroup.alpha += dir;
        }
        else
        {
            canvasGroup.alpha -= dir;
        }
        //canvasGroup.color = updateColor;
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
        canvasGroup.gameObject.SetActive(_isActive);
    }
}
