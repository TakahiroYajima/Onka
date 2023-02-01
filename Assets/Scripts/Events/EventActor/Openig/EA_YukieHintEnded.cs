using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;
using SoundDistance;

public class EA_YukieHintEnded : EventActorBase
{
    public Event_YukieHintEnded eventBase { private get; set; } = null;
    private BoxCollider kitchenCollider = null;
    [SerializeField] private CollisionEnterEvent kithcenCollisionEnterEvent = null;
    [SerializeField, ReadOnly] private string kitchenSoundPointKey = "sd_point_outer_0009";
    [SerializeField, ReadOnly] private string drawingRoomSoundPointKey = "sd_point_outer_0001";
    [SerializeField, ReadOnly] private string savePointSoundPointKey = "sd_point_outer_0003";

    protected override void Initialize()
    {
        kitchenCollider = kithcenCollisionEnterEvent.gameObject.GetComponent<BoxCollider>();
        kitchenCollider.enabled = false;
    }
    
    public override void EventStart()
    {
        var kitchenSoundPoint = SoundDistanceManager.Instance.GetSoundDistancePoint(kitchenSoundPointKey);
        var drawingRoomSoundPoint = SoundDistanceManager.Instance.GetSoundDistancePoint(drawingRoomSoundPointKey);
        var savePointSoundPoint = SoundDistanceManager.Instance.GetSoundDistancePoint(savePointSoundPointKey);

        kitchenCollider.enabled = true;
        SoundManager.Instance.PlayEnvironmentWithKey("ambient_in_house");
        //キッチンの方から雪絵の声が聞こえるようにする
        StageManager.Instance.Yukie.transform.position = new Vector3(kitchenSoundPoint.transform.position.x, StageManager.Instance.Yukie.transform.position.y, kitchenSoundPoint.transform.position.z + 2f);
        SoundDistanceManager.Instance.Emitter.SetPointID(kitchenSoundPoint.ID);
        //セーブポイントに設定されていたら途中から再開したことになるのでそのままにする
        if (SoundDistanceManager.Instance.Listener.currentPointID != savePointSoundPoint.ID)
        {
            SoundDistanceManager.Instance.Listener.SetCurrentPointID(drawingRoomSoundPoint.ID);
            SoundDistanceManager.Instance.Listener.SetNextTargetPointID(drawingRoomSoundPoint.ID);
        }
        SoundDistanceManager.Instance.ForceInitCalc();
        SoundDistanceManager.Instance.Maker.SetVolume(0f);
        SoundDistanceManager.Instance.isActive = true;
        StageManager.Instance.Yukie.PlaySoundLoop(0, 0.3f);
        StageManager.Instance.Yukie.gameObject.SetActive(false);
        SoundDistanceManager.Instance.Maker.DoAction();
    }
    
    public override void EventUpdate()
    {
        base.EventUpdate();
        //鍵を取得するまでイベントクリアにはならない
        //if (DataManager.Instance.GetItemData(eventBase.kozoHatsuRoomKeyObject.ItemKey).geted)
        //{
        //    FinishEvent();
        //}
    }

    public override void EventEnd()
    {

    }
    public void OnKitchenColliderEnterEvent()
    {
        if (Utility.Instance.IsTagNameMatch(kithcenCollisionEnterEvent.HitCollision.gameObject, Tags.Player))
        {
            kitchenCollider.enabled = false;
            SoundDistanceManager.Instance.Maker.StopAction();
            SoundDistanceManager.Instance.Maker.SetVolume(0f);
            SoundManager.Instance.StopEnvironment();
            FinishEvent();
        }
    }
}
