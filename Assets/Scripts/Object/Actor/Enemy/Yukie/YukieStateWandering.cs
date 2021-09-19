using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 雪絵の徘徊ステート
/// </summary>
public class YukieStateWandering : StateBase
{
    private Enemy_Yukie yukie = null;
    private int frameCount = 0;
    private const int doUpdateFrameCount = 6;
    private bool isInitialized = false;
    //プレイヤーが自分の視界の範囲外をうろつく＝挑発しているので、それに対する不意打ち要素
    private const float NoticeProvocationTime = 10f;//挑発に気付くまでの時間
    private float provocationingTime = 0f;//プレイヤーに挑発されている時間

    public override void StartAction()
    {
        yukie = StageManager.Instance.GetYukie();
        yukie.wanderingActor.SetActive(true);
        //if (!isInitialized)
        //{
        //    isInitialized = true;
        //    yukie.wanderingActor.SetWanderingID(0);
        //}
        yukie.wanderingActor.SetWanderingID(0);
        yukie.wanderingActor.SetMoveSpeed(1f);
        yukie.onColliderEnterCallback = null;
        yukie.PlaySoundLoop(0, 0.3f);
    }

    public override void UpdateAction()
    {

        if(frameCount >= doUpdateFrameCount)
        {
            yukie.UpdatePositionXZ();
            if (yukie.IsInSightPlayer()){
                //壁を挟んでいないか
                yukie.raycastor.ObjectToRayAction(yukie.transform.position, yukie.player.transform.position, (RaycastHit hit) =>
                {
                    if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, Tags.Player))
                    {
                        yukie.ChangeState(EnemyState.RecognizedPlayer);
                    }
                }, 10f);
            }
            else
            {
                //10秒ほどプレイヤーが自分の近くをうろついていたら振り返り、発見モードになる
                //煽っていたら突然振り向いて追いかけられる要素が欲しい
            }
            frameCount = 0;
        }
        else
        {
            frameCount++;
        }
        
    }

    public override void EndAction()
    {
        yukie.wanderingActor.SetActive(false);
    }

    private void OnColliderEnterEvent(Collider collider)
    {
        switch (collider.transform.tag)
        {
            case Tags.Door:
                DoorObject doorObject = collider.gameObject.GetComponent<DoorObject>();
                if (doorObject != null)
                {
                    //ドアが解錠済みなら自分で開けられる
                    if (doorObject.isUnlocked)
                    {
                        doorObject.OpenDoor();
                    }
                }
                break;
        }
    }
}
