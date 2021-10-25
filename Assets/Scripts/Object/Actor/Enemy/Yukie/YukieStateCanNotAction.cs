using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YukieStateCanNotAction : StateBase
{
    private Enemy_Yukie yukie = null;


    public override void StartAction()
    {
        yukie = StageManager.Instance.Yukie;
        yukie.navMeshAgent.enabled = false;
    }

    public override void UpdateAction()
    {

    }

    public override void EndAction()
    {
        
    }
}
