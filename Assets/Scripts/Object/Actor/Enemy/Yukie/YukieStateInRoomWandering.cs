using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// プレイヤーを追いかけている途中で部屋に入った時、プレイヤーを探したりするアクション
/// </summary>
public class YukieStateInRoomWandering : StateBase
{
    private Enemy_Yukie yukie = null;

    private YukieInRoomWanderingState currentState = YukieInRoomWanderingState.SerachPlayerInitialize;
    private int frameCount = 0;

    //SerachPlayerToLeft & Right
    private const float rotationAngle = 60f;

    //SerachPlayerUpdate
    private Vector3 transformFoward;
    private Vector3 transformRight;
    private Vector3 transformLeft;
    private Vector3 targetDireciton;
    private float rotationSpeed = 1.4f;
    //private UnityAction<RaycastHit> serachedAction = null;//プレイヤーを探すコールバック（複数回記述するのが嫌なのでコールバックにした）
    private const float NoticeProvocationTime = 3f;//プレイヤーに気付くまでの時間

    //各アクションの要素
    private UnityAction RotationActionOnComplete = null;//RotationAction()完了後のコールバック

    public YukieStateInRoomWandering(Enemy_Yukie _yukie)
    {
        yukie = _yukie;
    }

    public override void StartAction()
    {
        if (yukie.isIgnoreInRoom || yukie.isRecognizedPlayerInRoom)
        {
            yukie.isRecognizedPlayerInRoom = false;//永久に部屋を無視しないようにここで初期化
            yukie.ChangeState(EnemyState.ChasePlayer);
            return;
        }
        yukie.provokedSystem.Initialize(NoticeProvocationTime, () =>
        {
            //currentState = YukieInRoomWanderingState.RotateToPlayer;
            yukie.ChangeState(EnemyState.RotateToPlayer);
        }, yukie.EyeTransform);

        //if (serachedAction == null)
        //{
        //    serachedAction = (RaycastHit hit) =>
        //    {
        //        if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, Tags.Player))
        //        {
        //            yukie.ChangeState(EnemyState.RecognizedPlayer);//プレイヤーを目視できていたら追いかけるステートへ
        //        }
        //    };
        //}



        currentState = YukieInRoomWanderingState.Init;
        SetInRoomWanderingActorArrivaledCallback();
        yukie.inRoomWanderingActor.Initialize();
        yukie.inRoomWanderingActor.SetActive(true, yukie.inRoomChecker.CurrentEnterRoomList[yukie.inRoomChecker.CurrentEnterRoomList.Count - 1], true);
        //Debug.Log("Yukie:InRoomStart : " + yukie.inRoomChecker.CurrentEnterRoomList[yukie.inRoomChecker.CurrentEnterRoomList.Count - 1]);
    }
    /// <summary>
    /// 徘徊通過点に到着した時のコールバックを設定
    /// </summary>
    /// <param name="isSetMyMethod"></param>
    private void SetInRoomWanderingActorArrivaledCallback(bool isSetMyMethod = true)
    {
        if (isSetMyMethod) yukie.inRoomWanderingActor.onArrivaledPointCallback = OnArrivaledPointCallback;
        else yukie.inRoomWanderingActor.onArrivaledPointCallback = null;
    }

    private void InitTransforms()
    {
        transformFoward = yukie.transform.forward;
        transformRight = Quaternion.AngleAxis(-rotationAngle, Vector3.up) * yukie.transform.forward;//yukie.transform.right + transformFoward;
        transformLeft = Quaternion.AngleAxis(rotationAngle, Vector3.up) * yukie.transform.forward;//-yukie.transform.right + transformFoward;
    }

