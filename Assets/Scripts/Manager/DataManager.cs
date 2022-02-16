using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using SoundSystem;
using Onka.Manager.Event;

namespace Onka.Manager.Data
{
    public class DataManager : SingletonMonoBehaviour<DataManager>
    {
        //各マスターデータ
        private SoundDataSO soundDataSOMasterData = null;
        private UseSoundNameSO useSoundNameSOMasterData = null;
        private ItemDataList itemDatalistMasterData = null;
        private EventDataList eventDataListMasterData = null;

        //セーブデータ
        private GameData generalGameData = null;//1度クリアしたイベントや取得したアイテムを保存するためのバックグラウンドセーブデータ
        private GameData playingGameData = null;//プレイ中のゲームデータ
        private List<GameData> loadedAllGameDataList = new List<GameData>();
        private int currentSelectLoadDataArrayNum = 0;//ロードしたゲームデータのリスト番号
        public const int MaxSaveDataCount = 3;

        public GameData GetGeneralGameData()
        {
            return generalGameData;
        }
        public IReadOnlyList<ItemData> GetGeneralItemDataList()
        {
            return generalGameData.itemDataList.itemDataList;
        }

        public const string SoundDataFileName = "SoundData.txt";
        public const string UseSoundNameFileName = "UseSoundName.txt";
        public const string ItemDataFileName = "ItemData.txt";
        public const string CharactorDataFileName = "CharactorData.txt";

        public const string GameDataFileName = "GameData.txt";
        public const string GameDataBaseFileName = "GameData";
        public const string EventDataFileName = "EventData.txt";

        public const string SaveDateTimeFormat = "yyyy/MM/dd HH:mm:ss";

        protected override void Awake()
        {
            base.Awake();
            InitializeDataLoad();
        }

        // Start is called before the first frame update
        void Start()
        {
            SoundManager.Instance.InitSeLoad();
        }

        public void InitializeDataLoad()
        {
            soundDataSOMasterData = FileManager.LoadSaveData<SoundDataSO>(SaveType.MasterData, SoundDataFileName);
            useSoundNameSOMasterData = FileManager.LoadSaveData<UseSoundNameSO>(SaveType.MasterData, UseSoundNameFileName);
            itemDatalistMasterData = FileManager.LoadSaveData<ItemDataList>(SaveType.MasterData, ItemDataFileName);
            eventDataListMasterData = FileManager.LoadSaveData<EventDataList>(SaveType.MasterData, DataManager.EventDataFileName);

            generalGameData = FileManager.LoadSaveData<GameData>(SaveType.GeneralPlayerData, GameDataFileName);
            LoadAllSavedGameData();
        }
        private void LoadAllSavedGameData()
        {
            loadedAllGameDataList.Clear();
            StringBuilder fileName = new StringBuilder();
            for (int i = 0; i < MaxSaveDataCount; i++)
            {
                fileName.Append(GameDataBaseFileName);
                fileName.Append(i.ToString());
                fileName.Append(".txt");
                if (FileManager.Exists(SaveType.PlayerData, fileName.ToString()))
                {
                    //Debug.Log($"{fileName.ToString()} ロード");
                    loadedAllGameDataList.Add(FileManager.LoadSaveData<GameData>(SaveType.PlayerData, fileName.ToString()));
                }
                else
                {
                    //存在しないので作成
                    loadedAllGameDataList.Add(new GameData());
                    loadedAllGameDataList[i].itemDataList = new ItemDataList();
                    loadedAllGameDataList[i].itemDataList.itemDataList = new List<ItemData>(itemDatalistMasterData.itemDataList);
                    loadedAllGameDataList[i].AllInitialize();
                }
                fileName.Clear();
            }
        }
        #region GameData
        //----------------------------------------------------
        //GameData関連
        //----------------------------------------------------
        /// <summary>
        /// 遊ぶゲームデータを設定
        /// </summary>
        /// <param name="_selectSaveDataArrayNum"></param>
        public void SelectPlayGameData(int _selectSaveDataArrayNum)
        {
            currentSelectLoadDataArrayNum = _selectSaveDataArrayNum;
            playingGameData = loadedAllGameDataList[currentSelectLoadDataArrayNum];
        }
        /// <summary>
        /// 新規作成、まっさらな状態に上書き
        /// </summary>
        /// <param name="_selectSaveDataArrayNum"></param>
        public void NewCreateGameDataAndSave(int _selectSaveDataArrayNum, UnityAction onComplete = null)
        {
            loadedAllGameDataList[currentSelectLoadDataArrayNum] = GetInitializedData();
            currentSelectLoadDataArrayNum = _selectSaveDataArrayNum;
            playingGameData = loadedAllGameDataList[currentSelectLoadDataArrayNum];
            SaveGameData(onComplete);
        }

