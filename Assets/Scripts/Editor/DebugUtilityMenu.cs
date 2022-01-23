using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Onka.Manager.Event;

public class DebugUtilityMenu : MonoBehaviour
{
    [MenuItem("Debug/GameData/データ初期化")]
    public static void InitGameData()
    {
        GameData data = FileManager.LoadSaveData<GameData>(SaveType.Normal, DataManager.GameDataFileName);
        if(data == null || data == default)
        {
            Debug.Log("データがないので初期化の必要はありません。");
        }
        else
        {
            data.AllInitialize();
            FileManager.DataSave<GameData>(data, SaveType.Normal, DataManager.GameDataFileName);
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Debug/GameData/オープニングまでクリアした状態でデータ初期化")]
    public static void InitGameData_AfterOpening()
    {
        UpdateItemData();
        UpdateEventListData();
        InitGameData();
        GameData data = FileManager.LoadSaveData<GameData>(SaveType.Normal, DataManager.GameDataFileName);
        if (data == null || data == default)
        {
            data = new GameData();
            data.itemDataList = new ItemDataList();
            data.itemDataList.itemDataList = new List<ItemData>();
        }

        //以下、UpdateItemDataとUpdateEventListData、InitGameDataを先に呼んでおけばnullチェックいらんよねという理論なので注意
        ItemDataList itemDataList = FileManager.LoadSaveData<ItemDataList>(SaveType.Normal, DataManager.ItemDataFileName);
        EventDataList eventDataList = FileManager.LoadSaveData<EventDataList>(SaveType.Normal, DataManager.EventDataFileName);
        //アイテムを手に入れた事にする
        DoGetedAndUsedItem(itemDataList, "key_drawingroom");
        DoGetedAndUsedItem(itemDataList, "diary_kozo_1");
        DoGetedAndUsedItem(itemDataList, "key_kozoroom");

        //イベントを終えたことにする
        DoEndedEvent(eventDataList, "Event_Opnening");
        DoEndedEvent(eventDataList, "Event_YukieHint");
        DoEndedEvent(eventDataList, "Event_YukieHintEnded");
        DoEndedEvent(eventDataList, "Event_KozoRoomKeyActive");
        DoEndedEvent(eventDataList, "Event_FirstChasedFromYukie");

        //アイテムとイベントをセーブ用データに適用
        data.itemDataList = itemDataList;
        data.eventDataList = eventDataList;

        FileManager.DataSave<GameData>(data, SaveType.Normal, DataManager.GameDataFileName);
        AssetDatabase.Refresh();
    }

    private static void DoGetedAndUsedItem(ItemDataList itemDataList, string key)
    {
        ItemData itemData = itemDataList.itemDataList.FirstOrDefault(x => x.key == key);
        if (itemData == null) { Debug.LogError("アイテムがありません : " + key); return; }
        itemData.geted = true;
        if(itemData.type == ItemType.Useable || itemData.type == ItemType.DoorKey)
        {
            itemData.used = true;
        }
    }
    private static void DoEndedEvent(EventDataList eventDataList, string key)
    {
        EventData eData = eventDataList.list.FirstOrDefault(x => x.eventKey == key);
        if(eData == null) { Debug.LogError("イベントがありません : " + key); return; }
        eData.isAppeared = true;
        eData.isEnded = true;
    }

    [MenuItem("Debug/GameData/アイテム更新")]
    public static void UpdateItemData()
    {
        GameData data = FileManager.LoadSaveData<GameData>(SaveType.Normal, DataManager.GameDataFileName);
        if (data == null || data == default)
        {
            data = new GameData();
            data.itemDataList = new ItemDataList();
            data.itemDataList.itemDataList = new List<ItemData>();
        }

        ItemDataList itemDataList = FileManager.LoadSaveData<ItemDataList>(SaveType.Normal, DataManager.ItemDataFileName);
        if (itemDataList == null || itemDataList == default)
        {
            Debug.LogError("アイテムデータがありません");
            return;
        }
        
        for(int i = 0; i < itemDataList.itemDataList.Count; i++)
        {
            ItemData iData = data.itemDataList.itemDataList.FirstOrDefault(x => x.key == itemDataList.itemDataList[i].key);
            if(iData == null || iData == default)
            {
                //GameDataに無ければ新規追加なので登録する
                data.itemDataList.itemDataList.Add(itemDataList.itemDataList[i]);
                Debug.Log("UpdateItemData_NewAdd : " + itemDataList.itemDataList[i].name);
            }
            else
            {
                Debug.Log("UpdateItemData : " + iData.name);
                iData.name = itemDataList.itemDataList[i].name;
                iData.spriteName = itemDataList.itemDataList[i].spriteName;
                iData.type = itemDataList.itemDataList[i].type;
                iData.description = itemDataList.itemDataList[i].description;
                iData.fileItem = itemDataList.itemDataList[i].fileItem;
                //Debug.Log("UpdateItemData_NerAdd : " + iData.name);
            }
        }

        //ソートさせたい

        FileManager.DataSave<GameData>(data, SaveType.Normal, DataManager.GameDataFileName);
    }
    [MenuItem("Debug/GameData/イベント更新")]
    public static void UpdateEventListData()
    {
        GameData data = FileManager.LoadSaveData<GameData>(SaveType.Normal, DataManager.GameDataFileName);
        if (data == null || data == default)
        {
            data = new GameData();
            data.itemDataList = new ItemDataList();
            data.itemDataList.itemDataList = new List<ItemData>();
        }

        EventDataList eventDataList = FileManager.LoadSaveData<EventDataList>(SaveType.Normal, DataManager.EventDataFileName);
        if (eventDataList == null || eventDataList == default)
        {
            Debug.LogError("イベントデータがありません");
            return;
        }

        for (int i = 0; i < eventDataList.list.Count; i++)
        {
            EventData eData = data.eventDataList.list.FirstOrDefault(x => x.eventKey == eventDataList.list[i].eventKey);
            if (eData == null || eData == default)
            {
                data.eventDataList.list.Add(eventDataList.list[i]);
            }
            else
            {
                eData.isAppeared = eventDataList.list[i].isAppeared;
                eData.isEnded = eventDataList.list[i].isEnded;
            }
        }
        FileManager.DataSave<GameData>(data, SaveType.Normal, DataManager.GameDataFileName);
    }
}
