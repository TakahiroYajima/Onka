using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using Encrypter;

public enum SaveType
{
    Normal,
    PlayerData,//セーブデータ
    GeneralPlayerData,//1度取得すれば取得済みフラグが立つもの
    MasterData,//最初からあり、更新できないもの
    SettingData,//言語設定
}

public sealed class FileManager
{
    const string EncryptKey = "1234567890ABCDEF";
    const string srcIV = "QAWSEDRFTGYHUJIK";

    static readonly Dictionary<SaveType, string> saveFolderPath = new Dictionary<SaveType, string>()
    {
        {SaveType.Normal, "/SaveData" },
        {SaveType.PlayerData,"/SaveData/PlayerData" },
        {SaveType.GeneralPlayerData, "/SaveData/GeneralPlayerData" },
        {SaveType.MasterData, "/SaveData/MasterData" },
        {SaveType.SettingData, "/SaveData/SettingData" },
    };

    public static void DataSave<Types>(Types saveClass, SaveType saveType, string fileName, UnityAction onComplete = null)
    {
        string json = JsonUtility.ToJson(saveClass);
        //Debug.Log("SaveData : " + json);
        //暗号化
        string iv = EncryptUtility.CalcMd5Str(srcIV, srcIV.Length);
        string outJson = "";
        EncryptUtility.EncryptAesBase64(json, EncryptKey, iv, out outJson);
        //セーブ
        Save(saveType, fileName, outJson, onComplete);
    }
    /// <summary>
    /// ファイルをロード。存在しなければdefaultを返す
    /// </summary>
    /// <typeparam name="Types"></typeparam>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static Types LoadSaveData<Types>(SaveType saveType, string fileName)
    {
        string base64 = Read(saveType, fileName);
        if (string.IsNullOrEmpty(base64))
        {
            return default;
        }

        //複合化
        string iv = EncryptUtility.CalcMd5Str(srcIV, srcIV.Length);
        string outJson = "";
        EncryptUtility.DecryptAesBase64(base64, EncryptKey, iv, out outJson);
        //Debug.Log("LoadData : " + outJson);
        if (!string.IsNullOrEmpty(outJson))
        {
            return JsonUtility.FromJson<Types>(outJson);
        }
        else
        {
            return default;
        }
    }

    public static bool Exists(SaveType saveType, string fileName)
    {
        //Debug.Log($"{fileName } Exists {saveType.ToString()}");
        string filePath = Application.streamingAssetsPath + saveFolderPath[saveType];
        string path = filePath + "/" + fileName;
        if (!Directory.Exists(filePath)) return false;
        if (!File.Exists(path)) return false;

        return true;
    }

    private static void Save(SaveType saveType, string fileName, string json, UnityAction onComplete = null)
    {
        string filePath = Application.streamingAssetsPath + saveFolderPath[saveType];
        string path = filePath + "/" + fileName;
        //ディレクトリが無ければ作成
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }

        File.WriteAllText(path, json);
        if(onComplete != null)
        {
            onComplete();
        }
    }
    private static string Read(SaveType saveType, string fileName)
    {
        string filePath = Application.streamingAssetsPath + saveFolderPath[saveType];
        string path = filePath + "/" + fileName;
        string data;
        if (!Directory.Exists(filePath))
        {
            return null;
        }
        else
        {
            if (File.Exists(path))
            {
                data = File.ReadAllText(path);
                return data;
            }
            else
            {
                return null;
            }
        }
    }
}
