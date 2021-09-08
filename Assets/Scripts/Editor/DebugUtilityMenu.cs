using System.Collections;
using System.Collections.Generic;
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
}
