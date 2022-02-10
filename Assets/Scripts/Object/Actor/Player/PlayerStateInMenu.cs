using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateInMenu : StateBase
{
    private PlayerObject player = null;
    public override void StartAction()
    {
        player = StageManager.Instance.Player;
        player.ForcedStopFPS();
        player.FirstPersonAIO.enabled = false;
    }

    public override void UpdateAction()
    {
        player.ForcedStopFPS();
    }

    public override void EndAction()
    {
        player.FirstPersonAIO.enabled = true;
    }
}
