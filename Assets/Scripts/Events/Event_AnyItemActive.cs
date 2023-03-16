using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Onka.Manager.Data;

/// <summary>
/// 何らかの鍵アイテムをアクティブにさせるイベント
/// </summary>
public class Event_AnyItemActive : EventBase
{
    private ItemObject[] itemObjects = null;
    [SerializeField] private string[] itemObjectKeys = null;
    [SerializeField] private bool awaitAllItemGeted = true;
    public void SetItemPosition(Vector3 pos) { 
        if(itemObjects == null)
        {
            itemObjects = ItemManager.Instance.GetItemObjectsWithKeys(itemObjectKeys);
        }
        if (itemObjects.Length > 0)
        {
            itemObjects[0].transform.position = pos;
        }
        else
        {
            Debug.LogError("item count is zero");
        }
    }

    protected override void AlreadyClearedMove()
    {
        itemObjects = ItemManager.Instance.GetItemObjectsWithKeys(itemObjectKeys);
        //永久に存在するアイテム（日記など）は、表示させて終了
        for (int i = 0; i < itemObjectKeys.Length; ++i)
        {
            switch (DataManager.Instance.GetItemData(itemObjectKeys[i]).type)
            {
                case ItemType.WatchOnly:
                    itemObjects[i].gameObject.SetActive(true);
                    break;
            }
        }
    }

    protected override void EventActive()
    {
        itemObjects = ItemManager.Instance.GetItemObjectsWithKeys(itemObjectKeys);
        base.EventActive();
        InitiationContact();
    }
    public override void EventStart()
    {
        base.EventStart();
        foreach(var obj in itemObjects)
        {
            obj.gameObject.SetActive(true);
        }
    }

    public override void EventUpdate()
    {
        base.EventUpdate();
        //鍵を取得するまでイベントクリアにはならない
        //取得しなくてもクリア可能なら無視する
        bool isCleared = true;
        if (awaitAllItemGeted)
        {
            foreach (var key in itemObjectKeys)
            {
                var item = DataManager.Instance.GetItemData(key);
                if (!item.geted)
                {
                    isCleared = false;
                }
            }
        }

        if (isCleared)
        {
            EventClearContact();
        }
    }
}
