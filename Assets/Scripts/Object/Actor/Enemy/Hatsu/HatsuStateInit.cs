using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatsuStateInit : StateBase
{
    private Enemy_Hatsu hatsu = null;

    public HatsuStateInit(Enemy_Hatsu _hatsu)
    {
        hatsu = _hatsu;
    }

    public override void StartAction()
    {
        hatsu.navMeshAgent.enabled = false;
        hatsu.rigidbody.isKinematic = true;
    }

    public override void UpdateAction()
    {

    }

    public override void EndAction()
    {
        hatsu.rigidbody.isKinematic = false;
    }
}
