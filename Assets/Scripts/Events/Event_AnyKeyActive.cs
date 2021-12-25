using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 何らかの鍵アイテムをアクティブにさせるイベント
/// </summary>
public class Event_AnyKeyActive : EventBase
{
    [SerializeField] private ItemObject keyObject = null;

    protected override void EventActive()
    {
        base.EventActive();
        InitiationContact();
    }
    public override void EventStart()
    {
        base.EventStart();
        keyObject.gameObject.SetActive(true);
    }

    public override void EventUpdate()
    {
        base.EventUpdate();
        //鍵を取得するまでイベントクリアにはならない
        if (DataManager.Instance.GetItemData(keyObject.ItemKey).geted)
        {
            EventClearContact();
        }
    }
}
