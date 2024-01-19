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
    [SerializeField]
    private SoundPlayerObject firstYukieVoicePlayer;

    private float time = 0;
    private const float JudgeTime = 5f;

    private enum State
    {
        LookYukie,
        Chase,
    }
    private State currentState = State.LookYukie;

    protected override void Initialize()
    {
        tutorialManager.SetUp();
    }

    public override void EventStart()
    {
        StageManager.Instance.Player.ChangeState(PlayerState.Event);//最初だけプレイヤーのステートを操作。後は雪絵のステート変化で自動的に変わるので終了時のプレイヤーステート操作は不要
        var soundPointSDP = SoundDistanceManager.Instance.GetSoundDistancePoint(soundPointSDPKey);

        SoundDistanceManager.Instance.Emitter.SetPointID(soundPointSDP.ID);
        SoundDistanceManager.Instance.Listener.SetCurrentPointID(soundPointSDP.ID);
        SoundDistanceManager.Instance.Maker.SetVolume(0f);
        SoundDistanceManager.Instance.ForceInitCalc();
        SoundDistanceManager.Instance.isActive = true;

        
        StageManager.Instance.Yukie.transform.position = firstYukiePosition.transform.position;
        StageManager.Instance.Yukie.ChangeState(EnemyState.CanNotAction);
        StageManager.Instance.Yukie.gameObject.SetActive(false);
        Vector3 lookPos = new Vector3(StageManager.Instance.Player.Position.x, StageManager.Instance.Yukie.transform.position.y, StageManager.Instance.Player.Position.z);
        StageManager.Instance.Yukie.transform.LookAt(lookPos);
        StageManager.Instance.Yukie.onStateChangeCallback += OnYukieStateChangedCallback;

        StartCoroutine(LookYukieAction());
    }
    public override void EventUpdate()
    {
        if (currentState == State.Chase)
        {
            if (tutorialManager.IsAction && StageManager.Instance.Yukie.currentState == EnemyState.InRoomWandering && time > JudgeTime)
            {
                tutorialManager.EndTutorial();
            }
            time += Time.deltaTime;
        }
    }
    public override void EventEnd()
    {
        StageManager.Instance.Yukie.onStateChangeCallback -= OnYukieStateChangedCallback;
        SoundManager.Instance.PlayEnvironmentWithKey("ambient_in_house");
    }

    private IEnumerator LookYukieAction()
    {
        var player = StageManager.Instance.Player;
        player.FirstPersonAIO.isEnable = false;
        //うめき声的なのを再生
        firstYukieVoicePlayer.PlayOne(0);
        CrosshairManager.Instance.SetCrosshairActive(false);
        yield return new WaitForSeconds(2.1f);
        StageManager.Instance.Yukie.gameObject.SetActive(true);
        yield return player.TurnAroundSmooth_Coroutine(firstYukiePosition.transform.position, 1f, true);
        yield return new WaitForSeconds(0.8f);
        CrosshairManager.Instance.SetCrosshairActive(true);
        player.FirstPersonAIO.isEnable = true;
        tutorialManager.StartTutorial();
        yield return new WaitForSeconds(0.2f);
        StageManager.Instance.Yukie.isEternalChaseMode = true;
        StageManager.Instance.Yukie.ChangeState(EnemyState.RecognizedPlayer);
        
        currentState = State.Chase;
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
