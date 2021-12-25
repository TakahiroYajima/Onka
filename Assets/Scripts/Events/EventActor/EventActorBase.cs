using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 実際にイベントを進行させるスクリプトの基底クラス
/// </summary>
public abstract class EventActorBase : MonoBehaviour
{
    protected EventBase parent = null;

    public void Standby(EventBase _parent)
    {
        parent = _parent;
        Initialize();
    }

    protected abstract void Initialize();

    public abstract void EventStart();
    public virtual void EventUpdate() { }
    public abstract void EventEnd();

    public virtual void FinishEvent()
    {
        parent.EventClearContact();
    }
}
