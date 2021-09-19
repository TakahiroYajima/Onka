using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class DebugUtilityMenu : MonoBehaviour
{
    [MenuItem("Debug/GameData/データ初期化")]
    public static void InitGameData()
    {
        GameData data = FileManager.LoadSaveData<GameData>(DataManager.GameDataFileName);
        if(data == null || data == default)
        {
            Debug.Log("データがないので初期化の必要はありません。");
        }
        else
        {
            data.AllInitialize();
            FileManager.DataSave<GameData>(data, DataManager.GameDataFileName);
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Debug/GameData/アイテム更新")]
    public static void UpdateItemData()
    {
        GameData data = FileManager.LoadSaveData<GameData>(DataManager.GameDataFileName);
        if (data == null || data == default)
        {
            data = new GameData();
            data.itemDataList = new ItemDataList();
            data.itemDataList.itemDataList = new List<ItemData>();
        }

        ItemDataList itemDataList = FileManager.LoadSaveData<ItemDataList>(DataManager.ItemDataFileName);
        if (data == null || data == default)
        {
            Debug.LogError("アイテムデータがありません");
            return;
        }
        
        for(int i = 0; i < itemDataList.itemDataList.Count; i++)
        {
            ItemData iData = data.itemDataList.itemDataList.FirstOrDefault(x => x.key == itemDataList.itemDataList[i].key);
            if(iData == null || iData == default)
            {
                data.itemDataList.itemDataList.Add(itemDataList.itemDataList[i]);
            }
            else
            {
                iData.name = itemDataList.itemDataList[i].name;
                iData.spriteName = itemDataList.itemDataList[i].spriteName;
                iData.type = itemDataList.itemDataList[i].type;
                iData.description = itemDataList.itemDataList[i].description;
                iData.fileItem = itemDataList.itemDataList[i].fileItem;
            }
        }
        FileManager.DataSave<GameData>(data, DataManager.GameDataFileName);
    }
}
