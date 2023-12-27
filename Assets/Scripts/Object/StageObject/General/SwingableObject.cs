using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using SoundSystem;

/// <summary>
/// 開閉するオブジェクトの挙動用コンポーネント
/// アタッチするオブジェクトは必ずダイナミックにすること（インスペクタのStaticのチェックを外す）
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class SwingableObject : OpenableObjectBase
{
    [SerializeField] private SwingDirection swingDirection;
    [SerializeField] private UnityEvent OpenedCallback = null;
    [SerializeField] private UnityEvent ClosedCallback = null;
    
    private Transform swingTransform = null;

    public bool isMoving { get; private set; } = false;
    public bool isOpenState { get; private set; } = false;

    private float initLocalRotation = 0f;
    private float currentLocalRotation = 0f;
    private const string openSEKey = "se_closet_open";
    private const string closeSEKey = "se_closet_close";

    private void Start()
    {
        swingTransform = GetComponent<Transform>();
        initLocalRotation = currentLocalRotation = swingTransform.localRotation.y;
    }

    public void Swing()
    {
        if (!isMoving)
        {
            if (isOpenState) { onClosed?.Invoke(); StartCoroutine(DoSwing(false, () => { SoundManager.Instance.PlaySeWithKeyOne(closeSEKey); })); }//本当は閉鎖前のコールバックを作るべきだった
            else { SoundManager.Instance.PlaySeWithKeyOne(openSEKey); StartCoroutine(DoSwing(true, () => { onOpened?.Invoke(); })); }
        }
    }

    private IEnumerator DoSwing(bool _open, UnityAction _onComplete = null)
    {
        isMoving = true;
        float endTime = 0.5f;
        float currentTime = 0;
        float initAngale = _open ? initLocalRotation : currentLocalRotation;
        float angle = initAngale;
        float swingAngle = 90f;
        if(swingDirection == SwingDirection.Right || swingDirection == SwingDirection.Up) { swingAngle = -swingAngle; }

        UnityAction updateAction = null;
        if (_open)
        {
            updateAction = () => { angle = initAngale + (currentTime / endTime * swingAngle); };
        }
        else
        {
            updateAction = () => { angle = initAngale - (currentTime / endTime * swingAngle); };
        }

        UnityAction setRotationAction = null;
        if (swingDirection == SwingDirection.Right || swingDirection == SwingDirection.Left) {
            setRotationAction = () => { swingTransform.localRotation = Quaternion.Euler(0, angle, 0); };
        }
        else
        {
            setRotationAction = () => { swingTransform.localRotation = Quaternion.Euler(angle, 0, 0); };
        }
        while (currentTime < endTime)
        {
            updateAction();
            setRotationAction();
            currentTime += Time.deltaTime;
            yield return null;
        }
        currentLocalRotation = angle;
        isMoving = false;
        isOpenState = _open;
        if(_onComplete != null)
        {
            _onComplete();
        }
    }
}

public enum SwingDirection
{
    Left,
    Right,
    Up,
    Down,
}