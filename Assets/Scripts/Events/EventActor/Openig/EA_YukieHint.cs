using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundDistance;
using SoundSystem;

/// <summary>
/// 応接室で日記を読んだ直後、ドアの向こうで雪絵が髪の毛だけを見せて通り過ぎるイベント
/// </summary>
public class EA_YukieHint : EventActorBase
{
    public Event_YukieHint eventBase { private get; set; } = null;
    [SerializeField] private GameObject yukieFirstPosition = null;
    [SerializeField] private GameObject playerLookPosition = null;

    protected override void Initialize()
    {
        
    }

    public override void EventStart()
    {
       
        StartCoroutine(EventAction());
    }
    public override void EventUpdate()
    {

    }
    public override void EventEnd()
    {

    }

    private IEnumerator EventAction()
    {
        StageManager.Instance.Player.ChangeState(PlayerState.Event);
        StageManager.Instance.Yukie.transform.position = yukieFirstPosition.transform.position;
        StageManager.Instance.Yukie.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        StageManager.Instance.Yukie.gameObject.SetActive(true);
        StageManager.Instance.Yukie.ChangeState(EnemyState.CanNotAction);

        Vector3 initRotation = eventBase.playerCameraMovingObject.transform.forward;
        StartCoroutine(eventBase.playerCameraMovingObject.TurnAroundSmooth_Coroutine(playerLookPosition.transform.position, 10f));
        yield return StartCoroutine(StageManager.Instance.Yukie.movingObject.MoveWithTime(yukieFirstPosition.transform.position + Vector3.right * 3f, 3f));
        //yield return StartCoroutine(eventBase.playerCameraMovingObject.TurnAroundSmooth_Coroutine(initRotation, 6f));

        
        StageManager.Instance.Player.ChangeState(PlayerState.Free);
        

        FinishEvent();
    }
}
