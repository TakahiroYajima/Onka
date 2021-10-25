using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// 部屋内専用：徘徊ポイントを順番に徘徊する動作のクラス
/// NavMeshAgentで動くので、このクラスでNavMeshAgentを動作させている時は、他のクラスはNavMeshAgentを使用してはならない
/// このクラスのNavMeshAgentの動作を止めたい場合はSetActive(false,null)を呼ぶこと。
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class InRoomWanderingActor : MonoBehaviour
{
    public RoomWanderingManager currentManager = null;
    private float moveSpeed = 1f;

    private NavMeshAgent navMeshAgent = null;

    public int currentWanderingPointID { get; private set; } = 0;//現在の目的地の配列の要素番号
    public bool isActive { get; private set; } = false;

    public UnityAction<int, OnWPArrivaledEventCode> onArrivaledPointCallback = null;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void Initialize()
    {
        currentWanderingPointID = 0;
    }

    public void SetActive(bool _active, RoomWanderingManager _manager, bool isNavMeshSetting = false)
    {
        //Debug.Log("SetActive : " + _active.ToString() + " : " + _manager);
        currentManager = _manager;
        isActive = _active;
        if (isNavMeshSetting)
        {
            navMeshAgent.enabled = isActive;
        }
    }
    public void SetMoveSpeed(float _speed)
    {
        moveSpeed = _speed;
        navMeshAgent.speed = moveSpeed;
    }

    /// <summary>
    /// 徘徊通過点到着・次の徘徊通過点を設定
    /// </summary>
    /// <param name="nextTargetID"></param>
    public void OnArrivaled(int nextTargetID, OnWPArrivaledEventCode _eventCode)
    {
        if (!isActive) return;
        if (nextTargetID < 0 || nextTargetID == currentWanderingPointID) return;
        if (nextTargetID >= currentManager.wanderingPoints.Count)
        {
            currentWanderingPointID = 0;
        }
        else
        {
            currentWanderingPointID = nextTargetID;
        }
        
        if (onArrivaledPointCallback != null)
        {
            onArrivaledPointCallback(currentWanderingPointID, _eventCode);
        }
    }

    public void MoveNext()
    {
        if(currentManager == null) { Debug.Log("CurrentManagerがnullです"); return; }
        navMeshAgent.enabled = true;
        navMeshAgent.SetDestination(currentManager.wanderingPoints[currentWanderingPointID].transform.position);
    }
}
