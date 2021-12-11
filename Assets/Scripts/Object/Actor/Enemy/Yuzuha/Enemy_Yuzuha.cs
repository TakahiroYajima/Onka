using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CapsuleCollider))]
public class Enemy_Yuzuha : Enemy
{
    public WalkAnimObj walkAnimObj { get; private set; } = null;

    public Dictionary<EnemyState, StateBase> yuzuhaStateDic { get; private set; } = new Dictionary<EnemyState, StateBase>();

    public UnityAction<EnemyState> onStateChangeCallback = null;
    public UnityAction<Collision> onCollsionEnterCallback = null;

    protected override void Awake()
    {
        base.Awake();
        walkAnimObj = GetComponent<WalkAnimObj>();

        yuzuhaStateDic.Add(EnemyState.CanNotAction, new YuzuhaStateCanNotAction());
        yuzuhaStateDic.Add(EnemyState.ChasePlayer, new YuzuhaStateChasePlayer());
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
        currentState = EnemyState.CanNotAction;//デバッグ
        yuzuhaStateDic[currentState].StartAction();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != EnemyState.Init)
        {
            yuzuhaStateDic[currentState].UpdateAction();
        }
    }

    public void ChangeState(EnemyState nextState)
    {
        yuzuhaStateDic[currentState].EndAction();
        currentState = nextState;
        yuzuhaStateDic[currentState].StartAction();
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
