using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SoundPlayerObject))]
public class Enemy_Shiori : Enemy
{
    private Dictionary<Enemy_ShioriState, StateBase> shioriStateDic = new Dictionary<Enemy_ShioriState, StateBase>();
    public Enemy_ShioriState currentShioriState { get; private set; } = Enemy_ShioriState.Init;

    [SerializeField] private SoundPlayerObject soundPlayerObject = null;
    public SoundPlayerObject SoundPlayerObject { get { return soundPlayerObject; } }

    public UnityAction onWalkEventEnded = null;
    public UnityAction<Collider> onColliderEnterEvent = null;

    private void Awake()
    {
        shioriStateDic.Add(Enemy_ShioriState.Init, new Enemy_ShioriStateInit());
        shioriStateDic.Add(Enemy_ShioriState.WalkEvent, new Enemy_ShioriStateWalkEvent());
        shioriStateDic.Add(Enemy_ShioriState.BeingNearEvent, new Enemy_ShioriStateBeingNearEvent());

        currentShioriState = Enemy_ShioriState.Init;
    }

    // Update is called once per frame
    void Update()
    {
        shioriStateDic[currentShioriState].UpdateAction();
    }

    /// <summary>
    /// State切り替え
    /// </summary>
    /// <param name="nextState"></param>
    public void ChangeState(Enemy_ShioriState nextState)
    {
        shioriStateDic[currentShioriState].EndAction();
        currentShioriState = nextState;
        shioriStateDic[currentShioriState].StartAction();
    }

    private void OnDestroy()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(onColliderEnterEvent != null)
        {
            onColliderEnterEvent(other);
        }
    }
}

public enum Enemy_ShioriState
{
    Init,
    WalkEvent,
    BeingNearEvent,
}
