using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ドア状の棚・収納の開閉を管理する
/// </summary>
[RequireComponent(typeof(SwingableObject))]
public class SwingableShelfObject : StageObjectBase
{
    [SerializeField] private SwingableObject door = null;
    private KeyHoleTarget keyHoleTarget = null;
    private KeyLockTarget keyLockTarget = null;

    private void Awake()
    {
        keyHoleTarget = GetComponent<KeyHoleTarget>();
        keyLockTarget = GetComponent<KeyLockTarget>();
    }

    public override void OnTapObject()
    {
        //throw new System.NotImplementedException();
        if(keyHoleTarget != null)
        {
            if (!keyHoleTarget.isUnlocked) return;
        }
        if(keyLockTarget != null)
        {
            if(!keyLockTarget.isUnlocked) return;
        }
        if (!door.isMoving)
        {
            if (door.isOpenState)
            {
                thisCollider.enabled = true;
            }
            door.Swing();
        }
    }

    /// <summary>
    /// 必要ならコライダーのON/OFFの切り替え設定をエディタにて行う
    /// </summary>
    public void EndSlide()
    {
        thisCollider.enabled = !door.isOpenState;
    }
}
