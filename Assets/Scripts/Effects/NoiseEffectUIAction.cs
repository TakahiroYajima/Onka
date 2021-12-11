using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// ノイズUIのアルファ値を調整することでアニメーションっぽく見せる
/// </summary>
public class NoiseEffectUIAction : MonoBehaviour
{
    [SerializeField] private Image noiseFrontImage = null;
    [SerializeField, Range(0,1)] private float[] noiseAlphas = null;

    private float initNoiseAlpha = 0f;

    // Start is called before the first frame update
    void Start()
    {
        initNoiseAlpha = noiseFrontImage.color.a;
    }

    public void StartNoiseAnimation(UnityAction onComplete = null)
    {
        StartCoroutine(NoiseAciton(onComplete));
    }

    private IEnumerator NoiseAciton(UnityAction onComplete = null)
    {
        Color c = noiseFrontImage.color;
        for (int i = 0; i < noiseAlphas.Length; i++)
        {
            c.a = noiseAlphas[i];
            noiseFrontImage.color = c;
            yield return new WaitForSeconds(0.1f);
        }
        if(onComplete != null)
        {
            onComplete();
        }
    }
}
