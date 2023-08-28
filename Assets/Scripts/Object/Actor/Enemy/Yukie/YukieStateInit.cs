using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YukieStateInit : StateBase
{
    private Enemy_Yukie yukie = null;

    public YukieStateInit(Enemy_Yukie _yukie)
    {
        yukie = _yukie;
    }

    public override void StartAction()
    {
        yukie.navMeshAgent.enabled = false;
        yukie.wanderingActor.SetActive(false);
        yukie.inRoomWanderingActor.SetActive(false, null);
        yukie.onPlayerEnterCallback = null;
        yukie.onPlayerStayCallback = null;
    }

    public override void UpdateAction()
    {

    }

    public override void EndAction()
    {

    }
}
