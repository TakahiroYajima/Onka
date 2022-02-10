using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵に追いかけられている間のステート
/// </summary>
public class PlayerStateChased : StateBase
{
    private PlayerObject player = null;
    private int frameCount = 0;

    public override void StartAction()
    {
        player = StageManager.Instance.Player;
        CrosshairManager.Instance.ChangeCenterSprites(CrosshairType.Normal);
    }

    public override void UpdateAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            player.raycastor.ScreenToRayActionWithLayerMask(LayerMaskData.FromPlayerRayMask, ClickAction);
        }
        if (frameCount >= 6)
        {
            player.raycastor.ScreenToRayActionWithLayerMask(LayerMaskData.FromPlayerRayMask, SerchRay, MissSerchRay);
            frameCount = 0;
        }
        else { frameCount++; }
    }

    public override void EndAction()
    {

    }

    /// <summary>
    /// 画面をクリックしたときの挙動
    /// </summary>
    /// <param name="hit"></param>
    public void ClickAction(RaycastHit hit)
    {
        switch (hit.transform.tag)
        {
            case Tags.StageObject:
                hit.transform.GetComponent<StageObjectBase>().OnTapObject();
                break;
            case Tags.KeyHole:
                hit.transform.GetComponent<KeyHoleObject>().DoUnlock();
                break;
        }
    }
    public void SerchRay(RaycastHit hit)
    {
        //Debug.Log("Serch : " + hit.transform.tag);
        switch (hit.transform.tag)
        {
            case Tags.StageObject:
                CrosshairManager.Instance.ChangeCenterSprites(CrosshairType.Tapable);
                break;
            case Tags.KeyHole:
                CrosshairManager.Instance.ChangeCenterSprites(CrosshairType.DoorKey);
                break;
            default:
                CrosshairManager.Instance.ChangeCenterSprites(CrosshairType.Normal);
                break;
        }
    }
    private void MissSerchRay()
    {
        CrosshairManager.Instance.ChangeCenterSprites(CrosshairType.Normal);
    }
}
