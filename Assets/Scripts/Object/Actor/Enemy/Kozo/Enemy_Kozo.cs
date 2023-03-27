using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Kozo : Enemy
{
    public WalkAnimObj walkAnimObj { get; private set; } = null;
    public MovingObject movingObject { get; private set; } = null;
    public SoundPlayerObject soundPlayerObject { get; private set; } = null;
    public Rigidbody rigidbody { get; private set; } = null;

    public Dictionary<EnemyState, StateBase> kozoStateDic { get; private set; } = new Dictionary<EnemyState, StateBase>();

    public Action<Collider> onColliderEnterEvent = null;

    protected override void Awake()
    {
        base.Awake();
        currentState = EnemyState.Init;
    }

    /// <summary>
    /// 初期設定
    /// </summary>
    public void SetUp()
    {
        walkAnimObj = GetComponent<WalkAnimObj>();
        movingObject = GetComponent<MovingObject>();
        soundPlayerObject = GetComponent<SoundPlayerObject>();
        rigidbody = GetComponent<Rigidbody>();
        walkSpeed = 0.4f;

        kozoStateDic.Add(EnemyState.Init, new KozoStateInit(this));
        kozoStateDic.Add(EnemyState.ChasePlayer, new KozoStateChasePlayer(this));

        ChangeState(EnemyState.Init);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != EnemyState.Init)
        {
            kozoStateDic[currentState].UpdateAction();
        }
    }

    public void ChangeState(EnemyState nextState)
    {
        if (!gameObject.activeSelf) return;

        kozoStateDic[currentState].EndAction();
        currentState = nextState;
        kozoStateDic[currentState].StartAction();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (onColliderEnterEvent != null)
        {
            onColliderEnterEvent(other);
        }
    }
}
