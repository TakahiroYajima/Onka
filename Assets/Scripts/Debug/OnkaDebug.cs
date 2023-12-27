#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Onka.Manager.Data;
using Onka.Manager.Event;
using System.Linq;

public class OnkaDebug : SingletonMonoBehaviour<OnkaDebug>
{
    private bool isDoBeforeClearState = false;
    private SceneType currentSceneType = SceneType.Initialize;

    private GameSceneManager gameSceneManager;

    public void SetCurrentScene(SceneBase sceneBase)
    {
        switch (sceneBase.CurrentScene)
        {
            case SceneType.Game:
                gameSceneManager = sceneBase as GameSceneManager;
                break;
        }
        currentSceneType = sceneBase.CurrentScene;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(currentSceneType == SceneType.Game)
        {
            //初回オープニングスキップ
            if(gameSceneManager.IsOpening && Input.GetKey(KeyCode.S))
            {
                gameSceneManager.SkipOpening();
                return;
            }

            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F))
            {
                DoBeforeClearState();
            }
            if (isDoBeforeClearState)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A))
                {
                    DoGetEntranceKey();
                    Onka.Manager.Event.EventManager.Instance.ProgressEvent();
                }
            }
        }
    }

    /// <summary>
    /// クリア前（玄関のカギを手に入れる前）の状態にする
    /// </summary>
    private void DoBeforeClearState()
    {
        foreach (var eventData in DataManager.Instance.GetAllEventData_Debug())
        {
            if (eventData.eventKey != "Event_AfterOutHouse" && eventData.eventKey != "Event_NoteFinalActive" && eventData.eventKey != "Event_AftertGetEntranceKey")
            {
                eventData.isEnded = true;
            }
        }
        foreach(var itemData in DataManager.Instance.GetAllItemData_Debug())
        {
            if(itemData.key != "key_entrance" && itemData.key != "note_final")
            {
                itemData.geted = true;
                if(itemData.type == ItemType.DoorKey || itemData.type == ItemType.Useable)
                {
                    itemData.used = true;
                }
            }
        }
        isDoBeforeClearState = true;
    }

    private void DoGetEntranceKey()
    {
        DataManager.Instance.GetItemData("key_entrance").geted = true;
    }
}
#endif