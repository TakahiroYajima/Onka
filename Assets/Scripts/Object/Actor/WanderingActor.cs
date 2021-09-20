using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 徘徊ポイントを順番に徘徊する動作のクラス
/// NavMeshAgentで動くので、このクラスでNavMeshAgentを動作させている時は、他のクラスはNavMeshAgentを使用してはならない
/// このクラスのNavMeshAgentの動作を止めたい場合は、他クラスからNavMeshAgentを操作させる事
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class WanderingActor : MonoBehaviour
{
    [SerializeField] public WanderingEnemyType wanderingEnemyType { get; private set; } = WanderingEnemyType.Yukie;
    private float moveSpeed = 1f;

    private NavMeshAgent navMeshAgent = null;

    public int currentWanderingPointID { get; private set; } = 0;//現在の目的地の配列の要素番号
    public bool isActive { get; private set; } = false;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    //private void Update()
    //{
    //    if (isActive)
    //    {
    //        //目的地に着いたら次の通過点へ向かう
    //        if((transform.position - WanderingPointManager.Instance.wanderingPoints[wanderingEnemyType][currentWanderingPointID].transform.position).sqrMagnitude <= 0.2f)
    //        {
    //            DoNextWanderingPointSet();
    //        }
    //    }
    //}

    public void SetActive(bool _active)
    {
        //Debug.Log("徘徊モードアクティブ : " + _active);
        isActive = _active;
    }
    public void SetMoveSpeed(float _speed)
    {
        moveSpeed = _speed;
    }

    /// <summary>
    /// 次の徘徊通過点を設定
    /// </summary>
    public void DoNextWanderingPointSet(int nextID)
    {
        
        if(nextID >= WanderingPointManager.Instance.wanderingPoints[wanderingEnemyType].Count)
        {
            currentWanderingPointID = 0;
        }
        else
        {
            currentWanderingPointID = nextID;
        }
        Debug.Log("次の目的地 : " + (currentWanderingPointID));
        navMeshAgent.SetDestination(WanderingPointManager.Instance.wanderingPoints[wanderingEnemyType][currentWanderingPointID].transform.position);
        navMeshAgent.speed = moveSpeed;
    }
    /// <summary>
    /// 徘徊通過点を強制的に設定
    /// </summary>
    /// <param name="targetID"></param>
    public void SetWanderingID(int targetID)
    {
        if (targetID < 0) return;
        if (targetID >= WanderingPointManager.Instance.wanderingPoints[wanderingEnemyType].Count)
        {
            currentWanderingPointID = 0;
        }
        else
        {
            currentWanderingPointID = targetID;
        }
        navMeshAgent.SetDestination(WanderingPointManager.Instance.wanderingPoints[wanderingEnemyType][currentWanderingPointID].transform.position);
        navMeshAgent.speed = moveSpeed;
    }
}
