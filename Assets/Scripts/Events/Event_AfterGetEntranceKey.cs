using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_AfterGetEntranceKey : EventBase
{
    public DoorObject entranceDoor = null;

    protected override void EventActive()
    {
        base.EventActive();
        instanceEventActor.gameObject.GetComponent<EA_AfterGetEntranceKey>().eventBase = this;
        InitiationContact();
    }
}
