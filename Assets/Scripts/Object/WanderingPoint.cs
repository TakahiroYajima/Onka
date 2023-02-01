using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵がプレイヤーを発見していないときに徘徊する中間地点のスクリプト
/// </summary>
public class WanderingPoint : MonoBehaviour
{
    [field: SerializeField] public string objectKey { get; private set; } = "";
    [SerializeField] private WanderingEnemyType wanderingEnemyType = WanderingEnemyType.Yukie;
    public WanderingEnemyType WanderingEnemyType { get { return wanderingEnemyType; } }

    //何番目の通過地点か
    [SerializeField] private int pointNum = 0;
    public int PointNum { get { return pointNum; } }
    public bool isOuter { get; private set; } = true;

    [SerializeField] private WanderingCollider wanderingCollider = null;

    [SerializeField] private OnWPArrivaledEventCode eventCode = OnWPArrivaledEventCode.None;
    public OnWPArrivaledEventCode EventCode { get { return eventCode; } }

    public void Initialize(bool _isOuter = true)
    {
        if(wanderingCollider == null) { Debug.LogError("WanderingColliderが設定されていません。"); return; }
        isOuter = _isOuter;
        wanderingCollider.onArrivaledEvent = OnArrival;
    }

    private void OnArrival()
    {
        if (isOuter)
        {
            StageManager.Instance.Yukie.wanderingActor.SetWanderingID(pointNum + 1);
        }
        else
        {
            StageManager.Instance.Yukie.inRoomWanderingActor.OnArrivaled(pointNum + 1, eventCode);
        }
    }
}
/// <summary>
/// 徘徊の通過ポイントに到達した際の挙動を決める列挙
/// </summary>
public enum OnWPArrivaledEventCode
{
    None,//イベント無し
    Survey,//その場で回りを見回す
    Survey_RightOnly,//その場で右を見回す
}