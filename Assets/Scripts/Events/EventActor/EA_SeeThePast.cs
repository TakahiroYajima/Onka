using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SeeThePastActor))]
public class EA_SeeThePast : EventActorBase
{
    [SerializeField]
    private SeeThePastActor seeThePastActor;

    protected override void Initialize()
    {

    }

    public override void EventStart()
    {
        
    }

    public override void EventUpdate()
    {

    }

    public override void EventEnd()
    {
        
    }

    public void OnEnterCollision()
    {
        parent.InitiationContact();
        seeThePastActor.Execute(FinishEvent);
    }
}
