using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 引き戸（スライドする）ドア
/// </summary>
public class SlideDoor : DoorObject
{
    public enum SlideDirection
    {
        X,Y,Z,MX,MY,MZ,
    }
    [SerializeField] private SlideDirection openSlideDirection = SlideDirection.X;
    [SerializeField] private float slideDistance = 2f;
    [SerializeField] private Transform moveTransform = null;
    private float moveTime = 1f;

    protected override void OpenAction()
    {
        if (!isMoving && !isOpenState)
        {
            isOpenState = true;
            StartCoroutine(DoorSlide(true, ()=> { StartCoroutine(DoorCloseWithWateTime()); }));
        }
    }

    public override void CloseDoor()
    {
        if (!isMoving && isOpenState)
        {
            isOpenState = false;
            StartCoroutine(DoorSlide(false));
        }
    }

    private IEnumerator DoorSlide(bool isOpen, UnityAction onComplete = null)
    {
        isMoving = true;
        float endTime = 0.5f;
        float currentTime = 0;
        Vector3 direction = Vector3.zero;
        switch (openSlideDirection)
        {
            case SlideDirection.X: direction.x = 1; break;
            case SlideDirection.Y: direction.y = 1; break;
            case SlideDirection.Z: direction.z = 1; break;
            case SlideDirection.MX: direction.x = -1; break;
            case SlideDirection.MY: direction.y = -1; break;
            case SlideDirection.MZ: direction.z = -1; break;
        }
        if (!isOpen)
        {
            direction = -direction;
        }
        Vector3 targetPosition = moveTransform.position + direction * slideDistance;
        float t = 0;
        while (t < 1)
        {
            t = currentTime / moveTime;
            moveTransform.position = Vector3.Lerp(moveTransform.position, targetPosition, t);
            currentTime += Time.deltaTime;
            yield return null;
        }
        isMoving = false;
        isOpenState = isOpen;
        if(onComplete != null)
        {
            onComplete();
        }
    }
}
