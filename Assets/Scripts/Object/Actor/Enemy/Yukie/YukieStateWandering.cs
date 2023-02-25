using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundDistance;

/// <summary>
/// 雪絵の徘徊ステート（通常モード）
/// </summary>
public class YukieStateWandering : StateBase
{
    private enum State
    {
        Wandering,
        LookInRoom
    }

    private Enemy_Yukie yukie = null;
    private YukieStateLookInRoom yukieStateLookInRoom = null;
    private int frameCount = 0;
    private bool isInitialized = false;
    private State currentState;
    private int currentHitedSDPOuterID = -1;

    public YukieStateWandering(Enemy_Yukie _yukie)
    {
        yukie = _yukie;
        yukieStateLookInRoom = new YukieStateLookInRoom(yukie, this);
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
        yukie.wanderingActor.SetMoveSpeed(1.1f);
        yukie.onColliderEnterCallback = null;
        yukie.PlaySoundLoop(0, 0.3f);
        yukie.provokedSystem.Initialize(7.5f, () =>
        {
            yukie.ChangeState(EnemyState.RotateToPlayer);
        }, yukie.EyeTransform);

        yukie.SoundEmitter.OnEnterOuterPoint = StartLookInRoom;
        currentState = State.Wandering;
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
                }, 18f);
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

        switch (currentState)
        {
            case State.Wandering:

                break;
            case State.LookInRoom:
                yukieStateLookInRoom.UpdateAction();
                break;
        }
    }

    public override void EndAction()
    {
        yukie.SoundEmitter.OnEnterOuterPoint = null;
        yukie.wanderingActor.SetActive(false);
    }

    private void StartLookInRoom(int soundDistancePointID)
    {
        if (soundDistancePointID == yukieStateLookInRoom.prevLookPointID) return;
        if (soundDistancePointID == yukie.SoundEmitter.prevHitedOuterPointID) return;//同じ場所に2回当たったら部屋から出てきたパターンと認識。覗かないようにする
        currentHitedSDPOuterID = soundDistancePointID;
        yukieStateLookInRoom.SetLookPointID(soundDistancePointID);
        var target = LookInRoomJudgeManager.Instance.GetRoomPointData(soundDistancePointID);
        if (!target.isExist) return;
        if (!LookInRoomJudgeManager.Instance.IsNeedLook(target.data)) return;

        yukie.SoundEmitter.OnEnterOuterPoint = null;//他の当たり判定に当たらないように一旦nullにする
        yukieStateLookInRoom.SetCurrentTargetRoomPoint(target.data);
        yukieStateLookInRoom.OnCompleted = EndLookInRoom;
        yukieStateLookInRoom.StartAction();
        currentState = State.LookInRoom;
        Debug.Log($"StartLookInRoom : {soundDistancePointID}");
    }
    private void EndLookInRoom()
    {
        yukieStateLookInRoom.SetCurrentTargetRoomPoint(null);
        yukieStateLookInRoom.OnCompleted = null;
        yukieStateLookInRoom.EndAction();
        currentState = State.Wandering;
        //NavMeshのターゲットを本来のものに戻す
        yukie.wanderingActor.SetWanderingID(yukie.wanderingActor.currentWanderingPointID);
        yukie.SoundEmitter.OnEnterOuterPoint = StartLookInRoom;
        Debug.Log($"EndLookInRoom : {yukie.wanderingActor.currentWanderingPointID}");
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
