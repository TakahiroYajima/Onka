using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵に追いかけられている間のステート
/// </summary>
public class PlayerStateChased : StateBase
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
}
