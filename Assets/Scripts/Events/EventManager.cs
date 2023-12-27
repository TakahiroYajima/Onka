using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Onka.Manager.Data;

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

        private Dictionary<string, UseEventObject> useEventObjects = new Dictionary<string, UseEventObject>();

        //private int inProgressEventArrayNum = -1;
        public bool IsAnyEventEnabled { get { return doingEventList.Count > 0; /*inProgressEventArrayNum > -1;*/ } }
        public bool isEnable = false;
        public string[] DoingEventKeys => doingEventList.Select(v => v.EventKey).ToArray();

        public void AddUseEventObjects(List<UseEventObject> list)
        {
            foreach (var ueo in list)
            {
                if (!useEventObjects.ContainsKey(ueo.objectKey))
                {
                    useEventObjects.Add(ueo.objectKey, ueo);
                }
            }
        }
        public UseEventObject GetUseEventObject(string key)
        {
            if (useEventObjects.ContainsKey(key))
            {
                return useEventObjects[key];
            }
            return null;
        }

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
                eventDataList = FileManager.LoadSaveData<EventDataList>(SaveType.MasterData, DataManager.EventDataFileName);
                if (eventDataList == null || eventDataList == default || eventDataList.list.Count == 0)
                {
                    //Debug.Log("イベント情報ファイル:ファイルにも無かったので新規作成");
                    eventDataList = new EventDataList();
                    for (int i = 0; i < eventObjectList.Count; i++)
                    {
                        eventDataList.list.Add(new EventData(eventObjectList[i].EventKey));
                    }
                    //DataManager.Instance.SetNewEventDataList(eventDataList);
                    //DataManager.Instance.SaveEventData();//念のため、一覧データもセーブ
                }
                DataManager.Instance.SetNewEventDataList(eventDataList);
                DataManager.Instance.SaveGameData();
                FirstInitializeSetting();
            }
        }

        private void Update()
        {
            if (!isEnable) return;
            if(doingEventList.Count > 0)
            {
                for(int i = 0; i < doingEventList.Count; i++)
                {
                    //Debug.Log("doing : " + i + " : " + doingEventList[i].EventKey);
                    if(doingEventList[i] == null) { continue; }
                    doingEventList[i].EventUpdate();
                }
            }
        }
        /// <summary>
        /// 初期化時限定
        /// </summary>
        public void InitProgressEach()
        {
            for (int i = 0; i < eventObjectList.Count; i++)
            {
                if (eventObjectList[i] != null)
                {
                    eventObjectList[i].InitProgress();
                    eventObjectList[i].ProgressEvent();
                }
            }
        }
        /// <summary>
        /// イベント出現判定を更新
        /// </summary>
        public void ProgressEvent()
        {
            if (!isEnable) return;
            for (int i = 0; i < eventObjectList.Count; i++)
            {
                if (eventObjectList[i] != null)
                {
                    eventObjectList[i].ProgressEvent();
                }
            }
        }

        public void RequestEventStart(string eventKey, UnityEngine.Events.UnityAction onCompleted)
        {
            EventBase gimmickEvent = null;
            //本当はクリア済みのものをリストから取り除きたいが、過去に作った仕様を忘れたため一旦foreachで回す。時間がある時に調整
            foreach(var v in eventObjectList)
            {
                if (v == null) continue;
                if(v.EventKey == eventKey)
                {
                    gimmickEvent = v;
                    break;
                }
            }
            if(gimmickEvent != null)
            {
                gimmickEvent.ForceStartEvent(onCompleted);
            }
            else
            {
                Debug.LogError($"EventKeyがありません : {eventKey}");
            }
        }

        /// <summary>
        /// イベント開始リクエスト
        /// </summary>
        /// <param name="managementID"></param>
        /// <param name="isSingleOnlyEvent"></param>
        public void RequestEventStart(int managementID, bool isSingleOnlyEvent = false)
        {
            if(eventObjectList[managementID] == null) { Debug.LogError("イベントが存在しません : " + managementID.ToString()); return; }

            //Debug.Log("イベント : " + managementID + " : " + eventObjectList[managementID].EventKey + " : " + eventObjectList[managementID].canBeStarted);
            //if (inProgressEventArrayNum == -1 || !doingEventList.Contains(eventObjectList[managementID]))
            //{
            if(isSingleOnlyEvent && IsAnyEventEnabled) { Debug.LogError("単一のみのイベントがある中、進行中のイベントがあります : "); foreach(var e in doingEventList) { Debug.LogError(e.EventKey); } }
            if(!doingEventList.Contains(eventObjectList[managementID]))
            {
                if (StageManager.Instance.Player.isEventEnabled)
                {
                    if (eventObjectList[managementID].canBeStarted)
                    {
                        //inProgressEventArrayNum = managementID;
                        //eventObjectList[inProgressEventArrayNum].EventStart();
                        doingEventList.Add(eventObjectList[managementID]);
                        Debug.Log("EventStart : " + eventObjectList[managementID].EventKey);
                        eventObjectList[managementID].EventStart();
                    }
                }
                else
                {
                    //Debug.Log("プレイヤーのStateからイベント拒否 : " + StageManager.Instance.Player.currentState.ToString());
                }
            }
            else
            {
                //Debug.Log("イベントが被っています : " + managementID.ToString() + " key : " + eventObjectList[managementID].EventKey);
                //Debug.Log("イベントが被っています : " + managementID.ToString() + " key : " + eventObjectList[inProgressEventArrayNum].EventKey);
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
                EventBase eventBase = doingEventList.FirstOrDefault(e => e.EventKey == key);
                //Debug.Log("EventClear() : " + key);
                if(eventBase != null)
                {
                    //Debug.Log("RemoveEvent : " + eventBase.EventKey);
                    doingEventList.Remove(eventBase);
                    eventBase.EventEnd();
                }
                else { Debug.LogError("クリアするイベントのキーが一致しません : " + key); }
                ClearedEventDestroy(key);
                //eventObjectList[inProgressEventArrayNum].EventEnd();
                //ClearedEventDestroy(inProgressEventArrayNum);
                //inProgressEventArrayNum = -1;
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
            //Debug.Log("EventDestroy : " + eventObjectList[arrayNum].EventKey);
            if(doingEventList.FirstOrDefault(e => e.EventKey == eventObjectList[arrayNum].EventKey))
            {
                doingEventList.Remove(eventObjectList[arrayNum]);
            }
            Destroy(eventObjectList[arrayNum].gameObject);
            eventObjectList[arrayNum] = null;
        }
        public void ClearedEventDestroy(string key)
        {
            int arrayNum = -1;
            for(int i = 0; i < eventObjectList.Count; i++)
            {
                if(eventObjectList[i] == null) { continue; }
                if(eventObjectList[i].EventKey == key)
                {
                    arrayNum = i;
                    break;
                }
            }
            ClearedEventDestroy(arrayNum);
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