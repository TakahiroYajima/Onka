using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 雪絵がプレイヤーを発見した瞬間の挙動
/// </summary>
public class YukieStateRecognizedPlayer : StateBase
{
    private Enemy_Yukie yukie = null;

    public override void StartAction()
    {
        yukie = StageManager.Instance.Yukie;
        yukie.ChangeState(EnemyState.ChasePlayer);
    }

    public override void UpdateAction()
    {

    }

    public override void EndAction()
    {
        
    }

}
