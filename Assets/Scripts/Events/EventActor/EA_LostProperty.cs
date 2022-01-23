using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_LostProperty : EventActorBase
{
    public Event_LostProperty eventBase = null;

    protected override void Initialize()
    {

    }
    public override void EventStart()
    {
        StartCoroutine(ItemLostEvent());
    }
    public override void EventUpdate()
    {

    }
    public override void EventEnd()
    {

    }

    private IEnumerator ItemLostEvent()
    {
        yield return null;
    }
}
