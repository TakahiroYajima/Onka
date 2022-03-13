using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;

public class EA_Openig : EventActorBase
{
    public Event_Openig eventBase { private get; set; }
    private BoxCollider inEntranceJudgeCollider = null;
    [SerializeField] private CollisionEnterEvent inEntranceJudgeColliderEvent = null;
    [SerializeField] private GameObject initPlayerPosition = null;

    protected override void Initialize()
    {
        inEntranceJudgeCollider = inEntranceJudgeColliderEvent.gameObject.GetComponent<BoxCollider>();

    }

    public override void EventStart()
    {
        StartCoroutine(FirstEventAction());
    }
    public override void EventUpdate()
    {
        
    }
    public override void EventEnd()
    {

    }

    private IEnumerator FirstEventAction()
    {
        StageManager.Instance.Player.transform.position = initPlayerPosition.transform.position;

        yield return null;//他のスクリプト初期化待ち

        //各アクターの初期化
        eventBase.endranceDoor.isForceOpenable = true;
        StageManager.Instance.InactiveYukieAndInitListenerPointID();
        eventBase.ChangeNextState();
        yield return new WaitForSeconds(2f);//念のため
        StageManager.Instance.Player.ChangeState(PlayerState.Free);

        inEntranceJudgeCollider.enabled = true;
    }

    private IEnumerator AfterInEntranceEventAction()
    {
        inEntranceJudgeCollider.enabled = false;
        eventBase.endranceDoor.isForceOpenable = false;
        StageManager.Instance.Player.ForcedStopFPS();
        eventBase.endranceDoor.CloseDoor();
        yield return new WaitForSecondsRealtime(0.5f);
        SoundManager.Instance.PlaySeWithKeyOne("se_door_close");
        yield return new WaitForSecondsRealtime(0.9f);
        StageManager.Instance.Player.FirstPersonAIO.enabled = true;
        eventBase.EventClearContact();
    }

    public void OnInEntranceColliderEvent()
    {
        if (Utility.Instance.IsTagNameMatch(inEntranceJudgeColliderEvent.HitCollision.gameObject, Tags.Player))
        {
            StartCoroutine(AfterInEntranceEventAction());
        }
    }
}
