using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateWatchItem : StateBase
{
    private PlayerObject player = null;
    public override void StartAction()
    {
        player = StageManager.Instance.GetPlayer();
        player.FirstPersonAIO.enabled = false;
    }

    public override void UpdateAction()
    {
        
    }

    public override void EndAction()
    {
        player.FirstPersonAIO.enabled = true;
    }
}
