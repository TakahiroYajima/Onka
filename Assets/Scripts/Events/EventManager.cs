using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : SingletonMonoBehaviour<EventManager>
{
    private EventDataList eventDataList = null;
    private List<EventBase> eventObjectList = new List<EventBase>();

    private int inProgressEventArrayNum = -1;

    // Start is called before the first frame update
    void Start()
    {
        eventObjectList = GetComponentsInChildren<EventBase>().ToList();
        for(int i = 0; i < eventObjectList.Count; i++)
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
            Debug.Log("イベント情報ファイル:GameDataに無かったのでファイルからロード");
            eventDataList = FileManager.LoadSaveData<EventDataList>(DataManager.EventDataFileName);
            if (eventDataList == null || eventDataList == default || eventDataList.list.Count == 0)
            {
                Debug.Log("イベント情報ファイル:ファイルにも無かったので新規作成");
                eventDataList = new EventDataList();
                for (int i = 0; i < eventObjectList.Count; i++)
                {
                    eventDataList.list.Add(new EventData(eventObjectList[i].EventKey));
                }
                FileManager.DataSave<EventDataList>(eventDataList, DataManager.EventDataFileName);//念のため、一覧データもセーブ
            }
            DataManager.Instance.SetNewEventDataList(eventDataList);
            DataManager.Instance.SaveGameData();
            FirstInitializeSetting();
        }
    }

    private void Update()
    {
        if(inProgressEventArrayNum >= 0 && inProgressEventArrayNum < eventObjectList.Count)
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
            eventObjectList[i].ProgressEvent("");
        }
    }

    public void EventStart(int managementID)
    {
        Debug.Log("イベントID : " + managementID);
        if(inProgressEventArrayNum == -1)
        {
            if (StageManager.Instance.Player.currentState == PlayerState.Free)
            {
                inProgressEventArrayNum = managementID;
                eventObjectList[inProgressEventArrayNum].EventStart();
            }
            else
            {
                Debug.Log("プレイヤーのStateがFreeではありません : " + StageManager.Instance.Player.currentState.ToString());
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
            inProgressEventArrayNum = -1;
        }
        else
        {
            Debug.Log("イベントデータがありません : " + key);
        }
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