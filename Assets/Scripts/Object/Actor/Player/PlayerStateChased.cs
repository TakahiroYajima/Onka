using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateChased : StateBase
{
    private PlayerObject player = null;

    public override void StartAction()
    {
        player = StageManager.Instance.Player;
    }

    public override void UpdateAction()
    {
        
    }

    public override void EndAction()
    {

    }
}
