using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundDistance;

/// <summary>
/// 応接室で日記を読んだ後、雪絵の気配を見せるためのイベント
/// </summary>
public class Event_YukieHint : EventBase
{

    protected override void EventActive()
    {
        base.EventActive();
        instanceEventActor.gameObject.GetComponent<EA_YukieHint>().eventBase = this;
        InitiationContact();
    }
    public override void EventStart()
    {
       
        instanceEventActor.EventStart();
    }
    public override void EventUpdate()
    {
        instanceEventActor.EventUpdate();
    }
    public override void EventEnd()
    {
        instanceEventActor.EventEnd();
        Destroy(instanceEventActor.gameObject);
    }
}
