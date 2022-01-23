using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 開閉するオブジェクトの挙動用コンポーネント
/// アタッチするオブジェクトは必ずダイナミックにすること（インスペクタのStaticのチェックを外す）
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class SwingableObject : MonoBehaviour
{
    [SerializeField] private SwingDirection swingDirection;
    [SerializeField] private UnityEvent OpenedCallback = null;
    [SerializeField] private UnityEvent ClosedCallback = null;

    private Transform swingTransform = null;
    private Collider thisCollider = null;

    public bool isMoving { get; private set; } = false;
    public bool isOpenState { get; private set; } = false;

    private float currentLocalRotation = 0f;

    private void Start()
    {
        swingTransform = GetComponent<Transform>();
        thisCollider = GetComponent<Collider>();
        if (OpenedCallback == null)
        {
            OpenedCallback = new UnityEvent();
        }
        if (ClosedCallback == null)
        {
            ClosedCallback = new UnityEvent();
        }
        currentLocalRotation = swingTransform.localRotation.y;
    }

    public void Swing()
    {
        if (!isMoving)
        {
            if (isOpenState) { ClosedCallback.Invoke(); StartCoroutine(DoSwing(false, () => {  })); }//本当は閉鎖前のコールバックを作るべきだった
            else { StartCoroutine(DoSwing(true, () => { OpenedCallback.Invoke(); })); }
        }
    }

    private IEnumerator DoSwing(bool _open, UnityAction _onComplete = null)
    {
        isMoving = true;
        float endTime = 0.5f;
        float currentTime = 0;
        float initAngale = currentLocalRotation;
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