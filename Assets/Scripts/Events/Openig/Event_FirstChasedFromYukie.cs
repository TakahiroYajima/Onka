using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundDistance;

/// <summary>
/// 雪絵の気配を追いかけた後、キッチンにてアイテムを拾ったら雪絵に追いかけられるイベント
/// </summary>
public class Event_FirstChasedFromYukie : EventBase
{
    public SoundDistancePoint soundPointSDP = null;

    protected override void EventActive()
    {
        base.EventActive();
        instanceEventActor.gameObject.GetComponent<EA_FirstChasedFromYukie>().eventBase = this;
        InitiationContact();
    }
    public override void EventStart()
    {
        base.EventStart();
        Debug.Log("Event_FirstChasedFromYukieスタート");
    }
}
