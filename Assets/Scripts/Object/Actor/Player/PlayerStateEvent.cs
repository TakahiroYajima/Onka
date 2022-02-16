using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateEvent : StateBase
{
    private PlayerObject player = null;
    public PlayerStateEvent(PlayerObject _player)
    {
        player = _player;
    }

    public override void StartAction()
    {
        player.ForcedStopFPS();
    }

    public override void UpdateAction()
    {

    }

    public override void EndAction()
    {
        player.FirstPersonAIO.enabled = true;
    }
}