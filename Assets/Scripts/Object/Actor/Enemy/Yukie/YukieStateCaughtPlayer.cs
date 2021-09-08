using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 雪絵がプレイヤーを捕まえた際のステート
/// </summary>
public class YukieStateCaughtPlayer : StateBase
{
    private Enemy_Yukie yukie = null;

    public override void StartAction()
    {
        yukie = StageManager.Instance.GetYukie();
        yukie.navMeshAgent.velocity = Vector3.zero;
        yukie.navMeshAgent.enabled = false;
        yukie.capsuleCollider.enabled = false;
        yukie.onCollisionEnterCallback = null;
        Debug.Log("GameOver");
    }

    public override void UpdateAction()
    {
        
    }

    public override void EndAction()
    {

    }
}
