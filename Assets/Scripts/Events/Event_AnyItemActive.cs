using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Onka.Manager.Data;

/// <summary>
/// 何らかの鍵アイテムをアクティブにさせるイベント
/// </summary>
public class Event_AnyItemActive : EventBase
{
    [SerializeField] private ItemObject itemObject = null;
    public void SetItemPosition(Vector3 pos) { itemObject.transform.position = pos; }

    protected override void AlreadyClearedMove()
    {
        //永久に存在するアイテム（日記など）は、表示させて終了
        switch (DataManager.Instance.GetItemData(itemObject.ItemKey).type)
        {
            case ItemType.WatchOnly:
                itemObject.gameObject.SetActive(true);
                break;
        }
    }

    protected override void EventActive()
    {
        base.EventActive();
        InitiationContact();
    }
    public override void EventStart()
    {
        base.EventStart();
        itemObject.gameObject.SetActive(true);
    }

    public override void EventUpdate()
    {
        base.EventUpdate();
        //鍵を取得するまでイベントクリアにはならない
        if (DataManager.Instance.GetItemData(itemObject.ItemKey).geted)
        {
            EventClearContact();
        }
    }
}
