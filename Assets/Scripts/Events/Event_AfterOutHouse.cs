using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玄関脱出後のイベント
/// </summary>
public class Event_AfterOutHouse : EventBase
{
    [SerializeField] private GameObject fieldColliderBase = null;
    [SerializeField] private DoorObject entranceDoor = null;

    protected override void EventActive()
    {
        base.EventActive();
        instanceEventActor.GetComponent<EA_AfterOutHouse>().eventBase = this;
        InitiationContact();
    }

    public override void EventStart()
    {
        fieldColliderBase.SetActive(false);
        entranceDoor.CloseDoor();
        base.EventStart();
    }
}
