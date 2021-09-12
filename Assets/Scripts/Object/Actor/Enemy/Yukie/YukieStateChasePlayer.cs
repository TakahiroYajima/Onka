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
    private const float ToChangeWanderingLostPlayerTime = 6f;

    public override void StartAction()
    {
        yukie = StageManager.Instance.GetYukie();
        yukie.navMeshAgent.speed = yukie.runSpeed;
        yukie.onColliderEnterCallback = OnColliderEnterEvent;
        frameCount = 0;
        yukie.SetMaxVolume(1f);
        yukie.PlaySoundLoop(1,1f);
        noRecognitionTime = 0f;
        Debug.Log("プレイヤー追尾");
    }

    public override void UpdateAction()
    {
        yukie.navMeshAgent.SetDestination(yukie.player.transform.position);

        //プレイヤーをToChangeWanderingLostPlayerTime秒間認識しなかったら徘徊モードに戻る
        if (frameCount >= doUpdateFrameCount)
        {
            if(noRecognitionTime >= ToChangeWanderingLostPlayerTime)
            {
                yukie.navMeshAgent.velocity = Vector3.zero;
                yukie.ChangeState(EnemyState.Wandering);
                Debug.Log("プレイヤー追尾をあきらめた");
                return;
            }
            yukie.UpdatePositionXZ();
            yukie.raycastor.ObjectToRayAction(yukie.transform.position, yukie.player.transform.position, (RaycastHit hit) =>
            {
                if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, "Player"))
                {
                    noRecognitionTime = 0f;
                    //yukie.SetMaxVolume(1f);
                }
                else
                {
                    noRecognitionTime += Time.deltaTime * doUpdateFrameCount;
                    //yukie.SetMaxVolume(0.5f);
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

    }

    public void OnColliderEnterEvent(Collider collider)
    {
        switch (collider.transform.tag)
        {
            case "Player":
                Debug.Log("プレイヤーを捕まえた");
                yukie.ChangeState(EnemyState.CaughtPlayer);
                break;
        }
    }
}
