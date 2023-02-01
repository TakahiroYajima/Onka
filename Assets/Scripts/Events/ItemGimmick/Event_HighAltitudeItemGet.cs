using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event_HighAltitudeItemGet : EventBase
{
    private void Awake()
    {
        this.isAutoEvent = false;//インスペクターで設定するが、念のため初期化
    }

    protected override void EventActive()
    {
        base.EventActive();
        instanceEventActor.GetComponent<EA_HighAltitudeItemGet>().eventBase = this;
    }
}
