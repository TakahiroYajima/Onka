using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateWatchItem : StateBase
{
    private PlayerObject player = null;
    public PlayerStateWatchItem(PlayerObject _player)
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
        player.StartActiveFPS();
    }
}
