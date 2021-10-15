using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 詩織の日記1を取得した後のイベント
/// </summary>
public class Event_AfterGetShioriDiary1 : EventBase
{
    [SerializeField] private Enemy_Shiori shioriPrefWalk = null;
    [SerializeField] private Enemy_Shiori shioriPrefIdle = null;
    private Enemy_Shiori instanceAnim = null;
    [SerializeField] private Vector3 walkInstancePosition = Vector3.zero;
    [SerializeField] private Vector3 idleInstancePosition = Vector3.zero;

    public override void ProgressEvent()
    {
        if (!EventManager.Instance.IsEventEnded(eventKey))
        {
            if (DataManager.Instance.GetItemData("diary_shiori_3").geted)
            {
                Debug.Log("詩織の日記1取得後イベント解放");
                EventManager.Instance.EventStart(managementID);
            }
        }
    }

    public override void EventStart()
    {
        Debug.Log("詩織の日記1取得後イベント");
        instanceAnim = Instantiate(shioriPrefWalk, this.transform);
        instanceAnim.transform.position = walkInstancePosition;
        instanceAnim.onWalkEventEnded = () =>
        {
            Debug.Log("詩織の日記1取得後イベントクリア");
            EventManager.Instance.EventClear(EventKey);
            Destroy(instanceAnim.gameObject);
        };
        StartCoroutine(StartEvent());
        //soundPlayer.PlaySE("se_lug");
        //StartCoroutine(MoveStatus(() =>
        //{
        //    EventManager.Instance.EventClear(EventKey);
        //}));
    }
    private IEnumerator StartEvent()
    {
        yield return null;//初期化待ち
        instanceAnim.ChangeState(Enemy_ShioriState.WalkEvent);
    }
    public override void EventUpdate()
    {
        
    }
    public override void EventEnd()
    {
        Debug.Log("詩織の日記1取得後イベント終了");
    }
}
