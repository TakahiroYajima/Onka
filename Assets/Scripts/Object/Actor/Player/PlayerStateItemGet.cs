using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Onka.Manager.Event;

public class PlayerStateItemGet : StateBase
{
    private PlayerObject player = null;

    private float timeCount = 0f;
    private const float waitTime = 1f;

    public override void StartAction()
    {
        player = StageManager.Instance.Player;
        player.ForcedStopFPS();
        //player.onStateChangedInPlayerScriptOnly = () =>
        //{
        //    EventManager.Instance.ProgressEvent();
        //};
        ItemManager.Instance.watchItemEventEndedCallback = () =>
        {
            if (player.currentState == PlayerState.ItemGet)
            {
                player.ChangeState(PlayerState.Free);
            }
            EventManager.Instance.ProgressEvent();
        };
        timeCount = 0f;
    }

    public override void UpdateAction()
    {
        //if(timeCount <= waitTime)
        //{
        //    timeCount += Time.deltaTime;
        //}
        //else
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        ItemManager.Instance.FinishWatchItem();
        //        player.ChangeState(PlayerState.Free);
        //    }
        //}
    }

    public override void EndAction()
    {
        player.FirstPersonAIO.enabled = true;
        player.onStateChangedInPlayerScriptOnly = null;
    }
}
