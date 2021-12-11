using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using SoundDistance;

public class Event_Openig : EventBase
{
    public enum OpeningEventState
    {
        Init,
        UntilInHouse,//外から玄関に入るまで

        End,
    }
    private OpeningEventState currentState = OpeningEventState.Init;

    public SoundDistanceManager soundDistanceManager = null;

    protected override void EventActive()
    {
        base.EventActive();
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
        Destroy(instanceEventActor.gameObject);
    }

    public void ChangeState()
    {

    }
    public void ChangeNextState()
    {
        int nextID = (int)currentState + 1;
        if (Enum.IsDefined(typeof(OpeningEventState), nextID))
        {
            currentState = (OpeningEventState)Enum.ToObject(typeof(OpeningEventState), nextID);
        }
        else
        {
            currentState = OpeningEventState.End;
        }
    }
}
