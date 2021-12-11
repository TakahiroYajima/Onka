using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_AfterGetAzuYuzuDiary3 : EventBase
{
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


}
