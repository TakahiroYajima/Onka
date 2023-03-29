using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Onka.Manager.Data;

public class KeyHoleTarget : MonoBehaviour
{
    [SerializeField] protected string unlockKey = "";
    public string UnlockKey { get { return unlockKey; } }
    [SerializeField] protected KeyHoleObject keyLockObject = null;
    public bool isUnlocked { get { return DataManager.Instance.IsKeyUnlocked(unlockKey); } }//過去にプレイヤーがドアを開けているか（ItemのisUsedで判定）

    public void SetUp()
    {
        keyLockObject.SetInitialize(this);
    }
}
