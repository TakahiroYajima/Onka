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
    private bool isInitialized = false;
    //プレイヤーが自分の視界の範囲外をうろつく＝挑発しているので、それに対する不意打ち要素
    //private const float NoticeProvocationTime = 10f;//挑発に気付くまでの時間
    //private float provocationingTime = 0f;//プレイヤーに挑発されている時間

    public override void StartAction()
    {
        yukie = StageManager.Instance.Yukie;
        //provocationingTime = 0f;
        yukie.wanderingActor.SetActive(true);
        if (!isInitialized)
        {
            isInitialized = true;
            yukie.wanderingActor.SetWanderingID(0);
        }
        else
        {
            yukie.wanderingActor.SetWanderingID(yukie.wanderingActor.currentWanderingPointID);
        }
        yukie.wanderingActor.SetMoveSpeed(1f);
        yukie.onColliderEnterCallback = null;
        yukie.PlaySoundLoop(0, 0.3f);
        yukie.provokedSystem.Initialize(10f, () =>
        {
            yukie.ChangeState(EnemyState.RotateToPlayer);
        });
    }

    public override void UpdateAction()
    {

        if(frameCount >= Enemy_Yukie.doUpdateFrameCount)
        {
            //yukie.UpdatePositionXZ();
            if (yukie.IsInSightPlayer()){
                //壁を挟んでいないか
                yukie.raycastor.ObjectToRayAction(yukie.transform.position, yukie.player.transform.position, (RaycastHit hit) =>
                {
                    if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, Tags.Player))
                    {
                        yukie.ChangeState(EnemyState.RecognizedPlayer);
                    }
                }, 10f);
                //provocationingTime = 0f;
                yukie.provokedSystem.provocationingTime = 0f;
            }
            else
            {
                //10秒ほどプレイヤーが自分の近くをうろついていたら振り返り、発見モードになる
                //煽っていたら突然振り向いて追いかけられる要素が欲しい
                //yukie.raycastor.ObjectToRayAction(yukie.transform.position, yukie.player.transform.position, (RaycastHit hit) =>
                //{
                //    if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, Tags.Player))
                //    {
                //        provocationingTime += Time.deltaTime * doUpdateFrameCount;
                //        if (provocationingTime >= NoticeProvocationTime)
                //        {
                //            //さんざん煽られたので振り返って追いかける（不意打ち要素・実況者殺し）
                //            yukie.ChangeState(EnemyState.RotateToPlayer);
                //            provocationingTime = 0f;
                //        }
                //    }
                //    else
                //    {
                //        provocationingTime -= Time.deltaTime * doUpdateFrameCount;
                //        if(provocationingTime < 0f) { provocationingTime = 0f; }
                //    }
                //}, 6f);//6メートル以内なら煽りと判断
                yukie.provokedSystem.ProvokedUpdate_ToPlayer_6Frame(yukie.player.transform.position);
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