        private GameData GetInitializedData()
        {
            GameData data = new GameData();
            data.itemDataList = itemDatalistMasterData;
            data.eventDataList = eventDataListMasterData;
            data.AllInitialize();
            return data;
        }

        public bool IsAfterFirstSavedCurrentSelectData()
        {
            EventData data = playingGameData.eventDataList.list.FirstOrDefault(x => x.eventKey == "Event_Opnening");
            if (data == null || data == default) return false;
            return data.isEnded;
        }
        /// <summary>
        /// データをセーブ
        /// </summary>
        /// <param name="onComplete"></param>
        public void SaveGameData(UnityAction onComplete = null)
        {
            StringBuilder fileName = new StringBuilder();
            fileName.Append(GameDataBaseFileName);
            fileName.Append(currentSelectLoadDataArrayNum.ToString());
            fileName.Append(".txt");
            System.DateTime dateTime = System.DateTime.Now;
            string parceDate = dateTime.ToString(SaveDateTimeFormat);
            playingGameData.saveDate = parceDate;
            FileManager.DataSave<GameData>(playingGameData, SaveType.PlayerData, fileName.ToString(), () =>
            {
                UpdateAndSaveGeneralPlayerData(onComplete);
            });
        }

        /// <summary>
        /// ゲームデータが更新されたことでバックグラウンドセーブデータも更新する
        /// </summary>
        private void UpdateAndSaveGeneralPlayerData(UnityAction onComplete = null)
        {
            for (int i = 0; i < playingGameData.itemDataList.itemDataList.Count; i++)
            {
                ItemData item = generalGameData.itemDataList.itemDataList.FirstOrDefault(x => x.key == playingGameData.itemDataList.itemDataList[i].key);
                if (item == null || item == default) continue;

                if (!playingGameData.itemDataList.itemDataList[i].geted) continue;

                item.geted = playingGameData.itemDataList.itemDataList[i].geted;
                item.used = playingGameData.itemDataList.itemDataList[i].used;
            }
            for (int i = 0; i < playingGameData.eventDataList.list.Count; i++)
            {
                EventData item = generalGameData.eventDataList.list.FirstOrDefault(x => x.eventKey == playingGameData.eventDataList.list[i].eventKey);
                if (item == null || item == default) continue;

                if (!playingGameData.eventDataList.list[i].isEnded) continue;

                item.isAppeared = playingGameData.eventDataList.list[i].isAppeared;
                item.isEnded = playingGameData.eventDataList.list[i].isEnded;
            }
            FileManager.DataSave<GameData>(generalGameData, SaveType.GeneralPlayerData, GameDataFileName, () =>
            {
            //Debug.Log("ゲームデータセーブ完了");
            if (onComplete != null)
                {
                    onComplete();
                }
            });
        }

        public IReadOnlyList<GameData> GetAllGameDatas()
        {
            return loadedAllGameDataList;
        }
        /// <summary>
        /// タイトルにて、遊ぶゲームのデータを選択した際に呼び出す
        /// </summary>
        /// <param name="_selectArrayNum"></param>
        public void OnSelectGameData(int _selectArrayNum)
        {
            if (_selectArrayNum >= 0 && _selectArrayNum < loadedAllGameDataList.Count)
            {
                playingGameData = loadedAllGameDataList[_selectArrayNum];
                currentSelectLoadDataArrayNum = _selectArrayNum;
            }
        }
        #endregion

