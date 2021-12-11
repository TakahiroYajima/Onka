using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateEvent : StateBase
{
    private PlayerObject player = null;
    public override void StartAction()
    {
        player = StageManager.Instance.Player;
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