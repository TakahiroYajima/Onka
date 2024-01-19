using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Onka.Manager.Event;
using Onka.Manager.Data;

/// <summary>
/// ステージ上のイベント用スクリプトの基底クラス
/// </summary>
public abstract class EventBase : MonoBehaviour
{
    [SerializeField] protected bool isAutoEvent = true;//フラグによって自動で発動するイベントか（falseの場合は、どこかでForceStartEvent()を呼ぶ必要がある）
    [SerializeField] protected bool isNeedEventActor = true;//eventActorPrefが必要なイベントか
    [SerializeField] protected string eventKey = "";
    public string EventKey { get { return eventKey; } }
    [SerializeField] protected string[] needItemKeys = null;//このイベントが発生するのに必要なアイテムキー
    [SerializeField] protected string[] needEventKeys = null;//このイベントが発生するのに必要なイベントキー
    [SerializeField]
    protected string forceClearItemKey = string.Empty;//このアイテムを持っていたら強制でイベントをクリアさせる（Ver1のセーブデータが残っていることによるバグ対策）
    [System.NonSerialized] public int managementID = -1;//配列で管理する際のID

    [SerializeField] protected EventActorBase eventActorPref = null;
    protected EventActorBase instanceEventActor = null;
    public bool canBeStarted { get; protected set; } = true;//EventStartが呼ばれたときにスタートできるか

    protected UnityAction onEventStartedCallback = null;
    public void AddOnEventActiveCallback(UnityAction callback)
    {
        if (onEventStartedCallback == null) { onEventStartedCallback = callback; }
        else { onEventStartedCallback += callback; }
    }
    protected UnityAction onEventEndCallback = null;
    public void AddOnEventEndCallback(UnityAction callback)
    {
        if(onEventEndCallback == null) { onEventEndCallback = callback; }
        else { onEventEndCallback += callback; }
    }

    /// <summary>
    /// 最初の初期化の段階でクリア済みかを判定。クリアしていたらクリア後の状態にする（オブジェクトの移動など）
    /// </summary>
    public void InitProgress()
    {
        //特定のアイテムを持っていたらクリア済みと判定させる（Ver2.0.0現在はSeeThePast専用）
        if (!string.IsNullOrEmpty(forceClearItemKey) && DataManager.Instance.IsGetedItem(forceClearItemKey))
        {
            AlreadyClearedMove();
            return;
        }
        //通常のクリア判定フロー
        if (EventManager.Instance.IsEventEnded(eventKey))
        {
            AlreadyClearedMove();
        }
    }
    /// <summary>
    /// 既にクリアしていた時に呼び出される（クリア後の状態にする）
    /// </summary>
    protected virtual void AlreadyClearedMove()
    {

    }

    /// <summary>
    /// 自身のイベントが発行可能か判断する
    /// </summary>
    public void ProgressEvent()
    {
        if (EventManager.Instance.IsEventEnded(eventKey))
        {
            EventManager.Instance.ClearedEventDestroy(managementID);
            return;
        }
        if (instanceEventActor != null)
        {
            InitiationContact();
            return;
        }

        if (!isAutoEvent) return;

        if (!EventManager.Instance.IsEventEnded(eventKey))
        {
            if (IsStartableEvent())
            {
                //条件を満たしているのでイベント発生
                EventActive();
            }
        }
    }
    private bool IsStartableEvent()
    {
        //必要なアイテムが全てそろっていたら次へ
        if (needItemKeys != null)
        {
            for (int i = 0; i < needItemKeys.Length; i++)
            {
                if (!DataManager.Instance.GetItemData(needItemKeys[i]).geted)
                {
                    return false;
                }
            }
        }

        //必要なイベントが全て終了していたら次へ
        if (needEventKeys != null)
        {
            for (int i = 0; i < needEventKeys.Length; i++)
            {
                if (!EventManager.Instance.IsEventEnded(needEventKeys[i]))
                {
                    return false;
                }
            }
        }
        return true;
    }
    /// <summary>
    /// 外部から呼び出す専用
    /// 任意のタイミングで発行するイベントを開始する
    /// </summary>
    public void ForceStartEvent(UnityAction onComplete)
    {
        if (isAutoEvent) return;
        if (IsStartableEvent())
        {
            onEventEndCallback = onComplete;
            EventActive();
            InitiationContact();
        }
    }
    /// <summary>
    /// イベント発生
    /// </summary>
    protected virtual void EventActive()
    {
        if (isNeedEventActor)
        {
            instanceEventActor = Instantiate(eventActorPref, transform) as EventActorBase;
            instanceEventActor.Standby(this);
        }
    }
    /// <summary>
    /// EventManagerにイベント開始の連絡をする
    /// </summary>
    public void InitiationContact()
    {
        EventManager.Instance.RequestEventStart(managementID);
    }

    public void SetCanBeStarted(bool _canBeStarted)
    {
        canBeStarted = _canBeStarted;
    }

    public virtual void EventStart()
    {
        if (isNeedEventActor && instanceEventActor != null)
        {
            instanceEventActor.EventStart();
        }
        if(onEventStartedCallback != null)
        {
            onEventStartedCallback();
        }
    }
    public virtual void EventUpdate()
    {
        if (isNeedEventActor && instanceEventActor != null)
        {
            instanceEventActor.EventUpdate();
        }
    }
    /// <summary>
    /// EventManagerにイベント終了の連絡をする
    /// </summary>
    public virtual void EventClearContact()
    {
        EventManager.Instance.EventClear(EventKey);
    }
    public virtual void EventEnd()
    {
        if (isNeedEventActor && instanceEventActor != null)
        {
            instanceEventActor.EventEnd();
            Destroy(instanceEventActor.gameObject);
        }
        if(onEventEndCallback != null)
        {
            onEventEndCallback();
        }
    }
}
