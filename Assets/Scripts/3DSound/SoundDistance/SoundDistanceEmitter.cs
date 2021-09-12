using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundDistance
{
    /// <summary>
    /// SoundDistance機能で音を発する元となるオブジェクト
    /// </summary>
    public class SoundDistanceEmitter : MonoBehaviour
    {
        public AudioClip audioClip { get; private set; } = null;
        public int currentPointID { get; private set; } = -1;
        public void SetPointID(int id) {
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

        public void SetEmitterAudioClip(AudioClip clip)
        {
            audioClip = clip;
        }
    }
}