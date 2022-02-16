using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundDistance;

public class EA_FirstChasedFromYukie : EventActorBase
{
    [HideInInspector] public Event_FirstChasedFromYukie eventBase = null;
    [SerializeField] private GameObject firstYukiePosition = null;

    protected override void Initialize()
    {
        
    }

    public override void EventStart()
    {
        SoundDistanceManager.Instance.Emitter.SetPointID(eventBase.soundPointSDP.ID);
        SoundDistanceManager.Instance.Listener.SetCurrentPointID(eventBase.soundPointSDP.ID);
        SoundDistanceManager.Instance.Maker.SetVolume(0f);
        SoundDistanceManager.Instance.ForceInitCalc();
        SoundDistanceManager.Instance.isActive = true;

        StageManager.Instance.Yukie.isEternalChaseMode = true;
        StageManager.Instance.Yukie.transform.position = firstYukiePosition.transform.position;
        StageManager.Instance.Yukie.gameObject.SetActive(true);
        Vector3 lookPos = new Vector3(StageManager.Instance.Player.transform.position.x, StageManager.Instance.Yukie.transform.position.y, StageManager.Instance.Player.transform.position.z);
        StageManager.Instance.Yukie.transform.LookAt(lookPos);
        StageManager.Instance.Yukie.onStateChangeCallback += OnYukieStateChangedCallback;
        StageManager.Instance.Yukie.ChangeState(EnemyState.RecognizedPlayer);
    }
    public override void EventUpdate()
    {

    }
    public override void EventEnd()
    {
        StageManager.Instance.Yukie.onStateChangeCallback -= OnYukieStateChangedCallback;
    }

    private void OnYukieStateChangedCallback(EnemyState state)
    {
        //雪絵がプレイヤーを見失ったら（部屋に隠れたプレイヤーを探し終えたら）イベント終了
        if(StageManager.Instance.Yukie.currentState == EnemyState.Wandering)
        {
            StageManager.Instance.Yukie.isEternalChaseMode = false;
            FinishEvent();
        }
    }
}
