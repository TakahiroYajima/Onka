using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using Onka.Manager.Event;
using Onka.Manager.Data;

public class DebugUtilityMenu : MonoBehaviour
{
    [MenuItem("Debug/GameData/全セーブデータ初期化or生成")]
    public static void InitAllSaveData()
    {
        StringBuilder fileName = new StringBuilder();
        for (int i = 0; i < DataManager.MaxSaveDataCount; i++)
        {
            fileName.Append(DataManager.GameDataBaseFileName);
            fileName.Append(i.ToString());
            fileName.Append(".txt");
            GameData data = LoadOrNewInstanceGameData(SaveType.PlayerData, fileName.ToString());
            data = CheckAndCopyMasterDataToGameData(ref data);
            FileManager.DataSave<GameData>(data, SaveType.PlayerData, fileName.ToString());
            fileName.Clear();
        }
    }

    [MenuItem("Debug/GameData/バックグラウンドセーブデータ初期化or生成")]
    public static void InitGeneralPlayerData()
    {
        GameData data = LoadOrNewInstanceGameData(SaveType.GeneralPlayerData, DataManager.GameDataFileName);
        data = CheckAndCopyMasterDataToGameData(ref data);
        FileManager.DataSave<GameData>(data, SaveType.GeneralPlayerData, DataManager.GameDataFileName);
    }

