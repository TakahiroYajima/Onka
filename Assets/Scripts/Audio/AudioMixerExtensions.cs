using UnityEngine;
using UnityEngine.Audio;

namespace SoundSystem
{
    public static class AudioMixerExtensions
    {
        public static float GetVolumeByLinear(this AudioMixer audioMixer, string exposedParamName)
        { 
            float decibel;

            audioMixer.GetFloat(exposedParamName, out decibel);
            
            if( decibel <= -96f )
            {
                return 0.0f;
            }
    
            return Mathf.Pow(10f, decibel / 20f);
        }
    
        public static void SetVolumeByLinear(this AudioMixer audioMixer, string exposedParamName, float volume) 
        {   
            float decibel = 20.0f * Mathf.Log10(volume);
    
            if (float.IsNegativeInfinity(decibel))
            {
                decibel = -96f;
            }
    
            audioMixer.SetFloat(exposedParamName, decibel);
        } 

    }
}