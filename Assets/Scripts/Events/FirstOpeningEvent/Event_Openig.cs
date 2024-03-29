﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using SoundDistance;

public class Event_Openig : EventBase
{
    public enum OpeningEventState
    {
        Init = 0,
        UntilInHouse = 1,//外から玄関に入るまで

        End,
    }
    [HideInInspector] public OpeningEventState currentState = OpeningEventState.Init;

    public SoundDistancePoint initSoundPoint = null;
    
    public DoorObject endranceDoor = null;

    protected override void EventActive()
    {
        base.EventActive();
        instanceEventActor.gameObject.GetComponent<EA_Openig>().eventBase = this;
        InitiationContact();
    }
    public override void EventStart()
    {
        InGameUtil.DoCursorLock();
        currentState = OpeningEventState.Init;
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
