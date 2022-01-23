using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_AfterGetEntranceKey : EventBase
{
    protected override void EventActive()
    {
        base.EventActive();
        InitiationContact();
    }
}
