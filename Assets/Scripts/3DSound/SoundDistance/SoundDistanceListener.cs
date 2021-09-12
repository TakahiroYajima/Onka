using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundDistance
{
    public class SoundDistanceListener : MonoBehaviour
    {
        //現在通過している(最後に通過した)SoundDistancePointのインスタンスID
        public int currentPointID { get; private set; } = -1;
        public void SetCurrentPointID(int id) {
            currentPointID = id;
            if (SoundDistanceManager.Instance.soundDistancePoints[currentPointID].IsOuter)
            {
                currentOuterPointID = currentPointID;
            }
        }
        //脇道や部屋に入った時用に、最後に通過した外周のIDを持っておく
        public int currentOuterPointID { get; private set; } = -1;
        //次に通過すると見なされるSoundDistancePointのインスタンスID
        public int nextTargetPointID { get; private set; } = -1;
        public void SetNextTargetPointID(int id) { nextTargetPointID = id; }
        //1つ前に通過したSoundDistancePointのインスタンスID
        public int prevPointID { get; private set; } = -1;
        public void SetPrevPointID(int id) { prevPointID = id; }
        //currentPointIDのポイントから、Emitterの方向にある隣のSoundDistancePointのインスタンスID
        public int emitDirectionPointID { get; private set; } = -1;
        public void SetEmitDirectionPointID(int id) { emitDirectionPointID = id; }

        [HideInInspector] public SoundDistancePoint currentHittingPoint = null;//衝突中の当たり判定（衝突していなければnull）

        
    }
}