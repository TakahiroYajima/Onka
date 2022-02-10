using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_AfterGetAzuYuzuDiary2 : EventBase
{
    public ItemObject yuzuhaDiary = null;

    protected override void EventActive()
    {
        base.EventActive();
        instanceEventActor.GetComponent<EA_AfterGetAzuYuzuDiary2>().eventBase = this;
    }
}
