using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundDistance;
using SoundSystem;

public class EA_FirstChasedFromYukie : EventActorBase
{
    [SerializeField] private GameObject firstYukiePosition = null;
    [SerializeField, ReadOnly] private string soundPointSDPKey = "sd_point_outer_0009";
    [SerializeField] private RunAwayTutorialManager tutorialManager = null;

    private float time = 0;
    private const float JudgeTime = 5f;

    protected override void Initialize()
    {
        tutorialManager.SetUp();
    }

    public override void EventStart()
    {
        var soundPointSDP = SoundDistanceManager.Instance.GetSoundDistancePoint(soundPointSDPKey);

        SoundDistanceManager.Instance.Emitter.SetPointID(soundPointSDP.ID);
        SoundDistanceManager.Instance.Listener.SetCurrentPointID(soundPointSDP.ID);
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

        tutorialManager.StartTutorial();
    }
    public override void EventUpdate()
    {
        if (tutorialManager.IsAction && StageManager.Instance.Yukie.currentState == EnemyState.InRoomWandering && time > JudgeTime)
        {
            tutorialManager.EndTutorial();
        }
        time += Time.deltaTime;
    }
    public override void EventEnd()
    {
        StageManager.Instance.Yukie.onStateChangeCallback -= OnYukieStateChangedCallback;
        SoundManager.Instance.PlayEnvironmentWithKey("ambient_in_house");
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
