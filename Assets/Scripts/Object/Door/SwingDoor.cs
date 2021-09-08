using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 開き戸のドア
/// </summary>
public class SwingDoor : DoorObject
{
    //[SerializeField] private Transform rotateTrans = null;
    [SerializeField] private Animation doorAnimation = null;
    [SerializeField] private AnimationClip openAnim = null;
    [SerializeField] private AnimationClip closeAnim = null;

    protected override void OpenAction()
    {
        if (!isMoving && !isOpenState)
        {
            isOpenState = true;
            thisCollider.enabled = false;
            doorAnimation.clip = openAnim;
            doorAnimation.Play();
            StartCoroutine(DoorCloseWithWateTime());
        }
    }

    public override void CloseDoor()
    {
        if (!isMoving && isOpenState)
        {
            isOpenState = false;
            thisCollider.enabled = true;
            doorAnimation.clip = closeAnim;
            doorAnimation.Play();
        }
    }

    //private IEnumerator RotationDoor(bool open)
    //{
    //    isMoving = true;
    //    thisCollider.enabled = !open;
    //    float endTime = 0.5f;
    //    float currentTime = 0;
    //    float angle = rotateTrans.rotation.y;
    //    while (currentTime < endTime)
    //    {
    //        if (open)
    //        {
    //            angle = currentTime / endTime * -90f;//-90度でオープン完了
    //            Debug.Log(angle);
    //        }
    //        else
    //        {
    //            angle = -90f + currentTime / endTime * 90f;//0度でクローズ完了
    //        }
    //        rotateTrans.rotation = Quaternion.Euler(0, angle, 0);
    //        currentTime += Time.deltaTime;
    //        yield return null;
    //    }
    //    isMoving = false;
    //    isOpenState = open;
    //}
}
