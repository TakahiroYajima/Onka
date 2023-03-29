using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// KeyLockで、プレイヤーの操作によって動くもの（南京錠のナンバーなど）
/// </summary>
public abstract class KeyLockOperationObject : MonoBehaviour
{
    public abstract void Tap();
}
