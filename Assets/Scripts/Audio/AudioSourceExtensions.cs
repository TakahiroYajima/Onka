using System.Collections;
using UnityEngine;

namespace SoundSystem
{
    public static class AudioSourceExtensions
    {
        public static void Play(this AudioSource audioSource, AudioClip audioClip = null, float volume = 1f)
        {
            if (audioClip != null)
            {
                audioSource.clip = audioClip;
            
                //ボリュームが適切な値になるように調整//
                audioSource.volume = Mathf.Clamp01( volume );
            
                audioSource.Play();
            }
        } 
        
        public static IEnumerator PlayRandomStart(this AudioSource audioSource, AudioClip audioClip, float volume = 1f)
        {
            if (audioClip == null) yield break;

            audioSource.clip = audioClip;
            audioSource.volume = Mathf.Clamp01(volume);

            //結果がlengthと同値になるとシークエラーを起こすため -0.01秒する//
            audioSource.time = UnityEngine.Random.Range(0f, audioClip.length - 0.01f);

            yield return PlayWithFadeIn(audioSource, audioClip, 1f, volume);
        }
        
        
        public static IEnumerator PlayWithFadeIn(this AudioSource audioSource, AudioClip audioClip, float fadeTime = 0.1f, float endVolume = 1.0f ) 
        {
            //目標ボリュームを0から1に補正//
            float targetVolume = Mathf.Clamp01(endVolume);
            //フェード時間がおかしかった場合は補正//
            fadeTime = fadeTime < 0.1f ? 0.1f : fadeTime;
        
            //音量0で再生開始//
            audioSource.Play(audioClip, 0f);
        
            for (float t = 0f; t < fadeTime; t+= Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(0f, targetVolume, Mathf.Clamp01( t / fadeTime) );
                yield return null; 
            }
            audioSource.volume = targetVolume;
        }
        
        public static IEnumerator VolumeUpWithFade(this AudioSource audioSource, float fadeTime = 0.1f, float endVolume = 1.0f)
        {
            //目標ボリュームを補正//
            float targetVolume = Mathf.Clamp01(endVolume);
            //フェード時間がおかしかった場合は補正//
            fadeTime = fadeTime < 0.1f ? 0.1f : fadeTime;
            float startVolume = audioSource.volume;
            for (float t = 0f; t < fadeTime; t += Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp(startVolume, targetVolume, Mathf.Clamp01(t / fadeTime));
                yield return null;
            }
            audioSource.volume = targetVolume;
        }
        
        public static IEnumerator StopWithFadeOut(this AudioSource audioSource, float fadeTime = 0.1f)
        {
            float startVolume = audioSource.volume;
        
            //フェード時間がおかしかった場合は補正//
            fadeTime = fadeTime < 0.1f ? 0.1f : fadeTime;
            
            for (float t = 0f; t < fadeTime; t+= Time.deltaTime)
            {
                audioSource.volume = Mathf.Lerp( startVolume, 0f, Mathf.Clamp01( t / fadeTime) );
                yield return null; 
            }
            audioSource.volume = 0f;
            
            audioSource.Stop();
            audioSource.clip = null;
        }
    }
}