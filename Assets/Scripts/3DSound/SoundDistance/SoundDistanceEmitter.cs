using System;
using UnityEngine;

namespace SoundDistance
{
    /// <summary>
    /// SoundDistance機能で音を発する元となるオブジェクト
    /// </summary>
    public class SoundDistanceEmitter : MonoBehaviour
    {
        public int currentPointID { get; private set; } = -1;
        public void SetPointID(int id) {
            currentPointID = id;
            SetOuterPointID(id);
        }
        //脇道や部屋に入った時用に、最後に通過した外周のIDを持っておく
        public int currentOuterPointID { get; private set; } = -1;
        public int prevHitedOuterPointID { get; private set; } = -1;//currentOuterPointIDの直後に更新される
        public void SetOuterPointID(int id)
        {
            if (SoundDistanceManager.Instance.soundDistancePoints[id].IsOuter)
            {
                currentOuterPointID = id;
            }
            OnEnterOuterPoint?.Invoke(currentOuterPointID);
            prevHitedOuterPointID = currentOuterPointID;
        }
        //次に通過すると見なされるSoundDistancePointのインスタンスID
        public int nextTargetPointID { get; private set; } = -1;
        public void SetNextTargetPointID(int id) { nextTargetPointID = id; }

        public Action<int> OnEnterOuterPoint = null;
    }
}