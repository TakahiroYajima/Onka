using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 部屋内を徘徊するときの通過地点を管理する
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class RoomWanderingManager : MonoBehaviour
{
    [SerializeField] private GameObject exitColliderObj = null;
    public List<WanderingPoint> wanderingPoints { get; private set; } = new List<WanderingPoint>();

    // Start is called before the first frame update
    void Start()
    {
        //徘徊用の通過ポイントを敵種類ごとに保存
        wanderingPoints = GetComponentsInChildren<WanderingPoint>().ToList();
        for(int i = 0; i < wanderingPoints.Count; i++)
        {
            wanderingPoints[i].Initialize(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case Tags.Player:
                StageManager.Instance.Player.inRoomChecker.SetEnterRoom(this);
                break;
            case Tags.Enemy:
                StageManager.Instance.Yukie.inRoomChecker.SetEnterRoom(this);
                break;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case Tags.Player:
                if (StageManager.Instance.Player.inRoomChecker.IsMatchExitCollider(exitColliderObj))
                {
                    StageManager.Instance.Player.inRoomChecker.ExitRoom(this);
                }
                break;
            case Tags.Enemy:
                if (StageManager.Instance.Yukie.inRoomChecker.IsMatchExitCollider(exitColliderObj))
                {
                    StageManager.Instance.Yukie.inRoomChecker.ExitRoom(this);
                }
                break;
        }
    }
}

//public enum Direction8
//{
//    F,//Foward
//    B,//Back
//    R,//RIght
//    L,//Left
//    FR,
//    FL,
//    BR,
//    BL,
//}