using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using Encrypter;

public sealed class FileManager
{
    const string SaveFilePath = "/SaveData";
    const string EncryptKey = "1234567890ABCDEF";
    const string srcIV = "QAWSEDRFTGYHUJIK";

    public static void DataSave<Types>(Types saveClass, string fileName, UnityAction onComplete = null)
    {
        string json = JsonUtility.ToJson(saveClass);
        Debug.Log("SaveData : " + json);
        //暗号化
        string iv = EncryptUtility.CalcMd5Str(srcIV, srcIV.Length);
        string outJson = "";
        EncryptUtility.EncryptAesBase64(json, EncryptKey, iv, out outJson);
        //セーブ
        Save(fileName, outJson, onComplete);
    }
    /// <summary>
    /// ファイルをロード。存在しなければdefaultを返す
    /// </summary>
    /// <typeparam name="Types"></typeparam>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static Types LoadSaveData<Types>(string fileName)
    {
        string path = Application.dataPath + SaveFilePath + "/" + fileName;
        string base64 = Read(fileName);
        if (string.IsNullOrEmpty(base64))
        {
            return default;
        }

        //複合化
        string iv = EncryptUtility.CalcMd5Str(srcIV, srcIV.Length);
        string outJson = "";
        EncryptUtility.DecryptAesBase64(base64, EncryptKey, iv, out outJson);
        Debug.Log("LoadData : " + outJson);
        if (!string.IsNullOrEmpty(outJson))
        {
            return JsonUtility.FromJson<Types>(outJson);
        }
        else
        {
            return default;
        }
    }

    private static void Save(string fileName, string json, UnityAction onComplete = null)
    {
        string filePath = Application.dataPath + SaveFilePath;
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
    private static string Read(string fileName)
    {
        string filePath = Application.dataPath + SaveFilePath;
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
