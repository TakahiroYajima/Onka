using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateInit : StateBase
{
    private PlayerObject player = null;
    public override void StartAction()
    {
        player = StageManager.Instance.Player;
        player.FirstPersonAIO.enabled = false;

    }

    public override void UpdateAction()
    {

    }

    public override void EndAction()
    {
        
    }
}
