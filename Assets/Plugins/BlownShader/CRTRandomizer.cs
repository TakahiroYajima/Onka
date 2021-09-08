using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public class CRTRandomizer : MonoBehaviour
{
    [SerializeField]
    CRT crt;

    void Start()
    {

        var resetTimer = Observable.FromCoroutine<float>(RandomInterval);

        var totalTime = 0.0f;
        var time = 0.0f;
        var noisePower = 0.0f;
        var baseNoisePower = 0.0f;
        var noisyTime = 0.0f;
        var offset = Vector2.zero;
        var baseOffset = Vector2.zero;

        resetTimer.Subscribe(t =>
        {
            totalTime = t;
            time = 0;
            //noisePower = UnityEngine.Random.Range(0.0f, 1.0f);
            //baseNoisePower = Mathf.Clamp01(UnityEngine.Random.Range(-0.01f, 0.01f));
            //noisyTime = UnityEngine.Random.Range(0.0f, 0.5f);
            //crt.SinNoiseWidth = UnityEngine.Random.Range(0.0f, 30.0f);
            //offset = Vector2.right * UnityEngine.Random.Range(-5.0f, 5) + Vector2.up * UnityEngine.Random.Range(-5.0f, 5);
            //baseOffset = (Vector2.right * UnityEngine.Random.Range(-1.0f, 1) + Vector2.up * UnityEngine.Random.Range(-1.0f, 1)) * 0.05f;
            noisePower = 0.6f;
            baseNoisePower = Mathf.Clamp01(0.005f);
            noisyTime = 0.2f;
            crt.SinNoiseWidth = 0f;
            offset = Vector2.right * 0f + Vector2.up * 0f;
            baseOffset = (Vector2.right * 0f + Vector2.up * 0f) * 0.05f;
        });

        this.UpdateAsObservable().Subscribe(_ =>
        {
            float t = time / totalTime;
            float nt = Mathf.Clamp01(t / noisyTime);
            float np = baseNoisePower + noisePower * (1 - nt);
            crt.NoiseX = np * 0.5f;
            crt.RGBNoise = np * 0.7f;
            crt.SinNoiseScale = np * 1.0f;
            crt.SinNoiseOffset += Time.deltaTime * 2;
            crt.Offset = baseOffset + offset * (np + baseNoisePower * t * 4);
            time += Time.deltaTime;
        });
    }

    public IEnumerator RandomInterval(IObserver<float> observer)
    {
        while (true)
        {
            float wait = UnityEngine.Random.Range(5, 7);
            //float wait = 3f;
            observer.OnNext(wait);
            yield return new WaitForSeconds(wait);
        }
    }
}
