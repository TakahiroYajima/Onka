using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;
using Onka.Manager.Data;

/// <summary>
/// 1階倉庫部屋で彩珠波と柚子羽の日記2を見た後、部屋の奥から柚子羽が追いかけてくるイベント
/// </summary>
public class EA_AfterGetAzuYuzuDiary2 : EventActorBase
{
    [SerializeField] private Enemy_Yuzuha yuzuha = null;
    [SerializeField] private SoundPlayerObject yuzuhaSoundPlayer = null;
    

    private BoxCollider kagomekagomeCollider = null;
    [SerializeField] private CollisionEnterEvent kagomekagomeCollisionEnterEvent = null;
    private BoxCollider yuzuhaActiveCollider = null;
    [SerializeField] private CollisionEnterEvent yuzuhaActiveCollisionEnterEvent = null;
    private BoxCollider[] startChaseToPlayerCollider = null;
    [SerializeField] private CollisionEnterEvent[] startChaseToplayerCollisionEnterEvents = null;
    private BoxCollider eventEndCollider = null;
    [SerializeField] private CollisionEnterEvent eventEndCollisionEnterEvent = null;

    [SerializeField] private AudioClip kagomekagomeSound = null;

    [SerializeField, ReadOnly] private string yuzuhaDiaryKey = "diary_azuhayuzuha_2";

    private enum YD2State
    {
        Inactive,
        Init,
        Kagomekagome,//かごめかごめが聞こえる段階
        AfterReadDiaryWaitTime,//日記を読み終わり、柚子羽出現判定ポイントまで移動する時間
        YuzuhaAppearance,//「後ろの正面だあれ」で柚子羽生成
        BeforeChaseWaitTime,
        Chase,//追いかける
    }
    private YD2State currentState = YD2State.Init;

    protected override void Initialize()
    {
        StageManager.Instance.Yuzuha = yuzuha;

        kagomekagomeCollider = kagomekagomeCollisionEnterEvent.GetComponent<BoxCollider>();
        yuzuhaActiveCollider = yuzuhaActiveCollisionEnterEvent.GetComponent<BoxCollider>();
        startChaseToPlayerCollider = new BoxCollider[startChaseToplayerCollisionEnterEvents.Length];
        for(int i = 0; i < startChaseToPlayerCollider.Length; i++)
        {
            startChaseToPlayerCollider[i] = startChaseToplayerCollisionEnterEvents[i].GetComponent<BoxCollider>();
            startChaseToPlayerCollider[i].enabled = false;
        }
        eventEndCollider = eventEndCollisionEnterEvent.GetComponent<BoxCollider>();

        yuzuhaActiveCollider.enabled = false;
        eventEndCollider.enabled = false;

        currentState = YD2State.Init;
    }

    public override void EventStart()
    {

    }

    public override void EventUpdate()
    {
        switch (currentState)
        {
            case YD2State.Init:break;
            case YD2State.Kagomekagome:
                //日記を読むまで待つ
                if (DataManager.Instance.GetItemData(yuzuhaDiaryKey).geted)
                {
                    yuzuhaActiveCollider.enabled = true;
                    currentState = YD2State.AfterReadDiaryWaitTime;
                }
                break;
            case YD2State.AfterReadDiaryWaitTime:break;
            case YD2State.YuzuhaAppearance:break;
            case YD2State.BeforeChaseWaitTime:
                if (StageManager.Instance.Player.IsInSightFromPlayer(yuzuha.transform.position))
                {
                    OnPlayerInSightedYuzuha();
                }
                break;
            case YD2State.Chase:break;
        }
    }

    public override void EventEnd()
    {
        StageManager.Instance.Yuzuha = null;
    }

