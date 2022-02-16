using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YukieStateCanNotAction : StateBase
{
    private Enemy_Yukie yukie = null;

    public YukieStateCanNotAction(Enemy_Yukie _yukie)
    {
        yukie = _yukie;
    }

    public override void StartAction()
    {
        yukie.navMeshAgent.enabled = false;
    }

    public override void UpdateAction()
    {

    }

    public override void EndAction()
    {
        
    }
}
