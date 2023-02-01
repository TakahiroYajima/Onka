using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Event_AfterGetNobuyukiDiary3 : EventBase
{
    public override void EventStart()
    {
        instanceEventActor.EventStart();
    }
    public override void EventUpdate()
    {

    }
    public override void EventEnd()
    {
        Destroy(instanceEventActor.gameObject);
    }
   
}
