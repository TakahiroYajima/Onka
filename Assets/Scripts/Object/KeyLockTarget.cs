using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ナンバーロックなど、解く専用の鍵のターゲット
/// KeyHoleTargetと同じように使う
/// </summary>
public class KeyLockTarget : MonoBehaviour
{
    [SerializeField] protected string unlockTargetKey = "";
    public string UnlockTargetKey { get { return unlockTargetKey; } }
    [SerializeField] protected KeyLockObject keyLockObject = null;
    public bool isUnlocked { get { return DataManager.Instance.IsKeyUnlocked(unlockTargetKey); } }//過去にプレイヤーが解錠しているか（ItemのisUsedで判定）

    // Start is called before the first frame update
    void Start()
    {
        keyLockObject.SetInitialize(this);
    }
    /// <summary>
    /// 解錠できた瞬間のイベント。終了の際は必ず下記のUnlocked()を呼ぶこと
    /// </summary>
    public void StartUnlockEvent()
    {
        SolveKeylockManager.Instance.StartUnlockEvent();
    }

    /// <summary>
    /// Unlockを呼ぶことでカギを手に入れた事と同じ扱いにする
    /// </summary>
    public void Unlocked()
    {
        //対象のデータ入手フラグをONにする
        ItemData data = DataManager.Instance.GetItemData(unlockTargetKey);
        if (data != null)
        {
            DataManager.Instance.ItemGetAction(data);
            DataManager.Instance.SetUsedItem(data);
        }
        else
        {
            Debug.LogError("取得したアイテムは存在しないか、キーが間違っています。 " + unlockTargetKey);
        }
        SolveKeylockManager.Instance.FinishUnlockEvent();
        Onka.Manager.Event.EventManager.Instance.ProgressEvent();
    }
}
