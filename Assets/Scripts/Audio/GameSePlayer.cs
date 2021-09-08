using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoundSystem
{
    [RequireComponent(typeof(AudioSource))]
    public class GameSePlayer : MonoBehaviour
    {
        public AudioSource audioSource;
        public List<AudioClip> audioClipList = new List<AudioClip>();
        
        public bool IsPaused { get; private set;}

        private void Awake()
        {
            IsPaused = false;
        }

        public void PlayFirstAudioClip()
        {
            if (IsPaused) return;
            
            if (audioClipList.Count > 0)
            {
                audioSource.Play(audioClipList[0]);              
            }
        }

        //ゲームSEの再生
        public void PlaySe(string audioClipName)
        {
            if (IsPaused) return;
            
            AudioClip audioClip = audioClipList.FirstOrDefault(clip => clip.name == audioClipName);

            if (audioClip != null)
            {
                audioSource.pitch = 1f;
                audioSource.Play(audioClip);
            }
        }
        
        public void PlaySePitchRandomize(string audioClipName, float range = 0.5f)
        {
            if (IsPaused) return;
            
            AudioClip audioClip = audioClipList.FirstOrDefault(clip => clip.name == audioClipName);

            if (audioClip != null)
            {
                audioSource.pitch = Random.Range(1f - range, 1f + range);
                audioSource.Play(audioClip);
            }
        }

        public void Pause()
        {
            IsPaused = true;
            audioSource.Pause();
        }

        public void Resume()
        {
            IsPaused = false;
            audioSource.UnPause();
        }
        
        //コンポーネントがアタッチされたときの初期設定
        public void Reset()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0.7f;
        }
    }
}