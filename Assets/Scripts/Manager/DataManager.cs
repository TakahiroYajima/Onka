using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using SoundSystem;
using Onka.Manager.Event;

public class DataManager : SingletonMonoBehaviour<DataManager>
{
    private SoundDataSO soundDataSO = null;
    private UseSoundNameSO useSoundNameSO = null;
    private ItemDataList itemDatalist = null;

    private GameData gameData = null;

    public const string SoundDataFileName = "SoundData.txt";
    public const string UseSoundNameFileName = "UseSoundName.txt";
    public const string ItemDataFileName = "ItemData.txt";

    public const string GameDataFileName = "GameData.txt";
    public const string EventDataFileName = "EventData.txt";

    protected override void Awake()
    {
        base.Awake();
        soundDataSO = FileManager.LoadSaveData<SoundDataSO>(SaveType.Normal, SoundDataFileName);

        useSoundNameSO = FileManager.LoadSaveData<UseSoundNameSO>(SaveType.Normal, UseSoundNameFileName);
        itemDatalist = FileManager.LoadSaveData<ItemDataList>(SaveType.Normal, ItemDataFileName);

        //ゲーム中のデータをロード。なければ新規作成
        gameData = FileManager.LoadSaveData<GameData>(SaveType.Normal, GameDataFileName);
        if (gameData == null || gameData == default)
        {
            gameData = new GameData();
            gameData.itemDataList = new ItemDataList();
            gameData.itemDataList.itemDataList = new List<ItemData>(itemDatalist.itemDataList);
            gameData.AllInitialize();
            SaveGameData();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.InitSeLoad();
    }
    //----------------------------------------------------
    //SoundDataSO
    //----------------------------------------------------
    public SoundDataSO GetSoundSO()
    {
        return soundDataSO;
    }
    public bool ContainsBGM(string key)
    {
        return soundDataSO.bgmList.FirstOrDefault(x => x.key == key) != null;
    }
    public bool ContainsSE(string key)
    {
        return soundDataSO.seList.FirstOrDefault(x => x.key == key) != null;
    }
    public bool ContainsMenuSE(string key)
    {
        return soundDataSO.menuSeList.FirstOrDefault(x => x.key == key) != null;
    }
    public bool ContainsVoice(string key)
    {
        return soundDataSO.voiceList.FirstOrDefault(x => x.key == key) != null;
    }
    public bool ContainsAmbient(string key)
    {
        return soundDataSO.ambientList.FirstOrDefault(x => x.key == key) != null;
    }
    public SoundData GetBGM(string key)
    {
        return soundDataSO.bgmList.FirstOrDefault(x => x.key == key);
    }
    public SoundData GetSE(string key)
    {
        return soundDataSO.seList.FirstOrDefault(x => x.key == key);
    }
    public SoundData GetMenuSE(string key)
    {
        return soundDataSO.menuSeList.FirstOrDefault(x => x.key == key);
    }
    public SoundData GetVoice(string key)
    {
        return soundDataSO.voiceList.FirstOrDefault(x => x.key == key);
    }
    public SoundData GetAmbient(string key)
    {
        return soundDataSO.ambientList.FirstOrDefault(x => x.key == key);
    }

    //----------------------------------------------------
    //UseSoundNameSO
    //----------------------------------------------------
    public void SetCurrentSceneUseSound(SceneType sceneType)
    {
        if(useSoundNameSO == null || useSoundNameSO == default)
        {
            Debug.Log("サウンド設定がありません");
            return;
        }
        List<UseSoundNameData> clips = useSoundNameSO.GetOneSceneUseData(sceneType);
        //Debug.Log("SceneType : " + sceneType.ToString());
        //Debug.Log("SceneSoundsCount : " + useSoundNameSO.useSoundNameDataList.Count);
        //Debug.Log("thisSceneSound.Count : " + clips.Count);
        //foreach(var b in soundDataSO.bgmList) { Debug.Log(b.soundName); }
        List<SoundData> bgmList = new List<SoundData>();
        List<SoundData> ambientList = new List<SoundData>();
        foreach (var c in clips)
        {
            //Debug.Log(c.key);
            SoundData d = GetBGM(c.key);
            if (d != null) { bgmList.Add(d); }
            SoundData a = GetAmbient(c.key);
            if (a != null) { ambientList.Add(a); }
        }
        
        SoundManager.Instance.bgmAudioClipList.Clear();
        SoundManager.Instance.environmentAudioClipList.Clear();

        foreach (var str in bgmList)
        {
            AudioClip clip = Resources.Load("Sounds/BGM/" + str.soundName) as AudioClip;
            SoundManager.Instance.bgmAudioClipList.Add(clip);
            //Debug.Log("BGM追加：" + clip.name + " : " + str.soundName);
        }
        foreach (var str in ambientList)
        {
            AudioClip clip = Resources.Load("Sounds/Ambient/" + str.soundName) as AudioClip;
            SoundManager.Instance.environmentAudioClipList.Add(clip);
            //Debug.Log("Ambient追加：" + clip.name + " : " + str.soundName);
        }
    }

    //----------------------------------------------------
    //ItemDataList
    //----------------------------------------------------

    public ItemData GetItemData(string _key)
    {
        return gameData.itemDataList.itemDataList.FirstOrDefault(x => x.key == _key);
    }
    /// <summary>
    /// アイテムのデータ更新（ゲーム内のみ。セーブ無し）
    /// </summary>
    /// <param name="_itemData"></param>
    private void UpdateItemData(ItemData _itemData)
    {
        for(int i = 0; i < gameData.itemDataList.itemDataList.Count; i++)
        {
            if(gameData.itemDataList.itemDataList[i].key == _itemData.key)
            {
                gameData.itemDataList.itemDataList[i] = _itemData;
                break;
            }
        }
    }

    public bool IsKeyUnlocked(string _keyLockItemKey)
    {
        ItemData itemData = GetItemData(_keyLockItemKey);
        if (itemData == null) { Debug.LogError("ドアのアイテムが存在しません : " + _keyLockItemKey); return false; }

        return itemData.used;//ドアのキーを使用しているかで判定
    }

    public void DoDoorUnlock(string _doorKey, UnityAction onSuccess)
    {
        ItemData itemData = GetItemData(_doorKey);
        if (itemData == null) { Debug.LogError("ドアのアイテムが存在しません : " + _doorKey); return; }
        if (itemData.geted)
        {
            itemData.used = true;
            UpdateItemData(itemData);
            onSuccess();
        }
    }

    public void ItemGetAction(ItemData _item, UnityAction<ItemData> onComplete = null)
    {
        ItemData itemData = gameData.itemDataList.itemDataList.FirstOrDefault(x => x.key == _item.key);
        if(itemData == null) { Debug.LogError("アイテムがありません : " + _item.key); return; }

        itemData.geted = true;
        if (onComplete != null)
        {
            onComplete(_item);
        }
        //for (int i = 0; i < gameData.itemDataList.itemDataList.Count; i++)
        //{
        //    if(gameData.itemDataList.itemDataList[i].key == _item.key)
        //    {
        //        gameData.itemDataList.itemDataList[i].geted = true;
        //        if(onComplete != null)
        //        {
        //            onComplete(_item);
        //        }
        //        break;
        //    }
        //}
    }

    public void ItemUseAction(ItemData _item)
    {
        ItemData itemData = gameData.itemDataList.itemDataList.FirstOrDefault(x => x.key == _item.key);
        if (itemData == null) { Debug.LogError("アイテムがありません : " + _item.key); return; }

        itemData.used = true;

        //for (int i = 0; i < gameData.itemDataList.itemDataList.Count; i++)
        //{
        //    if (gameData.itemDataList.itemDataList[i].key == _item.key)
        //    {
        //        gameData.itemDataList.itemDataList[i].used = true;
        //    }
        //}
    }

    //----------------------------------------------------
    //EventData
    //----------------------------------------------------
    public EventDataList GetEventDatas()
    {
        return gameData.eventDataList;
    }
    public void SetNewEventDataList(EventDataList eventDataList)
    {
        gameData.eventDataList = new EventDataList(eventDataList);
    }

    //----------------------------------------------------
    //GameData本体
    //----------------------------------------------------
    /// <summary>
    /// データをセーブ
    /// </summary>
    /// <param name="onComplete"></param>
    public void SaveGameData(UnityAction onComplete = null)
    {
        FileManager.DataSave<GameData>(gameData,SaveType.Normal, GameDataFileName,()=>
        {
            Debug.Log("ゲームデータセーブ完了");
            if(onComplete != null)
            {
                onComplete();
            }
        });
    }
}

