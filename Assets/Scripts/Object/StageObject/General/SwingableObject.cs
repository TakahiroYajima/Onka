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
            StartCoroutine(DoSwing(!isOpenState, () => { ClosedCallback.Invoke(); }));
        }
    }

    private IEnumerator DoSwing(bool open, UnityAction onComplete = null)
    {
        isMoving = true;
        //thisCollider.enabled = !open;
        float endTime = 0.5f;
        float currentTime = 0;
        float initAngale = currentLocalRotation;//swingTransform.localRotation.y;
        float angle = initAngale;
        Debug.Log("start : " + swingTransform.localRotation.y);
        const float swingAngle = 90f;

        UnityAction updateActin = null;
        if (open)
        {
            if (swingDirection == SwingDirection.Left)
            {
                updateActin = () => { angle = initAngale + (currentTime / endTime * swingAngle); };//90度でオープン完了
            }
            else
            {
                updateActin = () => { angle = initAngale + (currentTime / endTime * -swingAngle); };//-90度でオープン完了
            }
        }
        else
        {
            if (swingDirection == SwingDirection.Left)
            {
                updateActin = () => { angle = initAngale - (currentTime / endTime * swingAngle); };//0度でクローズ完了
            }
            else
            {
                updateActin = () => { angle = initAngale - (currentTime / endTime * -swingAngle); };//0度でクローズ完了
            }
        }
        while (currentTime < endTime)
        {
            updateActin();
            swingTransform.localRotation = Quaternion.Euler(0, angle, 0);
            currentTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("final : " + swingTransform.localRotation.y);
        currentLocalRotation = angle;
        isMoving = false;
        isOpenState = open;
        if(onComplete != null)
        {
            onComplete();
        }
    }
}

public enum SwingDirection
{
    Left,
    Right
}