    [MenuItem("Debug/GameData/オープニングまでクリアした状態でデータ初期化（1番目のデータのみ）")]
    public static void InitGameData_AfterOpening()
    {
        StringBuilder fileName = new StringBuilder();
        fileName.Append(DataManager.GameDataBaseFileName);
        fileName.Append("0.txt");
        GameData data = LoadOrNewInstanceGameData(SaveType.PlayerData, fileName.ToString());
        
        //マスターデータがある事前提なので注意
        ItemDataList itemDataList = FileManager.LoadSaveData<ItemDataList>(SaveType.MasterData, DataManager.ItemDataFileName);
        EventDataList eventDataList = FileManager.LoadSaveData<EventDataList>(SaveType.MasterData, DataManager.EventDataFileName);
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

        //日付を入れないとNewGameのデータと判断される
        System.DateTime dateTime = System.DateTime.Now;
        string parceDate = dateTime.ToString(DataManager.SaveDateTimeFormat);
        data.saveDate = parceDate;

        FileManager.DataSave<GameData>(data, SaveType.PlayerData, fileName.ToString());
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// マスターデータをセーブ用データにセット（要素追加後の初期化・フラグリセット用）
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static GameData CheckAndCopyMasterDataToGameData(ref GameData data)
    {
        if (!FileManager.Exists(SaveType.MasterData, DataManager.ItemDataFileName) ||
                !FileManager.Exists(SaveType.MasterData, DataManager.EventDataFileName))
        {
            Debug.LogError("アイテムかイベントのマスターデータがありません");
            return data;
        }
        ItemDataList itemDataList = FileManager.LoadSaveData<ItemDataList>(SaveType.MasterData, DataManager.ItemDataFileName);
        EventDataList eventDataList = FileManager.LoadSaveData<EventDataList>(SaveType.MasterData, DataManager.EventDataFileName);
        data.itemDataList = itemDataList;
        data.eventDataList = eventDataList;
        data.AllInitialize();
        return data;
    }

    /// <summary>
    /// 更新したアイテムを全てのデータに適用
    /// 入手・使用済みフラグは初期化されない
    /// </summary>
    public static void SetUpdatedItemDataToAllSaveDataAndBackgroundData()
    {
        ItemDataList updateItemList = FileManager.LoadSaveData<ItemDataList>(SaveType.MasterData, DataManager.ItemDataFileName);
        StringBuilder fileName = new StringBuilder();
        for (int i = 0; i < DataManager.MaxSaveDataCount; i++)
        {
            fileName.Append(DataManager.GameDataBaseFileName);
            fileName.Append(i.ToString());
            fileName.Append(".txt");
            GameData data = LoadOrNewInstanceGameData(SaveType.PlayerData, fileName.ToString());
            
            data = SetUpdateItemData(ref data, updateItemList);
            FileManager.DataSave<GameData>(data, SaveType.PlayerData, fileName.ToString());
            fileName.Clear();
        }

        GameData generalPlayerData = LoadOrNewInstanceGameData(SaveType.GeneralPlayerData, DataManager.GameDataFileName);
        generalPlayerData = SetUpdateItemData(ref generalPlayerData, updateItemList);
        FileManager.DataSave<GameData>(generalPlayerData, SaveType.GeneralPlayerData, DataManager.GameDataFileName);
    }
    private static GameData SetUpdateItemData(ref GameData data, ItemDataList setItemDataList)
    {
        ItemDataList prevItemDataList = new ItemDataList();
        prevItemDataList.itemDataList = new List<ItemData>(data.itemDataList.itemDataList);

        data.itemDataList = setItemDataList;
        for (int i = 0; i < prevItemDataList.itemDataList.Count; i++)
        {
            ItemData item = data.itemDataList.itemDataList.FirstOrDefault(x => x.key == prevItemDataList.itemDataList[i].key);
            if (item == null || item == default) continue;

            item.geted = prevItemDataList.itemDataList[i].geted;
            item.used = prevItemDataList.itemDataList[i].used;
        }
        return data;
    }

    /// <summary>
    /// 更新したイベントを全てのデータに適用
    /// 入手・使用済みフラグは初期化されない
    /// </summary>
    public static void SetUpdatedEventDataToAllSaveDataAndBackgroundData()
    {
        EventDataList updateEventList = FileManager.LoadSaveData<EventDataList>(SaveType.MasterData, DataManager.EventDataFileName);
        StringBuilder fileName = new StringBuilder();
        for (int i = 0; i < DataManager.MaxSaveDataCount; i++)
        {
            fileName.Append(DataManager.GameDataBaseFileName);
            fileName.Append(i.ToString());
            fileName.Append(".txt");
            GameData data = LoadOrNewInstanceGameData(SaveType.PlayerData, fileName.ToString());

            data = SetUpdateEventData(ref data, updateEventList);
            FileManager.DataSave<GameData>(data, SaveType.PlayerData, fileName.ToString());
            fileName.Clear();
        }

        GameData generalPlayerData = LoadOrNewInstanceGameData(SaveType.GeneralPlayerData, DataManager.GameDataFileName);
        generalPlayerData = SetUpdateEventData(ref generalPlayerData, updateEventList);
        FileManager.DataSave<GameData>(generalPlayerData, SaveType.GeneralPlayerData, DataManager.GameDataFileName);
    }
    private static GameData SetUpdateEventData(ref GameData data, EventDataList setEventDataList)
    {
        EventDataList prevEventDataList = new EventDataList();
        prevEventDataList.list = new List<EventData>(data.eventDataList.list);

        data.eventDataList = setEventDataList;
        for (int i = 0; i < prevEventDataList.list.Count; i++)
        {
            EventData item = data.eventDataList.list.FirstOrDefault(x => x.eventKey == prevEventDataList.list[i].eventKey);
            if (item == null || item == default) continue;

            item.isAppeared = prevEventDataList.list[i].isAppeared;
            item.isEnded = prevEventDataList.list[i].isEnded;
        }
        return data;
    }

    private static GameData LoadOrNewInstanceGameData(SaveType type, string fileName)
    {
        if (FileManager.Exists(type, fileName))
        {
            return FileManager.LoadSaveData<GameData>(type, fileName);
        }
        else
        {
            GameData data = new GameData();
            data.itemDataList = new ItemDataList();
            data.eventDataList = new EventDataList();
            return data;
        }
    }

    //[MenuItem("Debug/GameData/データ初期化")]
    //public static void InitGameData()
    //{
    //    GameData data = FileManager.LoadSaveData<GameData>(SaveType.Normal, DataManager.GameDataFileName);
    //    if(data == null || data == default)
    //    {
    //        Debug.Log("データがないので初期化の必要はありません。");
    //    }
    //    else
    //    {
    //        data.AllInitialize();
    //        FileManager.DataSave<GameData>(data, SaveType.Normal, DataManager.GameDataFileName);
    //        AssetDatabase.Refresh();
    //    }
    //}

    //[MenuItem("Debug/GameData/オープニングまでクリアした状態でデータ初期化")]
    //public static void InitGameData_AfterOpening()
    //{
    //    UpdateItemData();
    //    UpdateEventListData();
    //    InitGameData();
    //    GameData data = FileManager.LoadSaveData<GameData>(SaveType.Normal, DataManager.GameDataFileName);
    //    if (data == null || data == default)
    //    {
    //        data = new GameData();
    //        data.itemDataList = new ItemDataList();
    //        data.itemDataList.itemDataList = new List<ItemData>();
    //    }

    //    //以下、UpdateItemDataとUpdateEventListData、InitGameDataを先に呼んでおけばnullチェックいらんよねという理論なので注意
    //    ItemDataList itemDataList = FileManager.LoadSaveData<ItemDataList>(SaveType.Normal, DataManager.ItemDataFileName);
    //    EventDataList eventDataList = FileManager.LoadSaveData<EventDataList>(SaveType.Normal, DataManager.EventDataFileName);
    //    //アイテムを手に入れた事にする
    //    DoGetedAndUsedItem(itemDataList, "key_drawingroom");
    //    DoGetedAndUsedItem(itemDataList, "diary_kozo_1");
    //    DoGetedAndUsedItem(itemDataList, "key_kozoroom");

    //    //イベントを終えたことにする
    //    DoEndedEvent(eventDataList, "Event_Opnening");
    //    DoEndedEvent(eventDataList, "Event_YukieHint");
    //    DoEndedEvent(eventDataList, "Event_YukieHintEnded");
    //    DoEndedEvent(eventDataList, "Event_KozoRoomKeyActive");
    //    DoEndedEvent(eventDataList, "Event_FirstChasedFromYukie");

    //    //アイテムとイベントをセーブ用データに適用
    //    data.itemDataList = itemDataList;
    //    data.eventDataList = eventDataList;

    //    FileManager.DataSave<GameData>(data, SaveType.Normal, DataManager.GameDataFileName);
    //    AssetDatabase.Refresh();
    //}

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

    //[MenuItem("Debug/GameData/アイテム更新")]
    //public static void UpdateItemData()
    //{
    //    GameData data = FileManager.LoadSaveData<GameData>(SaveType.Normal, DataManager.GameDataFileName);
    //    if (data == null || data == default)
    //    {
    //        data = new GameData();
    //        data.itemDataList = new ItemDataList();
    //        data.itemDataList.itemDataList = new List<ItemData>();
    //    }

    //    ItemDataList itemDataList = FileManager.LoadSaveData<ItemDataList>(SaveType.Normal, DataManager.ItemDataFileName);
    //    if (itemDataList == null || itemDataList == default)
    //    {
    //        Debug.LogError("アイテムデータがありません");
    //        return;
    //    }
        
    //    for(int i = 0; i < itemDataList.itemDataList.Count; i++)
    //    {
    //        ItemData iData = data.itemDataList.itemDataList.FirstOrDefault(x => x.key == itemDataList.itemDataList[i].key);
    //        if(iData == null || iData == default)
    //        {
    //            //GameDataに無ければ新規追加なので登録する
    //            data.itemDataList.itemDataList.Add(itemDataList.itemDataList[i]);
    //            Debug.Log("UpdateItemData_NewAdd : " + itemDataList.itemDataList[i].name);
    //        }
    //        else
    //        {
    //            Debug.Log("UpdateItemData : " + iData.name);
    //            iData.name = itemDataList.itemDataList[i].name;
    //            iData.spriteName = itemDataList.itemDataList[i].spriteName;
    //            iData.type = itemDataList.itemDataList[i].type;
    //            iData.description = itemDataList.itemDataList[i].description;
    //            iData.fileItem = itemDataList.itemDataList[i].fileItem;
    //            //Debug.Log("UpdateItemData_NerAdd : " + iData.name);
    //        }
    //    }

    //    //ソートさせたい

    //    FileManager.DataSave<GameData>(data, SaveType.Normal, DataManager.GameDataFileName);
    //}
    //[MenuItem("Debug/GameData/イベント更新")]
    //public static void UpdateEventListData()
    //{
    //    GameData data = FileManager.LoadSaveData<GameData>(SaveType.Normal, DataManager.GameDataFileName);
    //    if (data == null || data == default)
    //    {
    //        data = new GameData();
    //        data.itemDataList = new ItemDataList();
    //        data.itemDataList.itemDataList = new List<ItemData>();
    //    }

    //    EventDataList eventDataList = FileManager.LoadSaveData<EventDataList>(SaveType.Normal, DataManager.EventDataFileName);
    //    if (eventDataList == null || eventDataList == default)
    //    {
    //        Debug.LogError("イベントデータがありません");
    //        return;
    //    }

    //    for (int i = 0; i < eventDataList.list.Count; i++)
    //    {
    //        EventData eData = data.eventDataList.list.FirstOrDefault(x => x.eventKey == eventDataList.list[i].eventKey);
    //        if (eData == null || eData == default)
    //        {
    //            data.eventDataList.list.Add(eventDataList.list[i]);
    //        }
    //        else
    //        {
    //            eData.isAppeared = eventDataList.list[i].isAppeared;
    //            eData.isEnded = eventDataList.list[i].isEnded;
    //        }
    //    }
    //    FileManager.DataSave<GameData>(data, SaveType.Normal, DataManager.GameDataFileName);
    //}

    
}
