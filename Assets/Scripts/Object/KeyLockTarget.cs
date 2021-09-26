using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyLockTarget : MonoBehaviour
{
    [SerializeField] protected string unlockKey = "";
    public string UnlockKey { get { return unlockKey; } }
    [SerializeField] protected KeyLockObject keyLockObject = null;
    public bool isUnlocked { get { return DataManager.Instance.IsKeyUnlocked(unlockKey); } }//過去にプレイヤーがドアを開けているか（ItemのisUsedで判定）

    // Start is called before the first frame update
    void Start()
    {
        keyLockObject.SetInitialize(this);
    }
}
