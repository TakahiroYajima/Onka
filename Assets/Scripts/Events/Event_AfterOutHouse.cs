using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玄関脱出後のイベント
/// </summary>
public class Event_AfterOutHouse : EventBase
{
    private GameObject fieldColliderBase = null;
    [SerializeField, ReadOnly] private string fieldColliderKey = "field_wall_colliders";
    private DoorObject entranceDoor = null;
    [SerializeField, ReadOnly] private string entranceDoorKey = "door_entrance";

    protected override void EventActive()
    {
        base.EventActive();
        fieldColliderBase = Onka.Manager.Event.EventManager.Instance.GetUseEventObject(fieldColliderKey).gameObject;
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
