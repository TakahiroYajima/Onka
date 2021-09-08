using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵がプレイヤーを発見していないときに徘徊する中間地点のスクリプト
/// </summary>
public class WanderingPoint : MonoBehaviour
{
    [SerializeField] private WanderingEnemyType wanderingEnemyType = WanderingEnemyType.Yukie;
    public WanderingEnemyType WanderingEnemyType { get { return wanderingEnemyType; } }

    //何番目の通過地点か
    [SerializeField] private int pointNum = 0;
    public int PointNum { get { return pointNum; } }

}

