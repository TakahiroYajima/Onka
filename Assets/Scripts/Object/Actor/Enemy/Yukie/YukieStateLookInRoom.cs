using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundDistance;

/// <summary>
/// 雪絵が通常の徘徊中にドアが開いている部屋や2階を覗くアクション
/// </summary>
public class YukieStateLookInRoom : StateBase
{
    private Enemy_Yukie yukie = null;
    private YukieStateWandering yukieStateWandering = null;

    private enum State
    {
        None,
        Init,
        MoveToCenter,
        FirstSearchRay,
        MoveLook,
        Moving,
        LookEnd,
        MoveBack,
        MoveBackEnd,
        ForceLookToPlayer,//無理やりプレイヤーのいる方を見る（気配を察知）
        End
    }

    private State currentState;
    private int frameCount = 0;

    private Vector3 moveToCenterTarget;
    private Vector3 targetDireciton;
    private Vector3 initAngle;
    private float rotationSpeed = 1.4f;
    private LookInRoomJudgeManager.RoomPointSoundDistancePointData currentTargetRoomPoint;

    public Action OnCompleted;
    public int prevLookPointID { get; private set; } = -1;
    public void SetLookPointID(int id)
    {
        prevLookPointID = id;
    }


    public YukieStateLookInRoom(Enemy_Yukie _yukie, YukieStateWandering _yukieStateWandering)
    {
        yukie = _yukie;
        yukieStateWandering = _yukieStateWandering;
    }

    public void SetCurrentTargetRoomPoint(LookInRoomJudgeManager.RoomPointSoundDistancePointData target)
    {
        currentTargetRoomPoint = target;
    }

    public override void StartAction()
    {
        yukie.onColliderEnterCallback = null;
        yukie.onColliderStayCallback = null;
        currentState = State.Init;
        frameCount = 0;
    }

    public override void UpdateAction()
    {
        if (frameCount >= Enemy_Yukie.doUpdateFrameCount)
        {
            SerachPlayerUpdate();
            frameCount = 0;
        }
        else
        {
            frameCount++;
        }

        switch (currentState)
        {
            case State.Init:
                //int currentWanderingPointID = yukie.wanderingActor.currentWanderingPointID;
                int currentOuterSDPID = yukie.SoundEmitter.currentOuterPointID;
                moveToCenterTarget = SoundDistanceManager.Instance.soundDistancePoints[currentOuterSDPID].transform.position;
                //moveToCenterTarget.y = yukie.transform.position.y;
                Debug.Log($"LookInRoomInit : {moveToCenterTarget} :: {yukie.navMeshAgent.pathStatus} != {UnityEngine.AI.NavMeshPathStatus.PathInvalid} ? ");
                yukie.navMeshAgent.SetDestination(moveToCenterTarget);
                yukie.navMeshAgent.speed *= 1.5f;
                ChangeState(State.MoveToCenter);
                break;
            case State.MoveToCenter:
                initAngle = yukie.transform.forward;
                Debug.Log($"MoveToCenter : {yukie.navMeshAgent.velocity.magnitude}");
                //if ((yukie.transform.position - moveToCenterTarget).sqrMagnitude <= 2f)
                //{
                //    ChangeState(State.FirstSearchRay);
                //}
                if (yukie.navMeshAgent.velocity.magnitude <= 0.001f)
                {
                    Debug.Log("FirstSearchRay");
                    yukie.navMeshAgent.speed = yukie.walkSpeed;
                    ChangeState(State.FirstSearchRay);
                }
                break;
            case State.FirstSearchRay:
                if (IsPlayerHitSerchRay(yukie.player.transform.position + new Vector3(0f, yukie.player.defaultColliderHeightHalf, 0f)))
                {
                    Debug.Log("FirstSearchRay Hit");
                    OnRecognizedPlayerAction();//プレイヤーを目視できていたら追いかけるステートへ
                }
                else
                {
                    ChangeState(State.MoveLook);
                }
                break;
            case State.MoveLook:
                yukie.StartCoroutine(LookInRoomOrAlwaysOpenPoint(State.End));
                ChangeState(State.Moving);
                break;
            case State.Moving:
                //nop
                break;
            //case State.LookEnd:

            //    break;
            //case State.MoveBack:

            //    break;
            //case State.MoveBackEnd:

            //    break;
            case State.End:
                currentState = State.None;
                yukie.StopCoroutine(LookInRoomOrAlwaysOpenPoint(State.None));
                OnCompleted?.Invoke();
                break;
            default:break;
        }
    }

    public override void EndAction()
    {
        yukie.wanderingActor.SetActive(true);//念のため
        frameCount = 0;
    }

