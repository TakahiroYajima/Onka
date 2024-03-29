﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;
using Onka.Manager.Data;

/// <summary>
/// 施錠用オブジェクト
/// </summary>
[RequireComponent(typeof(Collider))]
public class KeyHoleObject : MonoBehaviour
{
    private KeyHoleTarget keyLockTarget = null;
    private Collider collider = null;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    public void SetInitialize(KeyHoleTarget _keyLockTarget)
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
