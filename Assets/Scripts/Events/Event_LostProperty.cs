using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵が落とし物をするイベント
/// </summary>
public class Event_LostProperty : EventBase
{
    public ItemObject lostPropertyObject = null;
    public Vector3 lostPropertyMoveInitPos = Vector3.zero;
    public Vector3 lostPropertyMoveTargetPos = Vector3.zero;

    protected override void EventActive()
    {
        base.EventActive();
        instanceEventActor.GetComponent<EA_LostProperty>().eventBase = this;
    }
}
