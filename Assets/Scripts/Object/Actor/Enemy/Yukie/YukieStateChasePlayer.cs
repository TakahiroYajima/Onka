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
    private const float ToChangeWanderingLostPlayerTime = 6f;//プレイヤーを見失ってから探索モードに戻るまでの時間

    private bool isHitPlayer = false;//最初からプレイヤーに衝突している場合、OnColliderEnterが反応しないので、OnColliderStayを1度だけ発生させるようにするフラグ

    public override void StartAction()
    {
        yukie = StageManager.Instance.Yukie;
        yukie.navMeshAgent.enabled = true;
        yukie.navMeshAgent.speed = yukie.runSpeed;
        yukie.onColliderEnterCallback = OnColliderEnterEvent;
        yukie.onColliderStayCallback = OnColliderEnterEvent;
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

        if (yukie.isEternalChaseMode) return;//永久追尾モードなら追いかけ続ける

        //プレイヤーをToChangeWanderingLostPlayerTime秒間認識しなかったら徘徊モードに戻る
        if (frameCount >= doUpdateFrameCount)
        {
            if(noRecognitionTime >= ToChangeWanderingLostPlayerTime)
            {
                yukie.navMeshAgent.velocity = Vector3.zero;
                yukie.ChangeState(EnemyState.Wandering);
                Debug.Log("プレイヤー追尾をあきらめた");
                yukie.player.RemoveChasedCount(yukie);
                return;
            }
            
            yukie.raycastor.ObjectToRayAction(yukie.transform.position, yukie.player.transform.position, (RaycastHit hit) =>
            {
                if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, Tags.Player))
                {
                    noRecognitionTime = 0f;
                }
                else
                {
                    noRecognitionTime += Time.deltaTime * doUpdateFrameCount;
                }
            }, 10f);
            frameCount = 0;
        }
        else
        {
            frameCount++;
        }
    }

    public override void EndAction()
    {
        yukie.onColliderStayCallback = null;
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
