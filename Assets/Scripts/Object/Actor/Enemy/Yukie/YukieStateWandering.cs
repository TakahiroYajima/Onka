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
        yukie.onCollisionEnterCallback = null;
        yukie.soundPlayerObject.PlaySoundLoop(0,0.3f);
    }

    public override void UpdateAction()
    {

        if(frameCount >= doUpdateFrameCount)
        {
            yukie.UpdatePositionXZ();
            yukie.raycastor.ObjectToRayAction(yukie.transform.position, yukie.player.transform.position, (RaycastHit hit) =>
            {
                if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, "Player"))
                {
                    yukie.soundPlayerObject.SetVolume(0.1f);
                }
                else
                {
                    yukie.soundPlayerObject.SetVolume(0.03f);
                }
            }, 100f);

            if (yukie.IsInSightPlayer()){
                //壁を挟んでいないか
                yukie.raycastor.ObjectToRayAction(yukie.transform.position, yukie.player.transform.position, (RaycastHit hit) =>
                {
                    if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, "Player"))
                    {
                        yukie.ChangeState(EnemyState.RecognizedPlayer);
                    }
                }, 10f);
            }
            else
            {
                yukie.soundPlayerObject.SetVolume(0.06f);
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
}
