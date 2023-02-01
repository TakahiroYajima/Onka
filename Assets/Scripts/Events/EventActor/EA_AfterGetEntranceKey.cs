using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;
using SoundDistance;

public class EA_AfterGetEntranceKey : EventActorBase
{
    //[HideInInspector] public Event_AfterGetEntranceKey eventBase = null;

    [SerializeField] private GameObject firstYukiePosition = null;

    [SerializeField] private CollisionEnterEvent yukieEventCollisionEnterEvent = null;
    private BoxCollider yukieEventCollider = null;
    [SerializeField] private CollisionEnterEvent eventEndCollisionEnterEvent = null;
    private BoxCollider eventEndCollider = null;

    [SerializeField, ReadOnly] private string entranceDoorKey = "door_entrance";
    [SerializeField, ReadOnly] private string emitterPointKey = "sd_point_room1_0002";
    [SerializeField, ReadOnly] private string emitterNextPointKey = "sd_point_2f_0001";
    [SerializeField, ReadOnly] private string listenerPointKey = "sd_point_2f_0000";
    [SerializeField, ReadOnly] private string listenerNextPointKey = "sd_point_2f_0000";
    [SerializeField, ReadOnly] private string outerPointKey = "sd_point_outer_0007";

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
        var emitterPoint = SoundDistanceManager.Instance.GetSoundDistancePoint(emitterPointKey);
        var emitterNextPoint = SoundDistanceManager.Instance.GetSoundDistancePoint(emitterNextPointKey);
        var outerPoint = SoundDistanceManager.Instance.GetSoundDistancePoint(outerPointKey);
        var listenerPoint = SoundDistanceManager.Instance.GetSoundDistancePoint(listenerPointKey);
        var listenerNextPoint = SoundDistanceManager.Instance.GetSoundDistancePoint(listenerNextPointKey);
        StageManager.Instance.Yukie.transform.position = firstYukiePosition.transform.position;
        StageManager.Instance.Yukie.gameObject.SetActive(true);

        SoundDistanceManager.Instance.Emitter.SetPointID(emitterPoint.ID);
        SoundDistanceManager.Instance.Emitter.SetNextTargetPointID(emitterNextPoint.ID);
        SoundDistanceManager.Instance.Emitter.SetOuterPointID(outerPoint.ID);
        SoundDistanceManager.Instance.Listener.SetCurrentPointID(listenerPoint.ID);
        SoundDistanceManager.Instance.Listener.SetNextTargetPointID(listenerNextPoint.ID);


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
        var entranceDoor = Onka.Manager.Event.EventManager.Instance.GetUseEventObject(entranceDoorKey).GetComponent<DoorObject>();
        entranceDoor.CloseDoor();
        entranceDoor.isEternalClosed = true;
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
