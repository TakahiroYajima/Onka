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
        List<WanderingPoint> list = GetComponentsInChildren<WanderingPoint>().ToList();
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case Tags.Player:
                StageManager.Instance.GetPlayer().inRoomChecker.SetEnterRoom(this);
                break;
            case Tags.Enemy:
                StageManager.Instance.GetYukie().inRoomChecker.SetEnterRoom(this);
                break;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case Tags.Player:
                if (StageManager.Instance.GetPlayer().inRoomChecker.IsMatchExitCollider(exitColliderObj))
                {
                    StageManager.Instance.GetPlayer().inRoomChecker.ExitRoom(this);
                }
                break;
            case Tags.Enemy:
                if (StageManager.Instance.GetYukie().inRoomChecker.IsMatchExitCollider(exitColliderObj))
                {
                    StageManager.Instance.GetYukie().inRoomChecker.ExitRoom(this);
                }
                break;
        }
    }
}

public enum Direction8
{
    F,//Foward
    B,//Back
    R,//RIght
    L,//Left
    FR,
    FL,
    BR,
    BL,
}