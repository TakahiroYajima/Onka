using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 部屋の出口に設置するコライダー
/// 今いる部屋から、廊下へ続く・あるいは出口の方向にある部屋に出たかの判定に使う
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class RoomExitCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case Tags.Player:
                StageManager.Instance.Player.inRoomChecker.SetEnterExitColliderObj(gameObject);
                break;
            case Tags.Enemy:
                StageManager.Instance.Yukie.inRoomChecker.SetEnterExitColliderObj(gameObject);
                break;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case Tags.Player:
                StageManager.Instance.Player.inRoomChecker.ExitEnterExitCollider();
                break;
            case Tags.Enemy:
                StageManager.Instance.Yukie.inRoomChecker.ExitEnterExitCollider();
                break;
        }
    }
}
