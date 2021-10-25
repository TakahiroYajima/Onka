/// <summary>
/// プレイヤーの方へ振り返るステート。追いかけるステートへのつなぎ
/// </summary>
public class YukieStateTurnAroundToPlayer : StateBase
{
    private Enemy_Yukie yukie = null;

    public override void StartAction()
    {
        yukie = StageManager.Instance.Yukie;
        yukie.wanderingActor.SetActive(false);
    }

    public override void UpdateAction()
    {
        yukie.TurnAroundToTargetAngle_Update(yukie.player.transform.position, () =>
         {
             yukie.ChangeState(EnemyState.RecognizedPlayer);
         });
    }

    public override void EndAction()
    {
        yukie.wanderingActor.SetActive(true);
    }
}
