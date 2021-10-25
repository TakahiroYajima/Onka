using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GeneralCoroutine : SingletonMonoBehaviour<GeneralCoroutine>
{

    public void Wait(float time, UnityAction onComplete)
    {
        StartCoroutine(WaitCoroutine(time, onComplete));
    }

    public IEnumerator WaitCoroutine(float time, UnityAction onComplete)
    {
        yield return new WaitForSeconds(time);
        onComplete();
    }
}
