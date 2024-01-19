using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 雪絵がプレイヤーを捕まえた際のステート
/// </summary>
public class YukieStateCaughtPlayer : StateBase
{
    private Enemy_Yukie yukie = null;
    Vector3 playerDir;
    bool isFrontCaught = false;//プレイヤーの前から捕まえたか

    public YukieStateCaughtPlayer(Enemy_Yukie _yukie)
    {
        yukie = _yukie;
    }

    public override void StartAction()
    {
        yukie.navMeshAgent.velocity = Vector3.zero;
        yukie.navMeshAgent.enabled = false;
        yukie.capsuleCollider.enabled = false;
        yukie.ToPlayerWallCollider.enabled = false;
        yukie.onPlayerEnterCallback = null;
        //Debug.Log("GameOver");

        yukie.player.ChangeState(PlayerState.Arrested);
        //プレイヤーの背後から捕まえたかによってアクションを変える
        playerDir = (yukie.transform.position - yukie.player.Position).normalized;
        isFrontCaught = Utility.Instance.IsInSightAngle(yukie.player.gameObject, yukie.gameObject, 120f);
        yukie.StartCoroutine(Move());
        
        //背後：噛みつかれるUIを画面に表示するだけ
        //それ以外：プレイヤーのカメラを自分に向けさせ、獣が人間を襲う時のFPS的なアクションをさせてからゲームオーバー
        
    }

    public override void UpdateAction()
    {
        yukie.player.cameraMovingObject.TurnAroundSmooth_Update(yukie.FaceTransform.position, 14f);
    }

    public override void EndAction()
    {

    }

    private IEnumerator Move()
    {
        yukie.FaceTransform.LookAt(yukie.player.eyePosition);
        yield return yukie.StartCoroutine(yukie.movingObject.MoveWithTime(yukie.transform.position + (playerDir * 2f), 0.3f));
        yukie.FaceTransform.LookAt(yukie.player.eyePosition);
        Vector3 targetPos = yukie.player.eyePosition + playerDir - new Vector3(0,0.17f,0);
        yield return yukie.StartCoroutine(yukie.movingObject.MoveWithTime(targetPos, 0.2f));
        yukie.StopSound();
        GameOverManager.Instance.StartGameOver(GameOverType.YukieArrested);
    }
}
