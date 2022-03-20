using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;
using SoundDistance;

public class EA_AfterGetEntranceKey : EventActorBase
{
    [HideInInspector] public Event_AfterGetEntranceKey eventBase = null;

    [SerializeField] private GameObject firstYukiePosition = null;

    [SerializeField] private CollisionEnterEvent yukieEventCollisionEnterEvent = null;
    private BoxCollider yukieEventCollider = null;
    [SerializeField] private CollisionEnterEvent eventEndCollisionEnterEvent = null;
    private BoxCollider eventEndCollider = null;

    protected override void Initialize()
    {
        yukieEventCollider = yukieEventCollisionEnterEvent.GetComponent<BoxCollider>();
        eventEndCollider = eventEndCollisionEnterEvent.GetComponent<BoxCollider>();

        StageManager.Instance.Yukie.ChangeState(EnemyState.CanNotAction);
        StageManager.Instance.Yukie.gameObject.SetActive(false);
    }
    public override void EventStart()
    {
        StageManager.Instance.Yukie.StopSound();
    }
    
    public override void EventEnd()
    {
        StageManager.Instance.Yukie.gameObject.SetActive(false);
    }
    /// <summary>
    /// 雪絵を表示するコライダーに当たった時
    /// </summary>
    public void OnYukieActiveColliderEvent()
    {
        if (Utility.Instance.IsTagNameMatch(yukieEventCollisionEnterEvent.HitCollision.gameObject, Tags.Player))
        {
            yukieEventCollider.enabled = false;
            StartCoroutine(YukieActiveEvent());
        }
    }
    private IEnumerator YukieActiveEvent()
    {
        StageManager.Instance.Yukie.transform.position = firstYukiePosition.transform.position;
        StageManager.Instance.Yukie.gameObject.SetActive(true);

        SoundDistanceManager.Instance.Emitter.SetPointID(eventBase.emitterPoint.ID);
        SoundDistanceManager.Instance.Emitter.SetNextTargetPointID(eventBase.emitterNextPoint.ID);
        SoundDistanceManager.Instance.Emitter.SetOuterPointID(eventBase.outerPoint.ID);
        SoundDistanceManager.Instance.Listener.SetCurrentPointID(eventBase.listenerPoint.ID);
        SoundDistanceManager.Instance.Listener.SetNextTargetPointID(eventBase.listenerNextPoint.ID);


        Vector3 lookPos = new Vector3(StageManager.Instance.Player.transform.position.x, StageManager.Instance.Yukie.transform.position.y, StageManager.Instance.Player.transform.position.z);
        StageManager.Instance.Yukie.transform.LookAt(lookPos);
        StageManager.Instance.Yukie.isEternalChaseMode = true;
        StageManager.Instance.Yukie.isIgnoreInRoom = true;
        //「逃がさない」ボイス再生
        StageManager.Instance.Yukie.PlaySoundOne(2);
        StageManager.Instance.Player.ChangeState(PlayerState.Event);
        yield return new WaitForSecondsRealtime(3.5f);
        StageManager.Instance.Yukie.ChangeState(EnemyState.ChasePlayer);
    }
    public void OnEndColliderEvent()
    {
        if (Utility.Instance.IsTagNameMatch(yukieEventCollisionEnterEvent.HitCollision.gameObject, Tags.Player))
        {
            eventEndCollider.enabled = false;
            StartCoroutine(EndColliderEnterEvent());
        }
    }
    private IEnumerator EndColliderEnterEvent()
    {
        eventBase.entranceDoor.CloseDoor();
        eventBase.entranceDoor.isEternalClosed = true;
        StageManager.Instance.Yukie.ChangeState(EnemyState.CanNotAction);
        StageManager.Instance.Yukie.StopSound();
        StageManager.Instance.Player.ChangeState(PlayerState.Event);
        //雪絵がドアに当たるくらいプレイヤーに接近していた場合、ドアと被さるので無理やり移動させる（4fは感覚値）
        if(StageManager.Instance.Yukie.transform.position.z < 4f)
        {
            Vector3 tPos = StageManager.Instance.Yukie.transform.position;
            tPos.z = 4.3f;
            StartCoroutine(StageManager.Instance.Yukie.movingObject.MoveWithTime(tPos, 0.3f));
        }
        yield return new WaitForSecondsRealtime(0.5f);
        SoundManager.Instance.PlaySeWithKeyOne("se_door_close");
        yield return new WaitForSecondsRealtime(0.5f);
        StageManager.Instance.Player.ChangeState(PlayerState.Free);
        FinishEvent();
    }
}
