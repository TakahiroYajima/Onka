﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Onka.Manager.Event
{
    /// <summary>
    /// ステージ上のイベントを管理。GameSceneにのみ存在
    /// </summary>
    public class EventManager : SingletonMonoBehaviour<EventManager>
    {
        private EventDataList eventDataList = null;
        private List<EventBase> eventObjectList = new List<EventBase>();
        private List<EventBase> doingEventList = new List<EventBase>();//実行中のイベント

        private int inProgressEventArrayNum = -1;
        public bool IsAnyEventEnabled { get { return inProgressEventArrayNum > -1; } }

        public void Initialize()
        {
            eventObjectList = GetComponentsInChildren<EventBase>().ToList();
            for (int i = 0; i < eventObjectList.Count; i++)
            {
                eventObjectList[i].managementID = i;
            }

            FirstInitializeSetting();
        }

        public void FirstInitializeSetting()
        {
            eventDataList = DataManager.Instance.GetEventDatas();
            if (eventDataList == null || eventDataList == default || eventDataList.list.Count == 0)
            {
                //Debug.Log("イベント情報ファイル:GameDataに無かったのでファイルからロード");
                eventDataList = FileManager.LoadSaveData<EventDataList>(SaveType.Normal, DataManager.EventDataFileName);
                if (eventDataList == null || eventDataList == default || eventDataList.list.Count == 0)
                {
                    //Debug.Log("イベント情報ファイル:ファイルにも無かったので新規作成");
                    eventDataList = new EventDataList();
                    for (int i = 0; i < eventObjectList.Count; i++)
                    {
                        eventDataList.list.Add(new EventData(eventObjectList[i].EventKey));
                    }
                    FileManager.DataSave<EventDataList>(eventDataList, SaveType.Normal, DataManager.EventDataFileName);//念のため、一覧データもセーブ
                }
                DataManager.Instance.SetNewEventDataList(eventDataList);
                DataManager.Instance.SaveGameData();
                FirstInitializeSetting();
            }
        }

        private void Update()
        {
            if (inProgressEventArrayNum >= 0 && inProgressEventArrayNum < eventObjectList.Count)
            {
                eventObjectList[inProgressEventArrayNum].EventUpdate();
            }
        }
        /// <summary>
        /// イベント出現判定を更新
        /// </summary>
        public void ProgressEvent()
        {
            for (int i = 0; i < eventObjectList.Count; i++)
            {
                if (eventObjectList[i] != null)
                {
                    eventObjectList[i].ProgressEvent();
                }
            }
        }

        public void EventStart(int managementID)
        {
            if(eventObjectList[managementID] == null) { Debug.LogError("イベントが存在しません : " + managementID.ToString()); return; }

            Debug.Log("イベント : " + managementID + " : " + eventObjectList[managementID].EventKey + " : " + eventObjectList[managementID].canBeStarted);
            if (inProgressEventArrayNum == -1)
            {
                if (StageManager.Instance.Player.isEventEnabled)
                {
                    if (eventObjectList[managementID].canBeStarted)
                    {
                        inProgressEventArrayNum = managementID;
                        eventObjectList[inProgressEventArrayNum].EventStart();
                    }
                }
                else
                {
                    Debug.Log("プレイヤーのStateからイベント拒否 : " + StageManager.Instance.Player.currentState.ToString());
                }
            }
            else
            {
                Debug.Log("イベントが被っています : " + managementID.ToString() + " key : " + eventObjectList[inProgressEventArrayNum].EventKey);
            }
        }

        /// <summary>
        /// イベント終了処理
        /// </summary>
        /// <param name="key"></param>
        public void EventClear(string key)
        {
            EventData data = DataManager.Instance.GetEventDatas().list.FirstOrDefault(x => x.eventKey == key);
            if (data != null)
            {
                data.isEnded = true;
                eventObjectList[inProgressEventArrayNum].EventEnd();
                ClearedEventDestroy(inProgressEventArrayNum);
                inProgressEventArrayNum = -1;
                ProgressEvent();
            }
            else
            {
                Debug.Log("イベントデータがありません : " + key);
            }
        }

        public void ClearedEventDestroy(int arrayNum)
        {
            if (arrayNum < 0 || arrayNum >= eventObjectList.Count) return;
            if (eventObjectList[arrayNum] == null) return;
            Debug.Log("EventDestroy : " + eventObjectList[arrayNum].EventKey);
            Destroy(eventObjectList[arrayNum].gameObject);
            eventObjectList[arrayNum] = null;
        }

        public bool IsEventEnded(string key)
        {
            EventData data = DataManager.Instance.GetEventDatas().list.FirstOrDefault(x => x.eventKey == key);
            if (data != null)
            {
                return data.isEnded;
            }
            else
            {
                Debug.Log("イベントデータがありません : " + key);
                return false;
            }
        }
    }

    [System.Serializable]
    public class EventDataList
    {
        public List<EventData> list = new List<EventData>();
        public EventDataList()
        {
        }
        public EventDataList(EventDataList _list)
        {
            list = new List<EventData>(_list.list);
        }
    }

    [System.Serializable]
    public class EventData
    {
        public string eventKey = "";
        public bool isAppeared = false;//出現済み判定（アイテムの獲得状況で判断するのでいらない？）
        public bool isEnded = false;//イベントを体験したか判定

        public EventData(string key)
        {
            eventKey = key;
        }
    }
}