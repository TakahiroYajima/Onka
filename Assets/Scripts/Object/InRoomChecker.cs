using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 部屋の中に入っているかの情報を管理する
/// </summary>
public class InRoomChecker : MonoBehaviour
{
    //部屋に入っている時の情報
    private List<RoomWanderingManager> currentEnterRoomList = new List<RoomWanderingManager>();
    public IReadOnlyList<RoomWanderingManager> CurrentEnterRoomList { get { return currentEnterRoomList; } }
    public bool isEnterRoom { get { return currentEnterRoomList.Count > 0; } }//部屋に入っているか

    public UnityAction<RoomWanderingManager> onEnterRoomAction = null;
    public UnityAction<RoomWanderingManager> onExitRoomAction = null;
    
    public void SetEnterRoom(RoomWanderingManager roomWanderingManager)
    {
        if (!currentEnterRoomList.Contains(roomWanderingManager))
        {
            Debug.Log(transform.name + " : 部屋に入った : " + roomWanderingManager.name);
            currentEnterRoomList.Add(roomWanderingManager);
            if(onEnterRoomAction != null)
            {
                onEnterRoomAction(roomWanderingManager);
            }
        }
    }
    public void ExitRoom(RoomWanderingManager roomWanderingManager)
    {
        if (currentEnterRoomList.Contains(roomWanderingManager))
        {
            Debug.Log(transform.name + " : 部屋から出た : " + roomWanderingManager.name);
            currentEnterRoomList.Remove(roomWanderingManager);
            if(onExitRoomAction != null)
            {
                onExitRoomAction(roomWanderingManager);
            }
        }
    }

    //部屋から出る時の判定関連
    //部屋から出る時、出口の当たり判定に当たっているかどうかで部屋を出たかを判定する（部屋Aから部屋Bに入った場合、部屋Aを出たことにさせないため）
    private GameObject currentEnterExitColliderObj = null;
    public bool isEnterExitCollider { get { return currentEnterExitColliderObj != null; } }//部屋の出口のコライダーに当たっているか
    public void SetEnterExitColliderObj(GameObject obj)
    {
        currentEnterExitColliderObj = obj;
    }
    public void ExitEnterExitCollider()
    {
        currentEnterExitColliderObj = null;
    }
    public bool IsMatchExitCollider(GameObject obj)
    {
        if (currentEnterExitColliderObj == null) return false;
        return obj == currentEnterExitColliderObj;
    }
}
