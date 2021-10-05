using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent navMeshAgent { get; protected set; } = null;
    public float walkSpeed { get; protected set; } = 1f;
    public float runSpeed { get; protected set; } = 4f;

    public EnemyState currentState { get; protected set; } = EnemyState.Init;

    protected virtual void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    public void MoveToTargetNavMeshAgent(Vector3 targetPosition)
    {
        navMeshAgent.SetDestination(targetPosition);
    }


}

public enum EnemyState
{
    Init,//初期値
    Wandering,//通常の徘徊
    InRoomWandering,//部屋内の徘徊専用
    Event,//イベント中
    RotateToPlayer,//プレイヤーの方へ振り向くだけの挙動（雪絵限定？）
    RecognizedPlayer,//プレイヤーを認識した際の挙動
    ChasePlayer,//プレイヤーを追いかける
    CaughtPlayer,//プレイヤーを捕まえた
}