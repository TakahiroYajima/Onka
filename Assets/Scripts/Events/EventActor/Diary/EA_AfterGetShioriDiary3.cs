using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_AfterGetShioriDiary3 : EventActorBase
{
    [SerializeField] private Enemy_Shiori shioriPrefWalk = null;
    private Enemy_Shiori instanceAnim = null;
    [SerializeField] private Vector3 walkInstancePosition = Vector3.zero;
    public Event_AfterGetShioriDiary1 eventBase = null;

    private string yukieSetPositionSDPKey = "sd_point_outer_0008";
    private string yukieSetTargetWanderingPointKey = "wandering_point_yukie_0003";
    //private string yukieNextWanderingPointKey = "wandering_point_yukie_0003";

    protected override void Initialize()
    {
        //parent.SetCanBeStarted(false);
    }
    public override void EventStart()
    {
        instanceAnim = Instantiate(shioriPrefWalk, this.transform);
        StageManager.Instance.Shiori = instanceAnim;
        instanceAnim.transform.position = walkInstancePosition;
        instanceAnim.onWalkEventEnded = () =>
        {
            //風呂場の鍵を落とすため、位置を設定
            Vector3 keyPos = new Vector3(instanceAnim.transform.position.x, instanceAnim.transform.position.y + 1f, instanceAnim.transform.position.z);
            eventBase.azuyuzuKeyActiveEvent.SetItemPosition(keyPos);

            parent.EventClearContact();
            Destroy(instanceAnim.gameObject);
            StageManager.Instance.Shiori = null;
        };
        //雪絵の位置を移動（キッチンから出たときに丁度遭遇する距離、またそのままだと部屋を覗き込んで2人同時または詩織の後に即追いかけられる展開に）
        StageManager.Instance.ForceOperationYukiePositionWithSDP(yukieSetPositionSDPKey, yukieSetTargetWanderingPointKey);
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
