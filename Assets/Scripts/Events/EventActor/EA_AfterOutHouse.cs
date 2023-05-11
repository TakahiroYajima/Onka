using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundDistance;
using SoundSystem;

public class EA_AfterOutHouse : EventActorBase
{
    [SerializeField] private EndingEventManager endingEventManager = null;

    protected override void Initialize()
    {

    }
    public override void EventStart()
    {
        StageManager.Instance.AllEnemyForceChangeStateCanNotAction();
        StageManager.Instance.AllEnemyInactive();
        StageManager.Instance.Player.ChangeState(PlayerState.Event);
        SoundDistanceManager.Instance.SetInactive();
        SoundManager.Instance.StopBGMWithFadeOut(1f);
        endingEventManager.Initialize(StageManager.Instance.Player.CameraObj);
        endingEventManager.StartAction(EndingEventType.FinalEvent, () =>
         {
             DestroyImmediate(endingEventManager.gameObject);
             var manager = GameSceneManager.Instance as GameSceneManager;
             manager.StartEnding();
         });
    }
    public override void EventUpdate()
    {

    }
    public override void EventEnd()
    {

    }
}
