using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateInit : StateBase
{
    private PlayerObject player = null;
    public PlayerStateInit(PlayerObject _player)
    {
        player = _player;
    }

    public override void StartAction()
    {
        player.FirstPersonAIO.isEnable = false;

    }

    public override void UpdateAction()
    {

    }

    public override void EndAction()
    {
        
    }
}
