using System.Linq;
using UnityEngine;
using SoundDistance;

/// <summary>
/// 雪絵の徘徊中、ドアが開いている部屋の中を覗くか判断する
/// </summary>
public class LookInRoomJudgeManager : SingletonMonoBehaviour<LookInRoomJudgeManager>
{
    [System.Serializable]
    public class RoomPointSoundDistancePointData
    {
        public SoundDistancePoint soundDistancePoint;
        public DoorObject[] doors;
        public AlwaysOpenRookPoint[] alwaysOpen;
    }

    [SerializeField] private RoomPointSoundDistancePointData[] roomPointSoundDistancePointDatas;

    public (bool isExist, RoomPointSoundDistancePointData data) GetRoomPointData(int soundDistancePointID)
    {
        var data = (isExist : true, data : roomPointSoundDistancePointDatas.FirstOrDefault(v => v.soundDistancePoint.ID == soundDistancePointID));
        data.isExist = data.data != null;
        return data;
    }

    public bool IsNeedLook(RoomPointSoundDistancePointData data)
    {
        if(data.alwaysOpen != null)
        {
            foreach(var v in data.alwaysOpen)
            {
                if (IsNeedLookAlwaysOpenRookPoint(v))
                {
                    return true;
                }
            }
        }
        if(data.doors != null)
        {
            foreach(var v in data.doors)
            {
                if (v.isOpenState)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsNeedLookAlwaysOpenRookPoint(AlwaysOpenRookPoint point)
    {
        switch (point.pointType)
        {
            case AlwaysOpenRookPointType.InRoom:
                return StageManager.Instance.Player.inRoomChecker.CurrentEnterRoomList.Contains(point.targetRoom);
            case AlwaysOpenRookPointType.OnThe2F:
                return StageManager.Instance.Player.Position.y > 3f;
            default: return false;
        }
    }
}
