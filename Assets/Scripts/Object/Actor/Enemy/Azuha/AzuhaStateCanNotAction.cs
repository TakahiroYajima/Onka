using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AzuhaStateCanNotAction : StateBase
{
    private Enemy_Azuha azuha = null;

    public override void StartAction()
    {
        azuha = StageManager.Instance.Azuha;
        azuha.navMeshAgent.enabled = false;
    }

    public override void UpdateAction()
    {

    }

    public override void EndAction()
    {

    }
}
