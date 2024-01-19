using System.Collections;
using System;
using UnityEngine;

public class SeeThePastActor : MonoBehaviour
{
    [SerializeField]
    private ModelFader modelFader;
    [SerializeField]
    private Transform cameraTarget;
    [SerializeField]
    private SoundPlayerObject soundPlayerObject;

    public void Execute(Action onComplete)
    {
        StartCoroutine(Action(onComplete));
    }

    private IEnumerator Action(Action onComplete)
    {
        soundPlayerObject.PlayOne(0);
        modelFader.SetStart(null);
        var angle = StageManager.Instance.CalcVerticalAngleFromCamera(cameraTarget.transform.position);
        yield return StageManager.Instance.EventMove_ForceLookCamera(cameraTarget.transform.position, 5f, true, -angle * 0.5f);
        onComplete?.Invoke();
    }
}
