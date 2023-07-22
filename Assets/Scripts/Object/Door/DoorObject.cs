using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(BoxCollider))]
public abstract class DoorObject : MonoBehaviour
{
    //ドアを開けるために必要なカギのID
    [SerializeField] protected string openKey = "";
    //public string DoorOpenKey { get { return openKey; } }
    [SerializeField] protected KeyHoleTarget keyHoleTarget = null;
    [SerializeField] protected KeyLockTarget keyLockTarget = null;

    protected BoxCollider thisCollider = null;

    protected bool isMoving = false;
    public bool isOpenState { get; protected set; } = false;//ドアが開いている状態か
    [System.NonSerialized] public bool isForceOpenable = false;//イベントなどで強制的にドアを開けられるようにするか
    [HideInInspector] public bool isEternalClosed = false;//一切開けられないようにするか（イベント用）

    protected string seKey = "se_door_open";

    //過去にプレイヤーがドアを開けているか（ItemのisUsedで判定）
    public bool isUnlocked
    {
        get {
            if (keyHoleTarget == null && keyLockTarget == null) return true;
            else if (keyHoleTarget != null && keyLockTarget == null) return keyHoleTarget.isUnlocked;
            else if (keyLockTarget != null && keyHoleTarget == null) return keyLockTarget.isUnlocked;
            else return keyHoleTarget.isUnlocked && keyLockTarget.isUnlocked;
        }
    }
    protected const float DistanceRequiredToCloseDoorSqrMagnitude = 100f;//自動でドアが閉まるために必要なプレイヤーとの距離：距離は10。sqrMagnitudeの計算なので 10 * 10 = 100

    // Start is called before the first frame update
    protected virtual void Start()
    {
        thisCollider = GetComponent<BoxCollider>();
        if(thisCollider == null)
        {
            thisCollider = gameObject.AddComponent<BoxCollider>();
            Debug.Log("ドアのコライダーがありませんでした");
        }
        keyHoleTarget.SetDoor(this);
    }

    public bool IsOpenableDoor()
    {
        if (isEternalClosed) return false;
        if (isForceOpenable) return true;

        bool isKeyHoleUnlocked = true;
        bool isKeyLockUnlocked = true;
        if (keyHoleTarget != null)
        {
            if (string.IsNullOrEmpty(keyHoleTarget.UnlockKey)) { Debug.LogError("解錠キーが設定されていません"); return false; }
            if (!keyHoleTarget.isUnlocked)
            {
                //Debug.Log("鍵がかかっている : " + openKey);
                isKeyHoleUnlocked = false;
            }
            else
            {
                //Debug.Log("解錠済み : " + openKey);
            }
        }
        if (keyLockTarget != null)
        {
            if (string.IsNullOrEmpty(keyLockTarget.UnlockTargetKey)) { Debug.LogError("解錠キーが設定されていません"); return false; }
            if (!keyLockTarget.isUnlocked)
            {
                //Debug.Log("鍵がかかっている : " + openKey);
                isKeyLockUnlocked = false;
            }
        }
        return isKeyHoleUnlocked && isKeyLockUnlocked;
    }

    public void OpenDoor()
    {
        if (IsOpenableDoor() && !isMoving && !isOpenState)
        {
            OpenAction();
            SoundManager.Instance.PlaySeWithKeyOne(seKey);
        }
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
            if (!isOpenState) break;

            yield return new WaitForSeconds(3f);
            if ((transform.position - StageManager.Instance.Player.Position).sqrMagnitude >= DistanceRequiredToCloseDoorSqrMagnitude &&
                 !StageManager.Instance.Player.inRoomChecker.isEnterRoom && !StageManager.Instance.Yukie.inRoomChecker.isEnterRoom &&
                 isOpenState)
            {
                break;
            }
        }
        //強制でドアを閉める処理対策のため、一旦確認を入れる
        if (isOpenState)
        {
            CloseDoor();
        }
    }
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(DoorObject))]
//public class DoorObjectInspector : Editor
//{
//    public bool isUseKeyLock = false;

//    public override void OnInspectorGUI()
//    {
//        DoorObject door = target as DoorObject;

//        EditorGUILayout.BeginHorizontal();
//        {
//            isUseKeyLock = EditorGUILayout.Toggle("鍵システムを使用するか", isUseKeyLock, GUILayout.Width(120));
//        }
//        EditorGUILayout.EndHorizontal();
//        EditorGUILayout.LabelField("鍵システムを使用するか");

//    }
//}
//#endif