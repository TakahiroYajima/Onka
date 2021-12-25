using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YukieStateInit : StateBase
{
    private Enemy_Yukie yukie = null;


    public override void StartAction()
    {
        yukie = StageManager.Instance.Yukie;
        yukie.navMeshAgent.enabled = false;
        yukie.wanderingActor.SetActive(false);
        yukie.inRoomWanderingActor.SetActive(false, null);
        yukie.onColliderEnterCallback = null;
        yukie.onColliderStayCallback = null;
    }

    public override void UpdateAction()
    {

    }

    public override void EndAction()
    {

    }
}
