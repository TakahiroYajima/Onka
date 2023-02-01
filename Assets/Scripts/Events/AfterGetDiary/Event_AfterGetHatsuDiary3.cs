using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SoundSystem;

public class Event_AfterGetHatsuDiary3 : EventBase
{
    protected override void EventActive()
    {
        base.EventActive();
        instanceEventActor.GetComponent<EA_AfterGetHatsuDiary3>().eventBase = this;
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
