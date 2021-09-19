using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スライド・引き出し状の棚・収納の開閉を管理する
/// </summary>
[RequireComponent(typeof(SlidableObject))]
public class SlidableShelfObject : StageObjectBase
{
    [SerializeField] private SlidableObject door = null;

    public override void OnTapObject()
    {
        //throw new System.NotImplementedException();
        if (!door.isMoving)
        {
            if (door.isOpenState)
            {
                thisCollider.enabled = true;
            }
            door.Slide();
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
