using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateInMenu : StateBase
{
    private PlayerObject player = null;
    public PlayerStateInMenu(PlayerObject _player)
    {
        player = _player;
    }

    public override void StartAction()
    {
        player.ForcedStopFPS();
    }

    public override void UpdateAction()
    {
        player.ForcedStopFPS();
    }

    public override void EndAction()
    {
        player.StartActiveFPS();
    }
}
