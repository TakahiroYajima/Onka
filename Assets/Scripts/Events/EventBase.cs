using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Onka.Manager.Event;

/// <summary>
/// ステージ上のイベント用スクリプトの基底クラス
/// </summary>
public abstract class EventBase : MonoBehaviour
{
    [SerializeField] protected bool isNeedEventActor = true;//eventActorPrefが必要なイベントか
    [SerializeField] protected string eventKey = "";
    public string EventKey { get { return eventKey; } }
    [SerializeField] protected string[] needItemKeys = null;//このイベントが発生するのに必要なアイテムキー
    [SerializeField] protected string[] needEventKeys = null;//このイベントが発生するのに必要なイベントキー
    [HideInInspector] public int managementID = -1;//配列で管理する際のID

    [SerializeField] protected EventActorBase eventActorPref = null;
    protected EventActorBase instanceEventActor = null;
    public bool canBeStarted { get; protected set; } = true;//EventStartが呼ばれたときにスタートできるか

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

        if (!EventManager.Instance.IsEventEnded(eventKey))
        {
            //必要なアイテムが全てそろっていたら次へ
            if (needItemKeys != null)
            {
                for (int i = 0; i < needItemKeys.Length; i++)
                {
                    if (!DataManager.Instance.GetItemData(needItemKeys[i]).geted)
                    {
                        return;
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
                        return;
                    }
                }
            }
            //条件を満たしているのでイベント発生
            EventActive();
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
    }
}
