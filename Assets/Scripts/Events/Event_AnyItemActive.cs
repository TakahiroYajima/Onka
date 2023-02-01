using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Onka.Manager.Data;

/// <summary>
/// 何らかの鍵アイテムをアクティブにさせるイベント
/// </summary>
public class Event_AnyItemActive : EventBase
{
    private ItemObject itemObject = null;
    [SerializeField] private string itemObjectKey = "";
    public void SetItemPosition(Vector3 pos) { 
        if(itemObject == null)
        {
            itemObject = ItemManager.Instance.GetItemObjectWithKey(itemObjectKey);
        }
        itemObject.transform.position = pos; 
    }

    protected override void AlreadyClearedMove()
    {
        itemObject = ItemManager.Instance.GetItemObjectWithKey(itemObjectKey);
        //永久に存在するアイテム（日記など）は、表示させて終了
        switch (DataManager.Instance.GetItemData(itemObjectKey).type)
        {
            case ItemType.WatchOnly:
                itemObject.gameObject.SetActive(true);
                break;
        }
    }

    protected override void EventActive()
    {
        itemObject = ItemManager.Instance.GetItemObjectWithKey(itemObjectKey);
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
        if (DataManager.Instance.GetItemData(itemObjectKey).geted)
        {
            EventClearContact();
        }
    }
}