    private IEnumerator StartupYuzu()
    {
        yuzuha.gameObject.SetActive(true);
        yuzuha.transform.LookAt(new Vector3(StageManager.Instance.Player.CameraObj.transform.position.x, yuzuha.transform.position.y, StageManager.Instance.Player.CameraObj.transform.position.z));
        if (StageManager.Instance.Player.IsInSightFromPlayer(yuzuha.transform.position))
        {
            OnPlayerInSightedYuzuha();
        }
        else
        {
            yuzuhaSoundPlayer.PlaySE(1);
            StageManager.Instance.Player.ChangeState(PlayerState.Event);
            yield return new WaitForSecondsRealtime(3f);
            StageManager.Instance.Player.ChangeState(PlayerState.Free);
            currentState = YD2State.BeforeChaseWaitTime;
        }
    }

    private void StartChaseToPlayer()
    {
        yuzuha.ChangeState(EnemyState.ChasePlayer);
        currentState = YD2State.Chase;
        SoundManager.Instance.StopVoice();
        yuzuhaSoundPlayer.PlaySE(0);
    }

    public void OnKagomeKagomeCollisionEnterEvent()
    {
        if (Utility.Instance.IsTagNameMatch(kagomekagomeCollisionEnterEvent.HitCollider.gameObject, Tags.Player))
        {
            parent.SetCanBeStarted(true);
            parent.InitiationContact();
            StageManager.Instance.Yukie.SetVolumeONEnable(false);
            currentState = YD2State.Kagomekagome;
            SoundManager.Instance.PlayVoiceClip(kagomekagomeSound);
        }
    }
    /// <summary>
    /// 日記を読まずに部屋を出たら一旦ストップ
    /// </summary>
    public void OnKagomeKagomeCollisionExitEvent()
    {
        if (!DataManager.Instance.GetItemData(yuzuhaDiaryKey).geted)
        {
            if (Utility.Instance.IsTagNameMatch(kagomekagomeCollisionEnterEvent.HitCollider.gameObject, Tags.Player))
            {
                StageManager.Instance.Yukie.SetVolumeONEnable(true);
                currentState = YD2State.Inactive;
                SoundManager.Instance.StopVoice();
            }
        }
    }
    public void OnYuzuhaActiveColliderEvent()
    {
        if (Utility.Instance.IsTagNameMatch(yuzuhaActiveCollisionEnterEvent.HitCollision.gameObject, Tags.Player))
        {
            SoundManager.Instance.StopVoice();
            currentState = YD2State.YuzuhaAppearance;
            StartCoroutine(StartupYuzu());
            kagomekagomeCollider.enabled = false;
            yuzuhaActiveCollider.enabled = false;
            SetChaseToPlayerColliderActive(true);
            eventEndCollider.enabled = true;
        }
    }
    /// <summary>
    /// プレイヤーが柚子羽の方を向かずに一定以上動いたら強制で追いかけさせる
    /// </summary>
    public void OnChaseToPlayerColliderEvent(int collisionArrayNum)
    {
        if (Utility.Instance.IsTagNameMatch(startChaseToplayerCollisionEnterEvents[collisionArrayNum].HitCollider.gameObject, Tags.Player))
        {
            OnPlayerInSightedYuzuha();
        }
    }
    /// <summary>
    /// プレイヤーが柚子羽を視界に収めた際のイベント
    /// </summary>
    private void OnPlayerInSightedYuzuha()
    {
        if (currentState != YD2State.Chase)
        {
            SetChaseToPlayerColliderActive(false);
            StartChaseToPlayer();
        }
    }
    private void SetChaseToPlayerColliderActive(bool _active)
    {
        for (int i = 0; i < startChaseToPlayerCollider.Length; i++)
        {
            startChaseToPlayerCollider[i].enabled = _active;
        }
    }
    public void OnEventEndColliderEvent()
    {
        if (Utility.Instance.IsTagNameMatch(eventEndCollisionEnterEvent.HitCollision.gameObject, Tags.Player))
        {
            yuzuha.ChangeState(EnemyState.CanNotAction);
            yuzuha.gameObject.SetActive(false);
            eventEndCollider.enabled = false;
            StageManager.Instance.Yuzuha = null;
            StageManager.Instance.Yukie.SetVolumeONEnable(true);
            FinishEvent();
        }
    }
}
