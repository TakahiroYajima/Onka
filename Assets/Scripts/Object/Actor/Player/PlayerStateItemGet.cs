using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Onka.Manager.Event;

public class PlayerStateItemGet : StateBase
{
    private PlayerObject player = null;

    private float timeCount = 0f;
    private const float waitTime = 1f;

    public PlayerStateItemGet(PlayerObject _player)
    {
        player = _player;
    }

    public override void StartAction()
    {
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
            StageManager.Instance.OnEndPlayerWatchItem();
            EventManager.Instance.ProgressEvent();
        };
        timeCount = 0f;
        StageManager.Instance.OnStartPlayerWatchItem();
    }

    public override void UpdateAction()
    {
        player.ForcedStopFPS();
    }

    public override void EndAction()
    {
        player.StartActiveFPS();
        player.onStateChangedInPlayerScriptOnly = null;
    }
}
