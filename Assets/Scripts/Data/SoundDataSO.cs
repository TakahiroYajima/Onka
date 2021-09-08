using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class SoundDataSO
{
    public List<SoundData> soundDataList = new List<SoundData>();//大元のリスト

    public List<SoundData> bgmList = new List<SoundData>();
    public List<SoundData> seList = new List<SoundData>();
    public List<SoundData> menuSeList  = new List<SoundData>();
    public List<SoundData> voiceList = new List<SoundData>();
    public List<SoundData> ambientList = new List<SoundData>();

    public void SplitSoundDatas()
    {
        bgmList = soundDataList.Where(x => x.type == SoundType.BGM).ToList();
        seList = soundDataList.Where(x => x.type == SoundType.SE).ToList();
        menuSeList = soundDataList.Where(x => x.type == SoundType.MenuSE).ToList();
        voiceList = soundDataList.Where(x => x.type == SoundType.Voice).ToList();
        ambientList = soundDataList.Where(x => x.type == SoundType.Ambient).ToList();
    }

    public bool Contains(string key)
    {
        return soundDataList.FirstOrDefault(x => x.key == key) != null;
    }
    public SoundData GetSound(string key)
    {
        return soundDataList.FirstOrDefault(x => x.key == key);
    }
}
[System.Serializable]
public class SoundData
{
    public string key;
    public SoundType type;
    public string soundName;
    public string title;
    public float volume;
    public float spatialBlend;//2D~3Dサウンドの割合（BGM,Ambient,menuSEは強制で0）
}

[System.Serializable]
public enum SoundType
{
    BGM,
    SE,
    MenuSE,
    Voice,
    Ambient,//環境音
}