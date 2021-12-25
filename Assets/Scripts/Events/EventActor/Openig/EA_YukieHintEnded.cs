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

    protected override void Initialize()
    {
        kitchenCollider = kithcenCollisionEnterEvent.gameObject.GetComponent<BoxCollider>();
        kitchenCollider.enabled = false;
    }

    public override void EventStart()
    {
        kitchenCollider.enabled = true;
        SoundManager.Instance.PlayEnvironmentWithKey("ambient_in_house");

        //キッチンの方から雪絵の声が聞こえるようにする
        StageManager.Instance.Yukie.transform.position = new Vector3(eventBase.kitchenSoundPoint.transform.position.x, StageManager.Instance.Yukie.transform.position.y, eventBase.kitchenSoundPoint.transform.position.z + 2f);
        SoundDistanceManager.Instance.Emitter.SetPointID(eventBase.kitchenSoundPoint.ID);
        StageManager.Instance.Yukie.PlaySoundLoop(0);
        StageManager.Instance.Yukie.gameObject.SetActive(false);

        SoundDistanceManager.Instance.Listener.SetCurrentPointID(eventBase.drawingRoomSoundPoint.ID);
        SoundDistanceManager.Instance.Maker.DoAction();
        SoundDistanceManager.Instance.isActive = true;

        //孝蔵と初の寝室の鍵をアクティブにする
        //eventBase.kozoHatsuRoomKeyObject.gameObject.SetActive(true);
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
