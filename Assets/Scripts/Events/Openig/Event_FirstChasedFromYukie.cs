using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 雪絵の気配を追いかけた後、キッチンにてアイテムを拾ったら雪絵に追いかけられるイベント
/// </summary>
public class Event_FirstChasedFromYukie : EventBase
{
    protected override void EventActive()
    {
        base.EventActive();
        InitiationContact();
    }
    public override void EventStart()
    {
        base.EventStart();
        Debug.Log("Event_FirstChasedFromYukieスタート");
    }
}