        #region SoundData
        //----------------------------------------------------
        //SoundDataSO
        //----------------------------------------------------
        public SoundDataSO GetSoundSO()
        {
            return soundDataSOMasterData;
        }
        public bool ContainsBGM(string key)
        {
            return soundDataSOMasterData.bgmList.FirstOrDefault(x => x.key == key) != null;
        }
        public bool ContainsSE(string key)
        {
            return soundDataSOMasterData.seList.FirstOrDefault(x => x.key == key) != null;
        }
        public bool ContainsMenuSE(string key)
        {
            return soundDataSOMasterData.menuSeList.FirstOrDefault(x => x.key == key) != null;
        }
        public bool ContainsVoice(string key)
        {
            return soundDataSOMasterData.voiceList.FirstOrDefault(x => x.key == key) != null;
        }
        public bool ContainsAmbient(string key)
        {
            return soundDataSOMasterData.ambientList.FirstOrDefault(x => x.key == key) != null;
        }
        public SoundData GetBGM(string key)
        {
            return soundDataSOMasterData.bgmList.FirstOrDefault(x => x.key == key);
        }
        public SoundData GetSE(string key)
        {
            return soundDataSOMasterData.seList.FirstOrDefault(x => x.key == key);
        }
        public SoundData GetMenuSE(string key)
        {
            return soundDataSOMasterData.menuSeList.FirstOrDefault(x => x.key == key);
        }
        public SoundData GetVoice(string key)
        {
            return soundDataSOMasterData.voiceList.FirstOrDefault(x => x.key == key);
        }
        public SoundData GetAmbient(string key)
        {
            return soundDataSOMasterData.ambientList.FirstOrDefault(x => x.key == key);
        }
        #endregion

        #region UseSoundName
        //----------------------------------------------------
        //UseSoundNameSO
        //----------------------------------------------------
        public void SetCurrentSceneUseSound(SceneType sceneType)
        {
            if (useSoundNameSOMasterData == null || useSoundNameSOMasterData == default)
            {
                Debug.Log("サウンド設定がありません");
                return;
            }
            List<UseSoundNameData> clips = useSoundNameSOMasterData.GetOneSceneUseData(sceneType);
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
        #endregion

        #region ItemData
        //----------------------------------------------------
        //ItemDataList
        //----------------------------------------------------

        public ItemData GetItemData(string _key)
        {
            return playingGameData.itemDataList.itemDataList.FirstOrDefault(x => x.key == _key);
        }
        public IReadOnlyList<ItemData> GetAllItemData()
        {
            return playingGameData.itemDataList.itemDataList;
        }

        /// <summary>
        /// アイテムのデータ更新（ゲーム内のみ。セーブ無し）
        /// </summary>
        /// <param name="_itemData"></param>
        private void UpdateItemData(ItemData _itemData)
        {
            for (int i = 0; i < playingGameData.itemDataList.itemDataList.Count; i++)
            {
                if (playingGameData.itemDataList.itemDataList[i].key == _itemData.key)
                {
                    playingGameData.itemDataList.itemDataList[i] = _itemData;
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
        /// <summary>
        /// アイテムを取得させる
        /// </summary>
        /// <param name="_item"></param>
        /// <param name="onComplete"></param>
        public void ItemGetAction(ItemData _item, UnityAction<ItemData> onComplete = null)
        {
            if (_item == null) { Debug.LogError("アイテムがありません : " + _item.key); return; }

            _item.geted = true;
            if (onComplete != null)
            {
                onComplete(_item);
            }
        }
        /// <summary>
        /// アイテムを使用済みにする
        /// </summary>
        /// <param name="_item"></param>
        public void SetUsedItem(ItemData _item)
        {
            ItemData itemData = playingGameData.itemDataList.itemDataList.FirstOrDefault(x => x.key == _item.key);
            if (itemData == null) { Debug.LogError("アイテムがありません : " + _item.key); return; }

            itemData.used = true;
        }
        #endregion

        #region EventData
        //----------------------------------------------------
        //EventData
        //----------------------------------------------------
        public EventDataList GetEventDatas()
        {
            return playingGameData.eventDataList;
        }
        public void SetNewEventDataList(EventDataList eventDataList)
        {
            playingGameData.eventDataList = new EventDataList(eventDataList);
        }
        #endregion





        #region Debug
        //----------------------------------------------------
        //デバッグ用
        //----------------------------------------------------
        public List<ItemData> GetAllItemData_Debug()
        {
            return playingGameData.itemDataList.itemDataList;
        }
        public List<EventData> GetAllEventData_Debug()
        {
            return playingGameData.eventDataList.list;
        }
        #endregion
    }
}