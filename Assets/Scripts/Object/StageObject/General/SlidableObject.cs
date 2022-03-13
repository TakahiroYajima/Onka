using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SoundSystem;

public class SlidableObject : MonoBehaviour
{
    public enum SlideDirection
    {
        X, Y, Z, MX, MY, MZ,
    }
    [SerializeField] private SlideDirection openSlideDirection = SlideDirection.X;
    [SerializeField] private float slideDistance = 2f;

    [SerializeField] private UnityEvent OpenedCallback = null;
    [SerializeField] private UnityEvent ClosedCallback = null;

    private Transform moveTransform = null;
    private float moveTime = 1f;
    public bool isMoving { get; private set; } = false;
    public bool isOpenState { get; private set; } = false;//開いている状態か

    [HideInInspector] private string openSEKey = "se_slideobj_open";
    [HideInInspector] public string closeSEKey = "se_slideobj_open";

    private void Start()
    {
        moveTransform = GetComponent<Transform>();
        if(OpenedCallback == null)
        {
            OpenedCallback = new UnityEvent();
        }
        if(ClosedCallback == null)
        {
            ClosedCallback = new UnityEvent();
        }
    }

    public void Slide()
    {
        if (!isMoving)
        {
            if (isOpenState)
            {
                isOpenState = false;
                SoundManager.Instance.PlaySeWithKeyOne(closeSEKey);
                StartCoroutine(DoSlide(false, () => { ClosedCallback.Invoke(); }));
            }
            else
            {
                isOpenState = true;
                SoundManager.Instance.PlaySeWithKeyOne(openSEKey);
                StartCoroutine(DoSlide(true, () => { OpenedCallback.Invoke(); }));
            }
        }
    }

    private IEnumerator DoSlide(bool isOpen, UnityAction onComplete = null)
    {
        isMoving = true;
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
        if (onComplete != null)
        {
            onComplete();
        }
    }
}
