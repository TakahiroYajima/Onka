using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの自由行動時の処理
/// </summary>
public class PlayerStateFree : StateBase
{
    private PlayerObject player = null;
    private int frameCount = 0;

    public PlayerStateFree(PlayerObject _player)
    {
        player = _player;
    }

    public override void StartAction()
    {
        player.FirstPersonAIO.enabled = true;
        CrosshairManager.Instance.FindCrosshair();
        frameCount = 0;
    }

    public override void UpdateAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            player.raycastor.ScreenToRayActionWithLayerMask(LayerMaskData.FromPlayerRayMask, ClickAction);
            return;
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

    public void SerchRay(RaycastHit hit)
    {
        //Debug.Log("Serch : " + LayerMask.LayerToName(hit.transform.gameObject.layer) + " : " + hit.transform.gameObject.name);
        switch (hit.transform.tag)
        {
            case Tags.StageObject:
            case Tags.SaveObject:
            case Tags.Item:
            case Tags.KeyLock:
                CrosshairManager.Instance.ChangeCenterSprites(CrosshairType.Tapable);
                break;
            //case Tags.Door:
            //    CrosshairManager.Instance.ChangeCenterSprites(CrosshairType.Door);
            //    break;
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

    /// <summary>
    /// 画面をクリックしたときの挙動
    /// </summary>
    /// <param name="hit"></param>
    public void ClickAction(RaycastHit hit)
    {
        //Debug.Log("playerRaycastHit : " + hit.transform.name);
        switch (hit.transform.tag)
        {
            case Tags.StageObject:
                hit.transform.GetComponent<StageObjectBase>().OnTapObject();
                break;
            case Tags.Door:
                //DataManager.Instance.DoDoorUnlock(hit.transform.gameObject.GetComponent<DoorObject>().DoorOpenKey);
                break;
            case Tags.KeyHole:
                //Debug.Log("KeyHole : " + hit.transform.parent.GetComponent<KeyHoleTarget>().UnlockKey);
                hit.transform.GetComponent<KeyHoleObject>().DoUnlock();
                break;
            case Tags.KeyLock:
                hit.transform.GetComponent<KeyLockObject>().TapObject();
                break;
            case Tags.Item:
                ItemObject itemObject = hit.transform.gameObject.GetComponent<ItemObject>();
                ItemManager.Instance.ItemGetAction(itemObject, GetItemAction);
                
                break;
            case Tags.SaveObject:
                StageManager.Instance.StartSaveAction();
                break;
        }
    }

    private void GetItemAction(bool _isSuccess)
    {
        if (_isSuccess)
        {
            player.ChangeState(PlayerState.ItemGet);
        }
    }
}
