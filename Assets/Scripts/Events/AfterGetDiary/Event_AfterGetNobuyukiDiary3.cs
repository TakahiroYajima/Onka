using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Event_AfterGetNobuyukiDiary3 : EventBase
{
    public override void EventStart()
    {
        Debug.Log("信之の日記3取得後イベント");
        instanceEventActor.EventStart();
    }
    public override void EventUpdate()
    {

    }
    public override void EventEnd()
    {
        Debug.Log("信之の日記3取得後イベント終了");
        Destroy(instanceEventActor.gameObject);
    }
   
}
