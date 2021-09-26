using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの自由行動時の処理
/// </summary>
public class PlayerStateFree : StateBase
{
    private PlayerObject player = null;
    public override void StartAction()
    {
        player = StageManager.Instance.Player;
    }

    public override void UpdateAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            player.raycastor.ScreenToRayAction(ClickAction);
        }
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
        //Debug.Log("playerRaycastHit : " + hit.transform.name);
        switch (hit.transform.tag)
        {
            case Tags.StageObject:
                hit.transform.GetComponent<StageObjectBase>().OnTapObject();
                break;
            case Tags.Door:
                //DataManager.Instance.DoDoorUnlock(hit.transform.gameObject.GetComponent<DoorObject>().DoorOpenKey);
                break;
            case Tags.KeyLock:
                //hit.transform.GetComponent<DoorKeyLockObject>().DoUnlockDoorKey();
                hit.transform.GetComponent<KeyLockObject>().DoUnlock();
                break;
            case Tags.Item:
                ItemObject itemObject = hit.transform.gameObject.GetComponent<ItemObject>();
                ItemManager.Instance.ItemGetAction(itemObject);
                player.ChangeState(PlayerState.ItemGet);
                player.rigidbody.velocity = Vector3.zero;
                player.rigidbody.angularVelocity = Vector3.zero;
                break;
        }
    }
}
