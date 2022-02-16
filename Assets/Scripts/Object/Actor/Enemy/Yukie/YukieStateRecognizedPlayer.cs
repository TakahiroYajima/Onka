using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 雪絵がプレイヤーを発見した瞬間の挙動
/// </summary>
public class YukieStateRecognizedPlayer : StateBase
{
    private Enemy_Yukie yukie = null;

    public YukieStateRecognizedPlayer(Enemy_Yukie _yukie)
    {
        yukie = _yukie;
    }

    public override void StartAction()
    {
        yukie.ChangeState(EnemyState.ChasePlayer);
    }

    public override void UpdateAction()
    {

    }

    public override void EndAction()
    {
        
    }

}
