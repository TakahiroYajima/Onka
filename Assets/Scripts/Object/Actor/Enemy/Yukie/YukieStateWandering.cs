using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 雪絵の徘徊ステート（通常モード）
/// </summary>
public class YukieStateWandering : StateBase
{
    private Enemy_Yukie yukie = null;
    private int frameCount = 0;
    private bool isInitialized = false;
    
    public YukieStateWandering(Enemy_Yukie _yukie)
    {
        yukie = _yukie;
    }

    public override void StartAction()
    {
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
        yukie.provokedSystem.Initialize(7.5f, () =>
        {
            yukie.ChangeState(EnemyState.RotateToPlayer);
        }, yukie.EyeTransform);
    }

    public override void UpdateAction()
    {
        if(frameCount >= Enemy_Yukie.doUpdateFrameCount)
        {
            if (yukie.IsInSightPlayer()){
                //壁を挟んでいなければプレイヤーを発見させる
                yukie.raycastor.ObjectToRayAction(yukie.transform.position, yukie.player.transform.position, (RaycastHit hit) =>
                {
                    if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, Tags.Player))
                    {
                        yukie.ChangeState(EnemyState.RecognizedPlayer);
                    }
                }, 10f);
                yukie.provokedSystem.provocationingTime = 0f;
            }
            else
            {
                //一定時間プレイヤーが自分の近くをうろついていたら振り返り、発見モードになる
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
