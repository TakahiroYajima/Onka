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
        StartCoroutine(MoveStatus(eventBase.MoveObject , () =>
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

    private IEnumerator MoveStatus(Transform moveObject, UnityAction onComplete)
    {
        float currentTime = 0f;
        while (currentTime < 0.5)
        {
            moveObject.Rotate(0, 45 * (Time.deltaTime / 0.5f), 0);
            currentTime += Time.deltaTime;
            yield return null;
        }
        moveObject.rotation = eventBase.moveObjectFinishedRotationEular;
        onComplete();
    }

    public void OnCollisionEnterEvent()
    {
        if (Utility.Instance.IsTagNameMatch(collisionEnterEvent.HitCollision.gameObject, Tags.Player))
        {
            parent.SetCanBeStarted(true);
            parent.InitiationContact();
        }
    }
}
