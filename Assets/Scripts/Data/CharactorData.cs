using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharactorDataList
{
    public List<CharactorData> charactorDataList = new List<CharactorData>();
}

[System.Serializable]
public class CharactorData
{
    public int id;
    public string key;
    public string name;
    public string name_en;
    public bool isCharactorName = true;//nameが人物の名前か（例：「主人公」では人物名ではない）
    public int age;
    public Gender gender;
    public string imageName;
    public Sprite sprite;
    public string description;
    public string description_en;
    public string Name { get { return TextMaster.HandOverMaster(name, name_en); } }
    public string Description { get { return TextMaster.HandOverMaster(description, description_en); } }
}

//ページでキャラの説明を分けようと思ったが、スクロールにすれば良いと思ったのでやめた
//[System.Serializable]
//public class CharactorPage
//{
//    public string imageName;
//    public string description;
//}

[System.Serializable]
public enum Gender
{
    Man,
    Woman,
}

public static class EnumUtil
{
    public static string PerseGenderStr(Gender gender)
    {
        switch (gender)
        {
            case Gender.Man:return TextMaster.GetText("text_gender_man");
            case Gender.Woman:return TextMaster.GetText("text_gender_woman");
            default:return TextMaster.GetText("text_gender_unknown");
        }
    }
}