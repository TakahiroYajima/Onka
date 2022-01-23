using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateKeylock : StateBase
{
    private PlayerObject player = null;
    public override void StartAction()
    {
        player = StageManager.Instance.Player;
        player.ForcedStopFPS();
        InGameUtil.DoCursorFree();
        if (CrosshairManager.Instance.Crosshair == null)
        {
            CrosshairManager.Instance.FindCrosshair();
        }
        CrosshairManager.Instance.SetCrosshairActive(false);
    }

    public override void UpdateAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            player.raycastor.ScreenToRayAction(RayOperation);
        }
    }

    public override void EndAction()
    {
        player.FirstPersonAIO.enabled = true;
        InGameUtil.DoCursorLock();
        CrosshairManager.Instance.SetCrosshairActive(true);
    }

    private void RayOperation(RaycastHit hit)
    {
        if(Utility.Instance.IsTagNameMatch(hit.transform.gameObject, Tags.KeyLockTouchable))
        {
            hit.transform.GetComponent<KeyLockOperationObject>().Tap();
        }
    }
}
