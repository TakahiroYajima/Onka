using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 雪絵がプレイヤーを追いかけるステート
/// </summary>
public class YukieStateChasePlayer : StateBase
{
    private Enemy_Yukie yukie = null;

    private int frameCount = 0;
    private const int doUpdateFrameCount = 6;
    private float noRecognitionTime = 0f;
    private const float ToChangeWanderingLostPlayerTime = 8.5f;//プレイヤーを見失ってから探索モードに戻るまでの時間

    private bool isHitPlayer = false;//最初からプレイヤーに衝突している場合、OnColliderEnterが反応しないので、OnColliderStayを1度だけ発生させるようにするフラグ

    public YukieStateChasePlayer(Enemy_Yukie _yukie)
    {
        yukie = _yukie;
    }

    public override void StartAction()
    {
        yukie.navMeshAgent.enabled = true;
        yukie.navMeshAgent.speed = yukie.runSpeed;
        yukie.onPlayerEnterCallback = OnColliderEnterEvent;
        yukie.onPlayerStayCallback = OnColliderEnterEvent;
        frameCount = 0;
        yukie.SetMaxVolume(1f);
        yukie.PlaySoundLoop(1,1f);
        noRecognitionTime = 0f;
        yukie.ToPlayerWallCollider.enabled = false;

        yukie.player.AddChasedCount(yukie);
    }

    public override void UpdateAction()
    {
        yukie.navMeshAgent.SetDestination(yukie.player.transform.position);
        bool isHitPlayer = yukie.raycastor.IsRaycastHitObjectMatch(yukie.transform.position, yukie.player.transform.position, Tags.Player, 11f);
        //2階に向かって追いかけている時はプレイヤーを見上げるようにする（プレイヤーにRayが当たっていないと壁越しに階段の上を見上げるというちょっと笑える挙動になるのでチェックを入れる）
        if (Mathf.Abs(yukie.transform.position.y - yukie.player.transform.position.y) > 0.35f && isHitPlayer)
        {
            yukie.LookRotationFaceToTarget(yukie.player.eyePosition);
        }
        else
        {
            Vector3 target = yukie.transform.position + yukie.transform.forward;
            target.y = yukie.FaceTransform.position.y;
            yukie.LookRotationFaceToTarget(target);
        }

        if (yukie.isEternalChaseMode) return;//永久追尾モードなら追いかけ続ける

        //プレイヤーをToChangeWanderingLostPlayerTime秒間認識しなかったら徘徊モードに戻る
        if (frameCount >= doUpdateFrameCount)
        {
            if(noRecognitionTime >= ToChangeWanderingLostPlayerTime)
            {
                yukie.navMeshAgent.velocity = Vector3.zero;
                yukie.ChangeState(EnemyState.Wandering);
                //Debug.Log("プレイヤー追尾をあきらめた");
                yukie.player.RemoveChasedCount(yukie);
                frameCount = 0;
                return;
            }

            if (isHitPlayer)
            {
                noRecognitionTime = 0f;
            }
            else
            {
                noRecognitionTime += Time.deltaTime * doUpdateFrameCount;
            }
            //yukie.raycastor.ObjectToRayAction(yukie.transform.position, yukie.player.transform.position, (RaycastHit hit) =>
            //{
            //    if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, Tags.Player))
            //    {
            //        noRecognitionTime = 0f;
            //    }
            //    else
            //    {
            //        noRecognitionTime += Time.deltaTime * doUpdateFrameCount;
            //    }
            //}, 11f);
            frameCount = 0;
        }
        else
        {
            frameCount++;
        }
    }

    public override void EndAction()
    {
        yukie.onPlayerStayCallback = null;
        yukie.ToPlayerWallCollider.enabled = true;
    }

    public void OnColliderEnterEvent(Collider collider)
    {
        if (isHitPlayer) return;

        switch (collider.transform.tag)
        {
            case Tags.Player:
                isHitPlayer = true;
                yukie.ChangeState(EnemyState.CaughtPlayer);
                break;
        }
    }
}
