using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KozoStateInit : StateBase
{
    private Enemy_Kozo kozo = null;

    public KozoStateInit(Enemy_Kozo _kozo)
    {
        kozo = _kozo;
    }

    public override void StartAction()
    {
        kozo.navMeshAgent.enabled = false;
        kozo.rigidbody.isKinematic = true;
    }

    public override void UpdateAction()
    {

    }

    public override void EndAction()
    {
        kozo.rigidbody.isKinematic = false;
    }
}
