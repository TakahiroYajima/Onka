using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using Onka.Manager.Data;

namespace SoundSystem
{
    public class SoundManager : SingletonMonoBehaviour<SoundManager>
    {        
        public List<AudioClip> menuSeAudioClipList = new List<AudioClip>();
        private AudioSource menuSeAudioSource;
        
        public List<AudioClip> environmentAudioClipList = new List<AudioClip>();
        private AudioSource environmentAudioSource;

        public List<AudioClip> voiceAudioClipList = new List<AudioClip>();
        private AudioSource voiceAudioSource;
        
        public List<AudioClip> bgmAudioClipList = new List<AudioClip>();
        private List<AudioSource> bgmAudioSourceList = new List<AudioSource>();
        private const int BGMAudiosourceNum = 2;

        private List<IEnumerator> fadeCoroutines = new List<IEnumerator>();

        //[SerializeField, HeaderAttribute("Audio Mixer")]
        //public AudioMixer audioMixer;
        public AudioMixerGroup bgmAMG, menuSeAMG, envAMG, voiceAMG;

        //public AudioMixer effectAudioMixer;

        public bool IsPaused { get; private set; }

        private const string MasterVolumeParamName = "MasterVolume";
        private const string GameSeVolumeParamName = "GameSEVolume";
        private const string BGMVolumeParamName = "BGMVolume";
        private const string EnvVolumeParamName = "EnvironmentVolume";
        
        protected override void Awake()
        {
            base.Awake();
            menuSeAudioSource = InitializeAudioSource(this.gameObject, false, menuSeAMG);
            bgmAudioSourceList = InitializeAudioSources(this.gameObject, true, bgmAMG, BGMAudiosourceNum);
            environmentAudioSource = InitializeAudioSource(this.gameObject, true, envAMG);

            voiceAudioSource = InitializeAudioSource(this.gameObject, false, voiceAMG);

            IsPaused = false;
        }

        //public float MasterVolume
        //{
        //    get { return audioMixer.GetVolumeByLinear(MasterVolumeParamName); }
        //    set { audioMixer.SetVolumeByLinear(MasterVolumeParamName, value); }
        //}
        
        //public float GameSeVolume
        //{
        //    get { return audioMixer.GetVolumeByLinear(GameSeVolumeParamName); }
        //    set { audioMixer.SetVolumeByLinear(GameSeVolumeParamName, value); }
        //}
        
        //public float BGMVolume
        //{
        //    get { return audioMixer.GetVolumeByLinear(BGMVolumeParamName); }
        //    set { audioMixer.SetVolumeByLinear(BGMVolumeParamName, value); }
        //}
        
        //public float EnvironmentVolume
        //{
        //    get { return audioMixer.GetVolumeByLinear(EnvVolumeParamName); }
        //    set { audioMixer.SetVolumeByLinear(EnvVolumeParamName, value); }
        //}

        //public void ChangeSnapshot(string snapshotName, float transitionTime = 1f)
        //{
        //    AudioMixerSnapshot snapshot = effectAudioMixer.FindSnapshot(snapshotName);
        
        //    if (snapshot == null)
        //    {
        //        Debug.Log(snapshotName + "は見つかりません");
        //    }
        //    else
        //    {
        //        snapshot.TransitionTo(transitionTime);
        //    }
        //}
        
        public void Pause()
        {
            IsPaused = true;

            fadeCoroutines.ForEach(StopCoroutine);
            
            environmentAudioSource.Pause();
            voiceAudioSource.Pause();
            bgmAudioSourceList.ForEach(bas => bas.Pause());
        }

        public void Resume()
        {
            IsPaused = false;

            fadeCoroutines.ForEach(routine =>StartCoroutine(routine));
            
            environmentAudioSource.UnPause();
            voiceAudioSource.UnPause();
            bgmAudioSourceList.ForEach(bas => bas.UnPause());          
        }

        private List<AudioSource> InitializeAudioSources(GameObject parentGameObject, bool isLoop = false,
            AudioMixerGroup amg = null, int count = 1)
        {
            List<AudioSource> audioSources = new List<AudioSource>();

            for (int i = 0; i < count; i++)
            {
                var audioSource = InitializeAudioSource(parentGameObject, isLoop, amg);
                audioSources.Add(audioSource);
            }

            return audioSources;
        }

        private AudioSource InitializeAudioSource(GameObject parentGameObject, bool isLoop = false, AudioMixerGroup amg = null)
        {
            var audioSource = parentGameObject.AddComponent<AudioSource>();
            
            audioSource.loop = isLoop;
            audioSource.playOnAwake = false;
        
            if (amg != null)
            {
                audioSource.outputAudioMixerGroup = amg;
            }
        
            return audioSource;
        }

