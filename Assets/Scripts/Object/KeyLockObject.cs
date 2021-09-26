using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;

/// <summary>
/// 施錠用オブジェクト
/// </summary>
[RequireComponent(typeof(Collider))]
public class KeyLockObject : MonoBehaviour
{
    private KeyLockTarget keyLockTarget = null;
    private Collider collider = null;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    public void SetInitialize(KeyLockTarget _keyLockTarget)
    {
        keyLockTarget = _keyLockTarget;
        if (DataManager.Instance.IsKeyUnlocked(keyLockTarget.UnlockKey))
        {
            collider.enabled = false;
        }
    }

    public void DoUnlock()
    {
        DataManager.Instance.DoDoorUnlock(keyLockTarget.UnlockKey, () =>
        {
            collider.enabled = false;
            SoundManager.Instance.PlaySeWithKey("menuse_key_unlock");
        });
    }
}
