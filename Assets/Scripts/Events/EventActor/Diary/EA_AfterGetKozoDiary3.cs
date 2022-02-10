using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EA_AfterGetKozoDiary3 : EventActorBase
{
    [SerializeField] private BoxCollider boxCollider = null;
    [SerializeField] private CollisionEnterEvent collisionEnterEvent = null;
    [SerializeField] private SoundPlayerObject soundPlayer = null;

    public Event_AfterGetKozoDiary3 eventBase { private get; set; } = null;

    protected override void Initialize()
    {
        boxCollider.enabled = true;
        parent.SetCanBeStarted(false);
    }

    public override void EventStart()
    {
        soundPlayer.PlaySE("se_lug");
        StartCoroutine(MoveStatus(() =>
        {
            parent.EventClearContact();
        }));
    }
    public override void EventUpdate()
    {

    }
    public override void EventEnd()
    {

    }

    private IEnumerator MoveStatus(UnityAction onComplete)
    {
        float currentTime = 0f;
        while (currentTime < 0.5)
        {
            eventBase.MoveObject.Rotate(0, 45 * (Time.deltaTime / 0.5f), 0);
            currentTime += Time.deltaTime;
            yield return null;
        }
        eventBase.MoveObject.rotation = eventBase.moveObjectFinishedRotationEular;
        onComplete();
    }

    public void OnCollisionEnterEvent()
    {
        if (Utility.Instance.IsTagNameMatch(collisionEnterEvent.HitCollision.gameObject, Tags.Player))
        {
            Debug.Log("孝蔵の日記3取得後イベント");
            parent.SetCanBeStarted(true);
            parent.InitiationContact();
        }
    }
}