    public override void UpdateAction()
    {
        if (frameCount >= Enemy_Yukie.DoUpdateFrameCount)
        {
            SerachPlayerUpdate();
            frameCount = 0;
        }
        else
        {
            frameCount++;
        }
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
                InitTransforms();
                //Debug.Log(yukie.transform.position + " : " + (yukie.transform.position + transformRight) + " : " + (yukie.transform.position + transformLeft));
                targetDireciton = transformLeft;
                yukie.StopSound(true);
                RotationActionOnComplete = DoNextState;
                FirstSerachPlayer();
                break;
            case YukieInRoomWanderingState.SerachPlayerToLeft:
                RotationAction(RotationActionOnComplete);
                break;
            case YukieInRoomWanderingState.SerachPlayerMiddleInit:
                targetDireciton = transformRight;
                RotationActionOnComplete = DoNextState;
                DoNextState();
                break;
            case YukieInRoomWanderingState.SerachPlayerToRight:
                RotationAction(RotationActionOnComplete);
                break;
            case YukieInRoomWanderingState.SerachPlayerEndInit:
                targetDireciton = transformFoward;
                RotationActionOnComplete = DoNextState;
                DoNextState();
                break;
            case YukieInRoomWanderingState.SerachPlayerEndRotation:
                RotationAction(RotationActionOnComplete);
                break;
            case YukieInRoomWanderingState.DoWanderingInit:
                yukie.PlaySoundLoop(0, 0.3f);
                yukie.inRoomWanderingActor.SetMoveSpeed(yukie.walkSpeed);
                yukie.inRoomWanderingActor.MoveNext();
                DoNextState();
                break;
            case YukieInRoomWanderingState.WanderingEnd:
                yukie.player.RemoveChasedCount(yukie);
                yukie.ChangeState(EnemyState.Wandering);
                break;
            //case YukieInRoomWanderingState.RotateToPlayer:
            //    yukie.TurnAroundToTargetAngle_Update(yukie.player.Position, () =>
            //    {
            //        yukie.ChangeState(EnemyState.ChasePlayer);
            //    });
            //    break;
            default:
                break;
        }
    }

    public override void EndAction()
    {
        yukie.inRoomWanderingActor.SetActive(false,null);
    }

    private void FirstSerachPlayer()
    {
        //まだプレイヤーを目視できているか判定
        //隠れ箇所で余分に当たり判定が上に出ている部分があるので、立っている時には気付くようにする
        if (IsPlayerHitSerchRay(yukie.player.Position))
            yukie.ChangeState(EnemyState.RecognizedPlayer);//プレイヤーを目視できていたら追いかけるステートへ
        else if (IsPlayerHitSerchRay(yukie.player.Position + new Vector3(0f, yukie.player.defaultColliderHeightHalf, 0f)))
            yukie.ChangeState(EnemyState.RecognizedPlayer);
        else
        {
            //プレイヤーを目視できていなかったらこのまま部屋内徘徊へ
            yukie.navMeshAgent.velocity = Vector3.zero;
            yukie.navMeshAgent.SetDestination(yukie.transform.position);

            currentState = YukieInRoomWanderingState.SerachPlayerToLeft;
        }
    }

    /// <summary>
    /// プレイヤーを探す
    /// </summary>
    public void SerachPlayerUpdate()
    {
        if (yukie.IsInSightPlayer())
        {
            if (IsPlayerHitSerchRay(yukie.player.Position))
                yukie.ChangeState(EnemyState.RecognizedPlayer);//プレイヤーを目視できていたら追いかけるステートへ
            else if (IsPlayerHitSerchRay(yukie.player.Position + new Vector3(0f, yukie.player.defaultColliderHeightHalf, 0f)))
                yukie.ChangeState(EnemyState.RecognizedPlayer);
        }
        else
        {
            yukie.provokedSystem.ProvokedUpdate_ToPlayer_6Frame(yukie.player.Position);
        }
    }
    /// <summary>
    /// 雪絵の目の位置からRayを飛ばし、プレイヤーに当たったかの判定を返す
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    private bool IsPlayerHitSerchRay(Vector3 targetPos)
    {
        return yukie.raycastor.IsRaycastHitObjectMatchWithLayerMask(yukie.EyeTransform.position, targetPos, Tags.Player, LayerMaskData.SerchToPlayerMask, 12f);
    }

    private void RotationAction(UnityAction onComplete)
    {
        float dot = Vector3.Dot(yukie.transform.forward, targetDireciton);

        float deg = Mathf.Acos(dot) * Mathf.Rad2Deg;
        //Debug.Log($"InRoomRotationAction : {currentState} : {targetDireciton} : {dot} : acos : {Mathf.Acos(dot)} : {deg} if <= {rotationSpeed}");
        //dotが1になるとAcosがバグる（本来想定されていない計算になる）ので、1になったら終了判定
        if (dot >= 1f || deg <= rotationSpeed)
        {
            onComplete();
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
    private void OnArrivaledPointCallback(int nextTargetID, OnWPArrivaledEventCode _eventCode)
    {
        UnityAction callback = () =>
        {
            if (!yukie.inRoomWanderingActor.enabled)
            {
                yukie.inRoomWanderingActor.enabled = true;
                yukie.navMeshAgent.enabled = true;
            }
            if (nextTargetID == 1)
            {
                //最初
                if (currentState == YukieInRoomWanderingState.FirstWanderingMove)
                {
                    currentState = YukieInRoomWanderingState.SerachPlayerInitialize;
                }
                //徘徊し終わった
                else if (currentState == YukieInRoomWanderingState.WanderingInRoom)
                {
                    currentState = YukieInRoomWanderingState.WanderingEnd;
                }
            }
            else
            {
                yukie.inRoomWanderingActor.MoveNext();
            }
        };

        if(_eventCode == OnWPArrivaledEventCode.None)
        {
            callback();
        }
        else
        {
            yukie.inRoomWanderingActor.enabled = false;
            yukie.navMeshAgent.enabled = false;
            SetInRoomWanderingActorArrivaledCallback(false);
            switch (_eventCode)
            {
                case OnWPArrivaledEventCode.Survey:
                    InitTransforms();
                    break;
                case OnWPArrivaledEventCode.Survey_RightOnly:
                    InitTransforms();
                    targetDireciton = transformLeft;
                    currentState = YukieInRoomWanderingState.SerachPlayerToLeft;
                    RotationActionOnComplete = ()=>
                    {
                        currentState = YukieInRoomWanderingState.None;
                        RotationActionOnComplete = null;
                        GeneralCoroutine.Instance.Wait(0.7f, () =>
                        {
                            currentState = YukieInRoomWanderingState.SerachPlayerEndRotation;
                            targetDireciton = transformFoward;
                            RotationActionOnComplete = () => {
                                RotationActionOnComplete = null;
                                currentState = YukieInRoomWanderingState.None;
                                GeneralCoroutine.Instance.Wait(0.35f, () =>
                                {
                                    currentState = YukieInRoomWanderingState.WanderingInRoom;
                                    SetInRoomWanderingActorArrivaledCallback();
                                    callback();
                                });
                            };
                        });
                    };
                    break;
            }
        }
    }

    private void DoNextState()
    {
        int nextID = (int)currentState + 1;
        if(Enum.IsDefined(typeof(YukieInRoomWanderingState), nextID))
        {
            currentState = (YukieInRoomWanderingState)Enum.ToObject(typeof(YukieInRoomWanderingState), nextID);
            //Debug.Log($"CurrentStateChange : {currentState}");
        }
        //Debug.Log($"DoNextStateComplete : {currentState}");
    }
}

public enum YukieInRoomWanderingState
{
    None = 0,
    Init,//初期化
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

    //RotateToPlayer,//プレイヤーの方を向く
}