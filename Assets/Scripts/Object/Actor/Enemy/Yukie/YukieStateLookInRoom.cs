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
    private const float RotationSpeed = 1.4f;
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
        yukie.onPlayerEnterCallback = null;
        yukie.onPlayerStayCallback = null;
        currentState = State.Init;
        frameCount = 0;
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

        switch (currentState)
        {
            case State.Init:
                int currentOuterSDPID = yukie.SoundEmitter.currentOuterPointID;
                moveToCenterTarget = SoundDistanceManager.Instance.soundDistancePoints[currentOuterSDPID].transform.position;
                //Debug.Log($"LookInRoomInit : {moveToCenterTarget} :: {yukie.navMeshAgent.pathStatus} != {UnityEngine.AI.NavMeshPathStatus.PathInvalid} ? ");
                yukie.navMeshAgent.SetDestination(moveToCenterTarget);
                yukie.navMeshAgent.speed *= 1.5f;
                ChangeState(State.MoveToCenter);
                break;
            case State.MoveToCenter:
                if (yukie.navMeshAgent.velocity.magnitude <= 0.001f)
                {
                    yukie.navMeshAgent.speed = yukie.walkSpeed;
                    ChangeState(State.FirstSearchRay);
                }
                break;
            case State.FirstSearchRay:
                if (IsPlayerHitSerchRay(yukie.player.Position + new Vector3(0f, yukie.player.defaultColliderHeightHalf, 0f)))
                {
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
            if (IsPlayerHitSerchRay(yukie.player.Position))
                OnRecognizedPlayerAction();//プレイヤーを目視できていたら追いかけるステートへ
            else if (IsPlayerHitSerchRay(yukie.player.Position + new Vector3(0f, yukie.player.defaultColliderHeightHalf, 0f)))
                OnRecognizedPlayerAction();
        }
        else
        {
            yukie.provokedSystem.ProvokedUpdate_ToPlayer_6Frame(yukie.player.Position);
        }
    }

    private IEnumerator LookInRoomOrAlwaysOpenPoint(State nextState)
    {
        Vector3 initForwardPosition = yukie.FaceTransform.position + yukie.FaceTransform.forward;//yukie.transform.position + yukie.transform.forward;

        if(currentTargetRoomPoint.alwaysOpen != null)
        {
            foreach(var open in currentTargetRoomPoint.alwaysOpen)
            {
                bool is2F = open.pointType == AlwaysOpenRookPointType.OnThe2F;
                yield return TurnAroundAction(open.transform.position, is2F);
                yield return WaitAction();
                yield return TurnAroundAction(initForwardPosition, is2F);
            }
        }
        if(currentTargetRoomPoint.doors != null)
        {
            foreach(var door in currentTargetRoomPoint.doors)
            {
                if (!door.isOpenState) continue;

                yield return TurnAroundAction(door.transform.position);
                yield return WaitAction();
                yield return TurnAroundAction(initForwardPosition);
            }
        }
        currentState = nextState;
    }

    private IEnumerator TurnAroundAction(Vector3 targetPosition, bool isFaceRotation = false, float rotationSpeed = RotationSpeed)
    {
        while (!yukie.TurnAroundToTargetAngle_Update(targetPosition, isFaceRotation, rotationSpeed))
        {
            if (OnRecognizedPlayerAndCoroutineJudge())
            {
                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator WaitAction()
    {
        float waitTime = 1f;
        float currentTime = 0f;
        while (currentTime < waitTime)
        {
            if (OnRecognizedPlayerAndCoroutineJudge())
            {
                yield break;
            }
            currentTime += Time.deltaTime;
            yield return null;
        }
        currentState = 0f;
    }
    /// <summary>
    /// プレイヤーを発見していたらコルーチンを終了させる
    /// </summary>
    /// <returns></returns>
    private bool OnRecognizedPlayerAndCoroutineJudge()
    {
        if (IsPlayerHitSerchRay(yukie.player.Position))
        {
            OnRecognizedPlayerAction();
            yukie.StopCoroutine(LookInRoomOrAlwaysOpenPoint(State.None));
            return true;
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
