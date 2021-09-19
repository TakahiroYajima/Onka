using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public abstract class DoorObject : MonoBehaviour
{
    //ドアを開けるために必要なカギのID
    [SerializeField] protected string openKey = "";
    public string DoorOpenKey { get { return openKey; } }

    protected BoxCollider thisCollider = null;

    protected bool isMoving = false;
    protected bool isOpenState = false;//ドアが開いている状態か
    public bool isUnlocked { get { return DataManager.Instance.IsDoorKeyUnlocked(openKey); } }//過去にプレイヤーがドアを開けているか（ItemのisUsedで判定）
    protected const float DistanceRequiredToCloseDoorSqrMagnitude = 100f;//自動でドアが閉まるために必要なプレイヤーとの距離：距離は10。sqrMagnitudeの計算なので 10 * 10 = 100

    // Start is called before the first frame update
    void Start()
    {
        thisCollider = GetComponent<BoxCollider>();
        if(thisCollider == null)
        {
            thisCollider = gameObject.AddComponent<BoxCollider>();
            Debug.Log("ドアのコライダーがありませんでした");
        }
    }

    public void OpenDoor()
    {
        if (!string.IsNullOrEmpty(openKey))
        {
            if (!isUnlocked)
            {
                Debug.Log("鍵がかかっている : " + openKey);
                return;
            }
        }
        
        OpenAction();
    }
    /// <summary>
    /// 実際にドアを開ける
    /// </summary>
    protected abstract void OpenAction();

    public abstract void CloseDoor();
    
    protected IEnumerator DoorCloseWithWateTime()
    {
        //プレイヤーがドアから離れている且つプレイヤーと雪絵が部屋の外にいる時にドアを閉める（雪絵のNavMeshが途切れてしまうため、その対策）
        while (true)
        {
            yield return new WaitForSeconds(3f);
            if ((transform.position - StageManager.Instance.GetPlayer().Position).sqrMagnitude >= DistanceRequiredToCloseDoorSqrMagnitude &&
                 !StageManager.Instance.GetPlayer().inRoomChecker.isEnterRoom && !StageManager.Instance.GetYukie().inRoomChecker.isEnterRoom)
            {
                break;
            }
        }
        CloseDoor();
    }
}
