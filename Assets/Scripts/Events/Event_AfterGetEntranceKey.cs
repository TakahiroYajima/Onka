using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundDistance;

public class Event_AfterGetEntranceKey : EventBase
{
    public DoorObject entranceDoor = null;
    public SoundDistancePoint emitterPoint = null;
    public SoundDistancePoint emitterNextPoint = null;
    public SoundDistancePoint listenerPoint = null;
    public SoundDistancePoint listenerNextPoint = null;
    public SoundDistancePoint outerPoint = null;

    protected override void EventActive()
    {
        base.EventActive();
        instanceEventActor.gameObject.GetComponent<EA_AfterGetEntranceKey>().eventBase = this;
        InitiationContact();
    }
}
