using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDataList
{
    public List<ItemData> itemDataList = new List<ItemData>();

    public void Save()
    {
        for(int i = 0; i < itemDataList.Count; i++)
        {
            if (itemDataList[i].type != ItemType.WatchOnly)
            {
                itemDataList[i].fileItem = null;
            }
        }
    }
}

[System.Serializable]
public class ItemData
{
    public ItemType type;
    public string key;//アイテムのID、鍵ならキーID
    public string name;
    public string spriteName;
    public string description;
    public string description_detail;//おまけ閲覧用
    public FileItem fileItem;
    public bool geted = false;//取得済みか
    public bool used = false;//使用済みか
}

[System.Serializable]
public enum ItemType
{
    DoorKey,
    WatchOnly,//取得できず、その場にあるだけ（日記など）
    Useable,//どこかで使うもの
    GetOnly,//取得するだけ
}

/// <summary>
/// 記憶のファイル（本のように読むもの）
/// </summary>
[System.Serializable]
public class FileItem
{
    public FontType fontType = FontType.AppliMincho;
    public int red = 255;
    public int green = 255;
    public int blue = 255;
    public int alpha = 255;
    public List<string> content = new List<string>();

    public Color GetTextColor()
    {
        return new Color((float)red / 255f, (float)green / 255f, (float)blue / 255f, (float)alpha / 100f);
    }
}

[System.Serializable]
public enum FontType
{
    Arial = 0,
    AppliMincho,
    Onryou,
}