using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// プレイヤーを追いかけている途中で部屋に入った時、プレイヤーを探したりするアクション
/// </summary>
public class YukieStateInRoomWandering : StateBase
{
    private Enemy_Yukie yukie = null;

    private YukieInRoomWanderingState currentState = YukieInRoomWanderingState.SerachPlayerInitialize;

    //SerachPlayerToLeft & Right
    private const float rotationAngle = 60f;

    //SerachPlayerUpdate
    private Vector3 transformFoward;
    private Vector3 transformRight;
    private Vector3 transformLeft;
    //private Vector3 currentTargetPosition;
    private Vector3 targetDireciton;
    private float rotationSpeed = 2f;

    public override void StartAction()
    {
        yukie = StageManager.Instance.Yukie;

        currentState = YukieInRoomWanderingState.Init;
        yukie.inRoomWanderingActor.onArrivaledPointCallback = OnArrivaledPointCallback;
        yukie.inRoomWanderingActor.SetActive(true, yukie.inRoomChecker.CurrentEnterRoomList[yukie.inRoomChecker.CurrentEnterRoomList.Count - 1], true);
        Debug.Log("Yukie:InRoomStart : " + yukie.inRoomChecker.CurrentEnterRoomList[yukie.inRoomChecker.CurrentEnterRoomList.Count - 1]);
    }

    public override void UpdateAction()
    {
        SerachPlayerUpdate();
        //Debug.Log(currentState.ToString());
        switch (currentState)
        {
            case YukieInRoomWanderingState.Init:
                yukie.inRoomWanderingActor.MoveNext();
                currentState = YukieInRoomWanderingState.FirstWanderingMove;
                break;
            case YukieInRoomWanderingState.FirstWanderingMove:
                
                break;
            case YukieInRoomWanderingState.SerachPlayerInitialize:
                transformFoward = yukie.transform.forward;
                transformRight = Quaternion.AngleAxis(-rotationAngle, Vector3.up) * yukie.transform.forward;//yukie.transform.right + transformFoward;
                transformLeft = Quaternion.AngleAxis(rotationAngle, Vector3.up) * yukie.transform.forward;//-yukie.transform.right + transformFoward;
                Debug.Log(yukie.transform.position + " : " + (yukie.transform.position + transformRight) + " : " + (yukie.transform.position + transformLeft));
                targetDireciton = transformLeft;
                yukie.StopSound();
                FirstSerachPlayer();
                break;
            case YukieInRoomWanderingState.SerachPlayerToLeft:
                RotationAction();
                break;
            case YukieInRoomWanderingState.SerachPlayerMiddleInit:
                targetDireciton = transformRight;
                DoNextState();
                break;
            case YukieInRoomWanderingState.SerachPlayerToRight:
                RotationAction();
                break;
            case YukieInRoomWanderingState.SerachPlayerEndInit:
                targetDireciton = transformFoward;
                DoNextState();
                break;
            case YukieInRoomWanderingState.SerachPlayerEndRotation:
                RotationAction();
                break;
            case YukieInRoomWanderingState.DoWanderingInit:
                yukie.PlaySoundLoop(0, 0.3f);
                yukie.inRoomWanderingActor.SetMoveSpeed(yukie.walkSpeed);
                yukie.inRoomWanderingActor.MoveNext();
                DoNextState();
                break;
            case YukieInRoomWanderingState.WanderingEnd:
                yukie.player.RemoveChasedCount();
                yukie.ChangeState(EnemyState.Wandering);
                break;
            default:
                break;
        }
    }

    public override void EndAction()
    {
        Debug.Log("EndAction ");
        yukie.inRoomWanderingActor.SetActive(false,null);
    }

    private void FirstSerachPlayer()
    {
        //まだプレイヤーを目視できているか判定
        yukie.raycastor.ObjectToRayAction(yukie.transform.position, yukie.player.transform.position, (RaycastHit hit) =>
        {
            if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, Tags.Player))
            {
                yukie.ChangeState(EnemyState.RecognizedPlayer);//プレイヤーを目視できていたら追いかけるステートへ
            }
            else
            {
                //プレイヤーを目視できていなかったらこのまま部屋内徘徊へ
                yukie.navMeshAgent.velocity = Vector3.zero;
                yukie.navMeshAgent.SetDestination(yukie.transform.position);

                currentState = YukieInRoomWanderingState.SerachPlayerToLeft;
            }
        }, 12f);
    }

    /// <summary>
    /// プレイヤーを探す
    /// </summary>
    public void SerachPlayerUpdate()
    {
        if (yukie.IsInSightPlayer())
        {
            yukie.raycastor.ObjectToRayAction(yukie.transform.position, yukie.player.transform.position, (RaycastHit hit) =>
            {
                if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, Tags.Player))
                {
                    yukie.ChangeState(EnemyState.RecognizedPlayer);//プレイヤーを目視できていたら追いかけるステートへ
                }
            }, 12f);
        }
    }

    private void RotationAction()
    {
        float dot = Vector3.Dot(yukie.transform.forward, targetDireciton);

        float deg = Mathf.Acos(dot) * Mathf.Rad2Deg;
        //Debug.Log("InRoomRotationAction : " + currentState.ToString() + " : " + deg);
        if (deg <= rotationSpeed)
        {
            //yukie.transform.LookAt(targetDireciton);
            DoNextState();
        }
        else
        {
            Vector3 newDir = Vector3.RotateTowards(yukie.transform.forward, targetDireciton, rotationSpeed * Time.deltaTime, 0f);
            yukie.transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    /// <summary>
    /// 徘徊通過地点に到達した時のコールバック
    /// </summary>
    /// <param name="nextTargetID"></param>
    private void OnArrivaledPointCallback(int nextTargetID)
    {
        
        if(nextTargetID == 1)
        {
            //最初
            if (currentState == YukieInRoomWanderingState.FirstWanderingMove)
            {
                currentState = YukieInRoomWanderingState.SerachPlayerInitialize;
            }
            //徘徊し終わった
            else if(currentState == YukieInRoomWanderingState.WanderingInRoom)
            {
                currentState = YukieInRoomWanderingState.WanderingEnd;
            }
        }
        else
        {
            yukie.inRoomWanderingActor.MoveNext();
        }
    }

    private void DoNextState()
    {
        int nextID = (int)currentState + 1;
        if(Enum.IsDefined(typeof(YukieInRoomWanderingState), nextID))
        {
            currentState = (YukieInRoomWanderingState)Enum.ToObject(typeof(YukieInRoomWanderingState), nextID);
        }
    }
}

public enum YukieInRoomWanderingState
{
    Init = 0,//初期化
    FirstWanderingMove,//最初にIDが0の地点まで移動する（部屋に入る動作）
    SerachPlayerInitialize,//プレイヤーを探す
    SerachPlayerToLeft,//自分の左側を向く
    SerachPlayerMiddleInit,//右側へ向く前の準備
    SerachPlayerToRight,//自分の右側を向く
    SerachPlayerEndInit,//正面に向き直す準備
    SerachPlayerEndRotation,//正面に向き直す
    DoWanderingInit,//徘徊前初期化
    WanderingInRoom,//徘徊
    WanderingEnd,//徘徊終了、部屋を出る
}