    /// <summary>
    /// プレイヤーを探す
    /// </summary>
    public void SerachPlayerUpdate()
    {
        if (yukie.IsInSightPlayer())
        {
            if (IsPlayerHitSerchRay(yukie.player.transform.position))
                OnRecognizedPlayerAction();//プレイヤーを目視できていたら追いかけるステートへ
            else if (IsPlayerHitSerchRay(yukie.player.transform.position + new Vector3(0f, yukie.player.defaultColliderHeightHalf, 0f)))
                OnRecognizedPlayerAction();
        }
        else
        {
            yukie.provokedSystem.ProvokedUpdate_ToPlayer_6Frame(yukie.player.transform.position);
        }
    }

    private IEnumerator LookInRoomOrAlwaysOpenPoint(State nextState)
    {
        Debug.Log("LookInRoomOrAlwaysOpenPointStart");
        float waitTime = 1f;
        float currentTime = 0f;
        //内容が同じものの連続なので後でリファクタ
        if(currentTargetRoomPoint.alwaysOpen != null)
        {
            foreach(var open in currentTargetRoomPoint.alwaysOpen)
            {
                Debug.Log($"AlwaysOpen : {open.transform.position}");
                targetDireciton = open.transform.position - yukie.transform.position;
                targetDireciton.y = 0f;
                while (!RotationAction())
                {
                    if (IsPlayerHitSerchRay(yukie.player.transform.position))
                    {
                        OnRecognizedPlayerAction();
                        yield break;
                    }
                    yield return null;
                }
                while(currentTime < waitTime)
                {
                    if (IsPlayerHitSerchRay(yukie.player.transform.position))
                    {
                        OnRecognizedPlayerAction();
                        yield break;
                    }
                    currentTime += Time.deltaTime;
                    yield return null;
                }
                currentState = 0f;
                targetDireciton = initAngle;
                while (!RotationAction())
                {
                    if (IsPlayerHitSerchRay(yukie.player.transform.position))
                    {
                        OnRecognizedPlayerAction();
                        yield break;
                    }
                    yield return null;
                }
            }
        }
        if(currentTargetRoomPoint.doors != null)
        {
            foreach(var door in currentTargetRoomPoint.doors)
            {
                Debug.Log($"Door : {door.transform.position}");
                targetDireciton = door.transform.position - yukie.transform.position;
                targetDireciton.y = 0f;
                while (!RotationAction())
                {
                    if (IsPlayerHitSerchRay(yukie.player.transform.position))
                    {
                        OnRecognizedPlayerAction();
                        yield break;
                    }
                    yield return null;
                }
                while (currentTime < waitTime)
                {
                    if (IsPlayerHitSerchRay(yukie.player.transform.position))
                    {
                        OnRecognizedPlayerAction();
                        yield break;
                    }
                    currentTime += Time.deltaTime;
                    yield return null;
                }
                currentState = 0f;
                targetDireciton = initAngle;
                while (!RotationAction())
                {
                    if (IsPlayerHitSerchRay(yukie.player.transform.position))
                    {
                        OnRecognizedPlayerAction();
                        yield break;
                    }
                    yield return null;
                }
            }
        }
        currentState = nextState;
    }
    /// <summary>
    /// 回転の動作。完了したらtrueを返す
    /// </summary>
    /// <returns></returns>
    private bool RotationAction()
    {
        float dot = Vector3.Dot(yukie.transform.forward, targetDireciton);

        float deg = Mathf.Acos(dot) * Mathf.Rad2Deg;
        //Debug.Log($"InRoomRotationAction : {currentState} : {targetDireciton} : {dot} : acos : {Mathf.Acos(dot)} : {deg} if <= {rotationSpeed}");
        //dotが1になるとAcosがバグる（本来想定されていない計算になる）ので、1になったら終了判定
        if (dot >= 1f || deg <= rotationSpeed)
        {
            return true;
        }
        else
        {
            Vector3 newDir = Vector3.RotateTowards(yukie.transform.forward, targetDireciton, rotationSpeed * Time.deltaTime, 0f);
            yukie.transform.rotation = Quaternion.LookRotation(newDir);
        }
        return false;
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

    private void ChangeState(State nextState)
    {
        currentState = nextState;
    }
    private void OnRecognizedPlayerAction()
    {
        EndAction();//念のため自分で終了処理を呼ぶ
        yukie.isRecognizedPlayerInRoom = true;
        yukie.ChangeState(EnemyState.RotateToPlayer);//プレイヤーを目視できていたら追いかけるステートへ
    }
}
