using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SwingableObject))]
public class SwingableShelfObject : StageObjectBase
{
    [SerializeField] private SwingableObject door = null;

    public override void OnTapObject()
    {
        //throw new System.NotImplementedException();
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
