using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 音を鳴らせるオブジェクト
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class SoundPlayerObject : MonoBehaviour
{
    [SerializeField] private List<AudioClip> audioClipList = new List<AudioClip>();
    public AudioSource audioSource { get; private set; } = null;
    private List<SoundData> soundDataList = new List<SoundData>();

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
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
    public void PlaySoundLoop(string key, float volume = 1f)
    {
        SoundData data = DataManager.Instance.GetSE(key);
        if (data != null)
        {
            AudioClip clip = audioClipList.FirstOrDefault(x => x.name == data.soundName);
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.loop = true;
                audioSource.volume = volume;
                audioSource.Play();
            }
        }
    }

    public void StopSound()
    {
        audioSource.Stop();
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
