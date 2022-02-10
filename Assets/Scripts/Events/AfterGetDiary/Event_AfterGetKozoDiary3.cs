using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 孝蔵の日記3を取得した後のイベント
/// </summary>
public class Event_AfterGetKozoDiary3 : EventBase
{
    //地下にあるキメラみたいな像を動かす。音も鳴らす（ビビらせ要員）
    [SerializeField] private Transform moveObject = null;
    public Transform MoveObject { get { return moveObject; } }
    public Quaternion moveObjectFinishedRotationEular = Quaternion.Euler(0, 45, 0);

    protected override void EventActive()
    {
        base.EventActive();
        instanceEventActor.GetComponent<EA_AfterGetKozoDiary3>().eventBase = this;
    }

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

    protected override void AlreadyClearedMove()
    {
        base.AlreadyClearedMove();
        moveObject.rotation = moveObjectFinishedRotationEular;
    }
}
