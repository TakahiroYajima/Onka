using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// URP用のノイズエフェクトの表示切り替え・演出を行う
/// </summary>
public class CRTController : MonoBehaviour
{
    [SerializeField]
    private Volume crtVolume;

    private Bleed bleed;
    private CRTAperture crt;
    private NTSCEncode ntsc;
    private Phosphor phosphor;

    private bool isActive;

    private const float PhosphorFadeMin = 0.08f;
    private const float PhosphorFadeMax = 0.16f;
    private const float EffectStartTime = 3.5f;
    private const float NoiseTime = 0.5f;

    private float currentEffectTime = 0f;
    private float currentNoseEffectTime = 0f;

    public void PlayEffect()
    {
        if (crtVolume.profile.TryGet(out bleed))
        {
            bleed.active = true;
        }
        if (crtVolume.profile.TryGet(out crt))
        {
            crt.active = true;
        }
        if (crtVolume.profile.TryGet(out ntsc))
        {
            ntsc.active = true;
        }
        if (crtVolume.profile.TryGet(out phosphor))
        {
            phosphor.active = true;
            DoNoise(0f);
        }
        isActive = true;
        currentEffectTime = 0f;
        currentNoseEffectTime = 0f;
    }

    public void CancelEffect()
    {
        if (bleed != null)
        {
            bleed.active = false;
        }
        if (crt != null)
        {
            crt.active = false;
        }
        if (ntsc != null)
        {
            ntsc.active = false;
        }
        if (phosphor != null)
        {
            phosphor.active = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (currentEffectTime >= EffectStartTime)
            {
                if(currentNoseEffectTime < NoiseTime)
                {
                    currentNoseEffectTime += Time.deltaTime;
                    DoNoise(currentNoseEffectTime / NoiseTime);
                }
                else
                {
                    DoNoise(0f);
                    currentNoseEffectTime = 0f;
                    currentEffectTime = 0f;
                }
            }
            else
            {
                currentEffectTime += Time.deltaTime;
            }
        }
    }

    private void DoNoise(float t)
    {
        if (phosphor != null)
        {
            var ratio = Mathf.PingPong(t, 0.5f) * 2;
            phosphor.fade.value = Mathf.Lerp(PhosphorFadeMin, PhosphorFadeMax, ratio);
        }
    }
}