        public void InitSeLoad()
        {
            menuSeAudioClipList.Clear();
            List<SoundData> seList = DataManager.Instance.GetSoundSO().menuSeList;
            foreach (var se in seList)
            {
                //Debug.Log("SE追加 : " + se.key + " : " + se.soundName);
                menuSeAudioClipList.Add(Resources.Load("Sounds/SE/" + se.soundName) as AudioClip);
            }
        }

        public void PlaySe(string clipName)
        {
            var audioClip = menuSeAudioClipList.FirstOrDefault(clip => clip.name == clipName);
            
            if (audioClip == null)
            {
                Debug.Log(clipName + "は見つかりません");
                return;
            }
            menuSeAudioSource.Play(audioClip);
        }
        public void PlaySe(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.Log(clip + "がありません");
                return;
            }
            menuSeAudioSource.Play(clip);
        }
        public void PlaySeOne(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.Log(clip + "がありません");
                return;
            }
            menuSeAudioSource.PlayOneShot(clip);
        }
        public void PlaySeWithKey(string key)
        {
            var audioClip = menuSeAudioClipList.FirstOrDefault(clip => clip.name == DataManager.Instance.GetMenuSE(key).soundName);

            if (audioClip == null)
            {
                Debug.Log(key + "は見つかりません");
                return;
            }
            float volume = DataManager.Instance.GetMenuSE(key).volume;
            menuSeAudioSource.Play(audioClip, volume);
        }
        public void PlaySeWithKeyOne(string key)
        {
            var audioClip = menuSeAudioClipList.FirstOrDefault(clip => clip.name == DataManager.Instance.GetMenuSE(key).soundName);

            if (audioClip == null)
            {
                Debug.Log(key + "は見つかりません");
                return;
            }
            float volume = DataManager.Instance.GetMenuSE(key).volume;
            menuSeAudioSource.PlayOneShot(audioClip, volume);
        }

        public void StopSE()
        {
            menuSeAudioSource.Stop();
        }

        public void PlayEnvironment(string clipName)
        {
            if (IsPaused) return;
            
            var audioClip = environmentAudioClipList.FirstOrDefault(clip => clip.name == clipName);
            
            if (audioClip == null)
            {
                Debug.Log(clipName + "は見つかりません");
                return;
            }
            
            StartCoroutine(environmentAudioSource.PlayRandomStart(audioClip));
        }
        public void PlayEnvironmentWithKey(string key, bool isRandomStart = true)
        {
            if (IsPaused) return;

            SoundData data = DataManager.Instance.GetAmbient(key);
            if (data == null) { return; }

            var audioClip = environmentAudioClipList.FirstOrDefault(clip => clip.name == data.soundName);

            if (audioClip == null)
            {
                Debug.Log(data.soundName + "は見つかりません");
                return;
            }
            if (environmentAudioSource.isPlaying) { environmentAudioSource.Stop(); }
            if (isRandomStart)
            {
                StartCoroutine(environmentAudioSource.PlayRandomStart(audioClip, data.volume));
            }
            else
            {
                //結果がlengthと同値になるとシークエラーを起こすため -0.01秒する//
                environmentAudioSource.time = 0f;
                environmentAudioSource.Play(audioClip, data.volume);
            }
        }

        public void StopEnvironment()
        {
            if (IsPaused) return;
            
            if (environmentAudioSource.isPlaying)
            {
                environmentAudioSource.Stop();
            }
        }

        public void PlayBGMWithKey(string key)
        {
            if (IsPaused) return;
            var soundData = DataManager.Instance.GetBGM(key);
            var audioClip = bgmAudioClipList.FirstOrDefault(clip => clip.name == soundData.soundName);
            if (audioClip == null)
            {
                Debug.Log(key + "は見つかりません");
                return;
            }

            if (bgmAudioSourceList.Any(source => source.clip == audioClip))
            {
                Debug.Log(key + "はすでに再生されています");
                return;
            }

            StopBGMWithFadeOut(0f);//現在再生中のBGMをフェードアウトする//

            AudioSource audioSource = bgmAudioSourceList.FirstOrDefault(asb => asb.isPlaying == false);

            if (audioSource != null)
            {
                audioSource.Play(audioClip, soundData.volume);
            }
            else
            {
                Debug.LogWarning("audiosource is null");
            }
        }

        public void PlayBGMWithFadeIn(string clipName, float fadeTime = 2f)
        {
            if (IsPaused) return;
            
            var audioClip = bgmAudioClipList.FirstOrDefault(clip => clip.name == clipName);
            
            if (audioClip == null)
            {
                Debug.Log(clipName + "は見つかりません");
                return;
            }

            PlayBGMWithFadeIn(audioClip, fadeTime);
        }

        public void PlayBGMWithFadeIn(AudioClip clip, float fadeTime = 2f, float volume = 1f)
        {
            if (IsPaused || clip == null) return;

            if (bgmAudioSourceList.Any(source => source.clip == clip))
            {
                Debug.Log(clip.name + "はすでに再生されています");
                return;
            }

            StopBGMWithFadeOut(fadeTime);//現在再生中のBGMをフェードアウトする//

            AudioSource audioSource = bgmAudioSourceList.FirstOrDefault(asb => asb.isPlaying == false);

            if (audioSource != null)
            {
                IEnumerator routine = audioSource.PlayWithFadeIn(clip, fadeTime, volume);
                fadeCoroutines.Add(routine);
                StartCoroutine(routine);
            }
        }

        public void PlayBGMWithKeyAndFadeIn(string key, float fadeTime = 2f)
        {
            if (IsPaused) return;

            var soundData = DataManager.Instance.GetBGM(key);
            var audioClip = bgmAudioClipList.FirstOrDefault(clip => clip.name == soundData.soundName);

            if (audioClip == null)
            {
                Debug.Log(key + "は見つかりません");
                return;
            }
            PlayBGMWithFadeIn(audioClip, fadeTime, soundData.volume);
            //if (bgmAudioSourceList.Any(source => source.clip == audioClip))
            //{
            //    Debug.Log(key + "はすでに再生されています");
            //    return;
            //}

            //StopBGMWithFadeOut(fadeTime);//現在再生中のBGMをフェードアウトする//

            //AudioSource audioSource = bgmAudioSourceList.FirstOrDefault(asb => asb.isPlaying == false);

            //if (audioSource != null)
            //{
            //    IEnumerator routine = audioSource.PlayWithFadeIn(audioClip, fadeTime, soundData.volume);
            //    fadeCoroutines.Add(routine);
            //    StartCoroutine(routine);
            //}
        }

        /// <summary>
        /// SctiptableObjectからデータを参照してサウンド設定
        /// </summary>
        /// <param name="clipName"></param>
        /// <param name="fadeTime"></param>
        public void PlayBGMWithFadeInFomScriptableObject(string clipName, float fadeTime = 2f)
        {
            if (IsPaused) return;

            var audioClip = bgmAudioClipList.FirstOrDefault(clip => clip.name == clipName);

            if (audioClip == null)
            {
                Debug.Log(clipName + "は見つかりません");
                return;
            }

            if (bgmAudioSourceList.Any(source => source.clip == audioClip))
            {
                Debug.Log(clipName + "はすでに再生されています");
                return;
            }

            StopBGMWithFadeOut(fadeTime);//現在再生中のBGMをフェードアウトする//

            AudioSource audioSource = bgmAudioSourceList.FirstOrDefault(asb => asb.isPlaying == false);

            if (audioSource != null)
            {
                IEnumerator routine = audioSource.PlayWithFadeIn(audioClip, fadeTime);
                fadeCoroutines.Add(routine);
                StartCoroutine(routine);
            }
        }

        public void StopBGMWithFadeOut(string clipName, float fadeTime = 2f)
        {
            if (IsPaused) return;
            
            AudioSource audioSource = bgmAudioSourceList.FirstOrDefault(bas => bas.clip != null && bas.clip.name == clipName);

            if (audioSource == null || audioSource.isPlaying == false)
            {
                Debug.Log(clipName + "は再生されていません");
                return;
            }

            IEnumerator routine = audioSource.StopWithFadeOut(fadeTime);
            StartCoroutine(routine);
            fadeCoroutines.Add(routine);
        }

        public void StopAllBGM()
        {
            if (IsPaused) return;

            fadeCoroutines.ForEach(StopCoroutine);
            fadeCoroutines.Clear();
            for (int i = 0; i < bgmAudioSourceList.Count; i++)
            {
                bgmAudioSourceList[i].Stop();
            }
        }

        public void StopBGMWithFadeOut(float fadeTime = 2f)
        {
            if (IsPaused) return;
            
            fadeCoroutines.ForEach(StopCoroutine);
            fadeCoroutines.Clear();

            fadeCoroutines = bgmAudioSourceList.Where(asb => asb.isPlaying)
            .ToList()
            .ConvertAll(asb =>
                {
                    IEnumerator routine = asb.StopWithFadeOut(fadeTime);
                    StartCoroutine(routine);
                    return routine;
                });
        }

        public void PlayVoice(string clipName, float delayTime = 0f)
        {
            var audioClip = voiceAudioClipList.FirstOrDefault(clip => clip.name == clipName);
            
            if (audioClip == null)
            {
                Debug.Log(clipName + "は見つかりません");
                return;
            }
            
            voiceAudioSource.clip = audioClip;
            voiceAudioSource.PlayDelayed(delayTime);
            
            voiceAudioSource.PlayScheduled (AudioSettings.dspTime + delayTime);
        }
        public void PlayVoiceClip(AudioClip voice, float volume = 1f)
        {
            voiceAudioSource.clip = voice;
            voiceAudioSource.volume = volume;
            voiceAudioSource.loop = false;//念のため
            voiceAudioSource.Play();
        }
        public void StopVoice()
        {
            if (voiceAudioSource.isPlaying)
            {
                voiceAudioSource.Stop();
            }
        }
    }

}
