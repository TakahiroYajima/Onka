using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CapsuleCollider))]
public class Enemy_Azuha : Enemy
{
    public WalkAnimObj walkAnimObj { get; private set; } = null;
    public SoundPlayerObject soundPlayerObject { get; private set; } = null;

    public Dictionary<EnemyState, StateBase> azuhaStateDic { get; private set; } = new Dictionary<EnemyState, StateBase>();

    public UnityAction<EnemyState> onStateChangeCallback = null;
    public UnityAction<Collision> onCollsionEnterCallback = null;

    protected override void Awake()
    {
        base.Awake();
        walkAnimObj = GetComponent<WalkAnimObj>();
        soundPlayerObject = GetComponent<SoundPlayerObject>();

        azuhaStateDic.Add(EnemyState.CanNotAction, new AzuhaStateCanNotAction());
        azuhaStateDic.Add(EnemyState.ChasePlayer, new AzuhaStateChasePlayer());
        currentState = EnemyState.CanNotAction;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //StartCoroutine(StartAction());
    }

    private IEnumerator StartAction()
    {
        yield return null;//他のスクリプトの初期化待ち
        currentState = EnemyState.CanNotAction;
        azuhaStateDic[currentState].StartAction();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != EnemyState.Init)
        {
            azuhaStateDic[currentState].UpdateAction();
        }
    }

    public void ChangeState(EnemyState nextState)
    {
        azuhaStateDic[currentState].EndAction();
        currentState = nextState;
        azuhaStateDic[currentState].StartAction();
        if (onStateChangeCallback != null)
        {
            onStateChangeCallback(currentState);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (onColliderEnterCallback != null)
    //    {
    //        onColliderEnterCallback(other);
    //    }
    //}
    private void OnCollisionEnter(Collision collision)
    {
        if (onCollsionEnterCallback != null)
        {
            onCollsionEnterCallback(collision);
        }
    }
}
