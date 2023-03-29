using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SoundSystem;
using Onka.Manager.Data;

/// <summary>
/// 音を鳴らせるオブジェクト
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundPlayerObject : MonoBehaviour
{
    [SerializeField] private List<AudioClip> audioClipList = new List<AudioClip>();
    public AudioSource audioSource { get; private set; } = null;
    //private List<SoundData> soundDataList = new List<SoundData>();

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    public AudioClip GetClip(int arrayNum)
    {
        if (arrayNum < 0 || arrayNum >= audioClipList.Count) return null;
        return audioClipList[arrayNum];
    }
    public List<AudioClip> GetClipList()
    {
        return audioClipList;
    }

    public void PlaySoundLoop(int arrayNum, float volume = 1f)
    {
        if (arrayNum >= 0 && arrayNum < audioClipList.Count)
        {
            AudioClip clip = audioClipList[arrayNum];
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.volume = volume;
            audioSource.Play();
        }
    }
    public void PlaySoundLoop(string key)
    {
        SoundData data = DataManager.Instance.GetSE(key);
        if (data != null)
        {
            AudioClip clip = SoundManager.Instance.menuSeAudioClipList.FirstOrDefault(x => x.name == data.soundName);
            if (clip != null)
            {
                PlayLoop(data, clip);
            }
            else { Debug.Log("clipがありません : " + key); }
        }
        else { Debug.Log("SoundDataがありません : " + key); }
    }
    public void PlayVoiceLoop(string key)
    {
        SoundData data = DataManager.Instance.GetVoice(key);
        if (data != null)
        {
            AudioClip clip = SoundManager.Instance.voiceAudioClipList.FirstOrDefault(x => x.name == data.soundName);
            if (clip != null)
            {
                PlayLoop(data, clip);
            }
            else { Debug.Log("clipがありません : " + key); }
        }
        else { Debug.Log("SoundDataがありません : " + key); }
    }

    private void PlayLoop(SoundData data, AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.volume = data.volume;
        audioSource.spatialBlend = data.spatialBlend;
        audioSource.Play();
    }

    public void VolumeUpWithFade(float fadeTime = 1f, float endVolume = 1f)
    {
        StartCoroutine(audioSource.VolumeUpWithFade(fadeTime, endVolume));
    }

    public void StopSound()
    {
        audioSource.Stop();
    }

    public void StopSoundWithFadeOut(float fadeTime = 1f)
    {
        StartCoroutine(audioSource.StopWithFadeOut(fadeTime));
    }
    
    public void PlaySE(string key)
    {
        SoundData data = DataManager.Instance.GetSE(key);
        if (data != null)
        {
            AudioClip clip = audioClipList.FirstOrDefault(x => x.name == data.soundName);
            if (clip != null)
            {
                audioSource.volume = data.volume;
                audioSource.spatialBlend = data.spatialBlend;
                audioSource.PlayOneShot(clip);
            }
            else
            {
                Debug.Log("SEがありません : " + key);
            }
        }
    }

    public void PlaySE(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            audioSource.volume = volume;
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlaySE(int arrayNum)
    {
        if(arrayNum >= 0 && arrayNum < audioClipList.Count)
        {
            audioSource.PlayOneShot(audioClipList[arrayNum]);
        }
    }

    public void SetVolume(float _volume)
    {
        audioSource.volume = _volume;
    }
}
