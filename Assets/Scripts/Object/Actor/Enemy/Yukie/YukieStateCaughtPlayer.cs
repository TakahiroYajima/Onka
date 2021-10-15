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
        yukie = StageManager.Instance.Yukie;
        yukie.navMeshAgent.velocity = Vector3.zero;
        yukie.navMeshAgent.enabled = false;
        yukie.capsuleCollider.enabled = false;
        yukie.onColliderEnterCallback = null;
        Debug.Log("GameOver");

        //プレイヤーの背後から捕まえたかによってアクションを変える
        //背後：噛みつかれるUIを画面に表示するだけ
        //それ以外：プレイヤーのカメラを自分に向けさせ、獣が人間を襲う時のFPS的なアクションをさせてからゲームオーバー
    }

    public override void UpdateAction()
    {
        
    }

    public override void EndAction()
    {

    }
}
