using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnkaDebug : MonoBehaviour
{
    private bool isDoBeforeClearState = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.F))
        {
            DoBeforeClearState();
        }
        if (isDoBeforeClearState)
        {
            if(Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A))
            {
                DoGetEntranceKey();
                Onka.Manager.Event.EventManager.Instance.ProgressEvent();
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
            if (eventData.eventKey != "Event_AfterOutHouse" && eventData.eventKey != "Event_NoteFinalActive")
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
