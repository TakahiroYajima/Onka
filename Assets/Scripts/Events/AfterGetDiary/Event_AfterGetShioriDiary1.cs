using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 詩織の日記1を取得した後のイベント
/// </summary>
public class Event_AfterGetShioriDiary1 : EventBase
{
    public Event_AnyItemActive azuyuzuKeyActiveEvent = null;

    protected override void EventActive()
    {
        base.EventActive();
        instanceEventActor.GetComponent<EA_AfterGetShioriDiary3>().eventBase = this;
        InitiationContact();
    }

    public override void EventStart()
    {
        instanceEventActor.EventStart();
    }

    public override void EventUpdate()
    {
        
    }
    public override void EventEnd()
    {
        instanceEventActor.EventEnd();
        Destroy(instanceEventActor.gameObject);
    }
}
