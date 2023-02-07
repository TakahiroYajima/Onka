using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FadeManager : SingletonMonoBehaviour<FadeManager> {
    public enum FadeColorType {
        None = -1,
        Black,
        White,
    }
    public const float DefaultDuration = 0.5f;

    [SerializeField] private Image _image;

    void Start() {
        this._image.enabled = false;
    }

    private readonly Dictionary<FadeColorType, Color> _colorDict = new Dictionary<FadeColorType, Color>() {
        {FadeColorType.None, Color.black },
        {FadeColorType.Black, Color.black },
        {FadeColorType.White, Color.white },
    };

    public void FadeIn(FadeColorType colorType = FadeColorType.Black, float duration = DefaultDuration, Action onComplete = null) {
        if (!this._colorDict.ContainsKey(colorType)) {
            return;
        }
        if (colorType == FadeColorType.None) {
            if (onComplete != null)
            {
                onComplete();
            }
            return;
        }
        this._image.enabled = true;
        this._image.color = this._colorDict[colorType];
        StartCoroutine(FadeAction(_image, FadeType.In, 1f, () =>
        {
            this._image.enabled = false;
            if (onComplete != null)
            {
                onComplete();
            }
        }));
    }

    public void FadeOut(FadeColorType colorType = FadeColorType.Black, float duration = DefaultDuration, Action onComplete = null) {
        if (!this._colorDict.ContainsKey(colorType)) {
            return;
        }
        if (colorType == FadeColorType.None)
        {
            if (onComplete != null)
            {
                onComplete();
            }
            return;
        }
        this._image.enabled = true;
        Color c = this._colorDict[colorType];
        this._image.color = new Color(c.r, c.g, c.b, 0);
        StartCoroutine(FadeAction(_image, FadeType.Out, 1f, () =>
        {
            if (onComplete != null)
            {
                onComplete();
            }
        }));
    }

    public IEnumerator FadeAction(Image _image, FadeType _type, float _duration, UnityAction onComplete = null)
    {
        Color color = _image.color;
        float currentTime = 0f;
        while(currentTime < _duration)
        {
            float dir = Time.deltaTime / _duration;
            if (_type == FadeType.In)
            {
                color.a -= dir;
            }
            else
            {
                color.a += dir;
            }
            _image.color = color;
            currentTime += dir;
            yield return null;
        }
        if(onComplete != null)
        {
            onComplete();
        }
    }
    public IEnumerator FadeAction(Text _text, FadeType _type, float _duration, UnityAction onComplete = null)
    {
        Color color = _text.color;
        float currentTime = 0f;
        while (currentTime < _duration)
        {
            float dir = Time.deltaTime / _duration;
            if (_type == FadeType.In)
            {
                color.a -= dir;
            }
            else
            {
                color.a += dir;
            }
            _text.color = color;
            currentTime += dir;
            yield return null;
        }
        if (onComplete != null)
        {
            onComplete();
        }
    }

    public void BlackOut()
    {
        Color c = this._colorDict[FadeColorType.Black];
        this._image.color = new Color(c.r, c.g, c.b, 1f);
        this._image.enabled = true;
    }

    public void HideFade() {
        this._image.enabled = false;
    }
}
public enum FadeType
{
    In,
    Out
}