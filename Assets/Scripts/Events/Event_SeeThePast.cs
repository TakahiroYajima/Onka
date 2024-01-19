using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// 過去を見るイベント（家族の遺体が見えるイベント）
/// </summary>
[RequireComponent(typeof(SeeThePastController))]
public class Event_SeeThePast : EventBase
{
    [SerializeField]
    private SeeThePastController controller;
    protected override void EventActive()
    {
        base.EventActive();
        controller.OnActive();
        InitiationContact();
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
        instanceEventActor.EventEnd();
        controller.OnFinish();
        Destroy(instanceEventActor.gameObject);
    }

#if UNITY_EDITOR
    private void Reset()
    {
        if(!TryGetComponent<SeeThePastController>(out var past))
        {
            controller = this.gameObject.AddComponent<SeeThePastController>();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
#endif
}
