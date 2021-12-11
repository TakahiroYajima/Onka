using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_AfterGetShioriDiary3 : EventActorBase
{
    [SerializeField] private Enemy_Shiori shioriPrefWalk = null;
    private Enemy_Shiori instanceAnim = null;
    [SerializeField] private Vector3 walkInstancePosition = Vector3.zero;

    protected override void Initialize()
    {
        //parent.SetCanBeStarted(false);
    }
    public override void EventStart()
    {
        Debug.Log("詩織の日記1取得後イベントスタート");
        instanceAnim = Instantiate(shioriPrefWalk, this.transform);
        StageManager.Instance.Shiori = instanceAnim;
        instanceAnim.transform.position = walkInstancePosition;
        instanceAnim.onWalkEventEnded = () =>
        {
            Debug.Log("詩織の日記1取得後イベントクリア");
            parent.EventClearContact();
            Destroy(instanceAnim.gameObject);
            StageManager.Instance.Shiori = null;
        };
        StartCoroutine(StartEvent());
    }
    public override void EventUpdate()
    {

    }
    public override void EventEnd()
    {

    }
    private IEnumerator StartEvent()
    {
        yield return null;//初期化待ち
        instanceAnim.ChangeState(Enemy_ShioriState.WalkEvent);
    }
}
