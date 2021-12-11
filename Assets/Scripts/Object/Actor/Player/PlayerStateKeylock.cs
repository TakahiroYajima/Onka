using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateKeylock : StateBase
{
    private PlayerObject player = null;
    GameObject Crosshair = null;
    public override void StartAction()
    {
        player = StageManager.Instance.Player;
        player.ForcedStopFPS();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (Crosshair == null)
        {
            Crosshair = GameObject.Find("Crosshair");
        }
        if(Crosshair != null)
        {
            Crosshair.SetActive(false);
        }
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (Crosshair != null)
        {
            Crosshair.SetActive(true);
        }
    }

    private void RayOperation(RaycastHit hit)
    {
        if(Utility.Instance.IsTagNameMatch(hit.transform.gameObject, Tags.KeyLockTouchable))
        {
            hit.transform.GetComponent<KeyLockOperationObject>().Tap();
        }
    }
}
