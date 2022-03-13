using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;
using SoundDistance;

public class Event_YukieHintEnded : EventBase
{
    public SoundDistancePoint kitchenSoundPoint = null;
    public SoundDistancePoint drawingRoomSoundPoint = null;
    public SoundDistancePoint savePointSoundPoint = null;

    protected override void EventActive()
    {
        base.EventActive();
        instanceEventActor.gameObject.GetComponent<EA_YukieHintEnded>().eventBase = this;
        InitiationContact();
    }
}
