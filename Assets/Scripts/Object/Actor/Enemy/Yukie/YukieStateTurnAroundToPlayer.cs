﻿/// <summary>
/// プレイヤーの方へ振り返るステート。追いかけるステートへのつなぎ
/// </summary>
public class YukieStateTurnAroundToPlayer : StateBase
{
    private Enemy_Yukie yukie = null;

    public YukieStateTurnAroundToPlayer(Enemy_Yukie _yukie)
    {
        yukie = _yukie;
    }

    public override void StartAction()
    {
        yukie.wanderingActor.SetActive(false);
    }

    public override void UpdateAction()
    {
        if (yukie.TurnAroundToTargetAngle_Update(yukie.player.transform.position, yukie.player.transform.position.y >= 3f))
        {
            yukie.ChangeState(EnemyState.RecognizedPlayer);
        }
    }

    public override void EndAction()
    {
        yukie.wanderingActor.SetActive(true);
    }
}
