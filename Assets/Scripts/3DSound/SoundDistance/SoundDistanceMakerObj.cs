using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundDistance
{
    /// <summary>
    /// SoundDistanceListenerから音が聞こえる方向に配置し、音を発するオブジェクト
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundDistanceMakerObj : MonoBehaviour
    {
        private AudioSource audioSource = null;
        [System.NonSerialized] public float maxVolume = 0.8f;
        public bool isActionable { get; private set; } = false;
        public void DoAction()
        {
            isActionable = true;
        }
        public void StopAction()
        {
            isActionable = false;
            SoundStop();
        }
        
        //音が聞こえている方向にあるPointから一定距離進んだ位置情報
        public Vector3 currentTargetPosition { get; private set; } = Vector3.zero;
        public void SetTargetPosition(Vector3 _target)
        {
            currentTargetPosition = _target;
        }
        
        private Vector3 prevListenerPosition = Vector3.zero;//前フレームでの自分の位置を保持
        private Vector3 targetDirection = Vector3.zero;//現在の位置から本来いるべき位置までの距離
        private const float moveJudgeDirectionSqrMagnitude = 0.07f;
        private float targetMoveSpeed = 3.5f;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = 0f;
        }

        void LateUpdate()
        {
            if (!isActionable) return;
            //Debug.Log("SDM " + audioSource.isPlaying + " volume : " + audioSource.volume);
            UpdatePosition();
            UpdateAudioMaker();
        }

        /// <summary>
        /// Position更新
        /// </summary>
        private void UpdatePosition()
        {
            Vector3 direction = (currentTargetPosition - SoundDistanceManager.Instance.Listener.transform.position).normalized;
            Vector3 moveTarget = SoundDistanceManager.Instance.Listener.transform.position + direction;

            if (prevListenerPosition == Vector3.zero) { prevListenerPosition = SoundDistanceManager.Instance.Listener.transform.position; transform.position = moveTarget; }
            //一旦、Listenerが進んだ分進ませる
            Vector3 listenerProgress = SoundDistanceManager.Instance.Listener.transform.position - prevListenerPosition;
            transform.position += listenerProgress;
            //突然離れたところがTargetになるといきなり音が鳴るところが飛んで聞こえが悪くなるため、徐々に向かうようにする
            targetDirection = moveTarget - transform.position;
            if(targetDirection.sqrMagnitude >= moveJudgeDirectionSqrMagnitude)
            {
                transform.position += targetDirection * (Time.deltaTime * targetMoveSpeed);
            }
            else
            {
                transform.position = moveTarget;
            }
            prevListenerPosition = SoundDistanceManager.Instance.Listener.transform.position;
        }

        /// <summary>
        /// 聞こえる音量更新
        /// </summary>
        private void UpdateAudioMaker()
        {
            //ratio : 1~0, volume : 0~1、お互いに0と1が真逆の関係
            float ratio = Mathf.Clamp01((SoundDistanceManager.Instance.currentDistanceListenerToEmitter / SoundDistanceManager.Instance.CanNotHearRatio) / SoundDistanceManager.Instance.OuterCircumference);
            ratio = 1 - ratio;//0~1に直す
            float fx01 = ratio * ratio;//f(x)=x^2
            float volume = 0f;
            //近ければ最大にする
            if (ratio >= 0.97f) { volume = maxVolume; }
            else { volume = Mathf.Lerp(audioSource.volume, fx01 * maxVolume, Time.deltaTime * 3); /*Debug.Log($"vol : {audioSource.volume.ToString("f4")} :t: {volume.ToString("f4")}");*/ }//徐々に変化
            audioSource.volume = volume;
            //Debug.Log($"vol : {volume.ToString("f2")} ratio : {ratio.ToString("f2")} : {fx01.ToString("f2")} : {SoundDistanceManager.Instance.currentDistanceListenerToEmitter.ToString("f2")} / {SoundDistanceManager.Instance.CanNotHearRatio.ToString("f2")} / {SoundDistanceManager.Instance.OuterCircumference.ToString("f2")}");
        }

        public void SetClipAndPlay(AudioClip clip, float currentTime = 0f)
        {
            if (clip == null) return;
            if (audioSource.clip == clip) return;
            audioSource.clip = clip;
            audioSource.time = currentTime;
            audioSource.Play();
        }
        public void ClearClipAndStop()
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
        //public void SoundPlay()
        //{
        //    if (audioSource.clip != null)
        //    {
        //        audioSource.Play();
        //    }
        //}
        public void SoundStop()
        {
            audioSource.Stop();
        }
        public void SoundPause()
        {
            audioSource.Pause();
        }
        public void SoundUnPause()
        {
            audioSource.UnPause();
        }

        public void SetVolume(float volume)
        {
            audioSource.volume = volume;
        }
    }
}