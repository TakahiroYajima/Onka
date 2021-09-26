using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy_Shiori : MonoBehaviour
{
    private Dictionary<Enemy_ShioriState, StateBase> shioriStateDic = new Dictionary<Enemy_ShioriState, StateBase>();
    public Enemy_ShioriState currentState { get; private set; } = Enemy_ShioriState.Init;

    public UnityAction onWalkEventEnded = null;

    private void Awake()
    {
        shioriStateDic.Add(Enemy_ShioriState.Init, new Enemy_ShioriStateInit());
        shioriStateDic.Add(Enemy_ShioriState.WalkEvent, new Enemy_ShioriStateWalkEvent());
        shioriStateDic.Add(Enemy_ShioriState.BeingNearEvent, new Enemy_ShioriStateBeingNearEvent());

        currentState = Enemy_ShioriState.Init;
    }

    // Start is called before the first frame update
    void Start()
    {
        StageManager.Instance.Shiori = this;
    }

    // Update is called once per frame
    void Update()
    {
        shioriStateDic[currentState].UpdateAction();
    }

    /// <summary>
    /// State切り替え
    /// </summary>
    /// <param name="nextState"></param>
    public void ChangeState(Enemy_ShioriState nextState)
    {
        shioriStateDic[currentState].EndAction();
        currentState = nextState;
        shioriStateDic[currentState].StartAction();
    }

    private void OnDestroy()
    {
        StageManager.Instance.Shiori = null;
    }
}

public enum Enemy_ShioriState
{
    Init,
    WalkEvent,
    BeingNearEvent,
}
