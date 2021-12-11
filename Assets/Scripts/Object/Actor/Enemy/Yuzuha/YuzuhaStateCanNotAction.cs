using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YuzuhaStateCanNotAction : StateBase
{
    private Enemy_Yuzuha yuzuha = null;

    public override void StartAction()
    {
        yuzuha = StageManager.Instance.Yuzuha;
        yuzuha.navMeshAgent.enabled = false;
    }

    public override void UpdateAction()
    {

    }

    public override void EndAction()
    {
        
    }
}
