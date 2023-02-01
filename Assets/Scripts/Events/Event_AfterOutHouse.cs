using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玄関脱出後のイベント
/// </summary>
public class Event_AfterOutHouse : EventBase
{
    [SerializeField] private GameObject fieldColliderBase = null;
    private DoorObject entranceDoor = null;
    [SerializeField, ReadOnly] private string entranceDoorKey = "door_entrance";

    protected override void EventActive()
    {
        base.EventActive();
        entranceDoor = Onka.Manager.Event.EventManager.Instance.GetUseEventObject(entranceDoorKey).GetComponent<DoorObject>();
        InitiationContact();
    }

    public override void EventStart()
    {
        fieldColliderBase.SetActive(false);
        entranceDoor.CloseDoor();
        CrosshairManager.Instance.SetCrosshairActive(false);
        base.EventStart();
    }
}
