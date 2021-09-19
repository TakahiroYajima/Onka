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
        player = StageManager.Instance.GetPlayer();
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
            case "StageObject":
                hit.transform.GetComponent<StageObjectBase>().OnTapObject();
                break;
            case "Door":
                //DataManager.Instance.DoDoorUnlock(hit.transform.gameObject.GetComponent<DoorObject>().DoorOpenKey);
                break;
            case "KeyLock":
                hit.transform.GetComponent<DoorKeyLockObject>().DoUnlockDoorKey();
                break;
            case "Item":
                Debug.Log("アイテム取得 : " + hit.transform.name);
                ItemObject itemObject = hit.transform.gameObject.GetComponent<ItemObject>();
                ItemManager.Instance.ItemGetAction(itemObject);
                player.ChangeState(PlayerState.ItemGet);
                break;
        }
    }
}
