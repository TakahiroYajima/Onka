using System.Collections;
using System;
using UnityEngine;

/// <summary>
/// 初がドアップで脅かすUI系アクション
/// </summary>
public class HatsuThreatenAction : MonoBehaviour
{
    [SerializeField] private Canvas canvasObj = null;
    [SerializeField] private RectTransform hatsuRectTransform = null;
    [SerializeField] private AudioSource hatsuAudio = null;
    [SerializeField] private AudioSource noiseAudio = null;
    [SerializeField] private AudioClip hatsuVoiceSEClip = null;
    [SerializeField] private AudioClip noiseSEClip = null;

    private Vector2 initPos = Vector2.zero;

    private CRT crt = null;
    private Camera worldCamera = null;

    public void Initialize()
    {
        crt = StageManager.Instance.Player.CameraObj.GetComponent<CRT>();
        worldCamera = StageManager.Instance.Player.CameraObj.GetComponent<Camera>();
        canvasObj.gameObject.SetActive(false);
        initPos = hatsuRectTransform.anchoredPosition;
    }

    public IEnumerator HatsuEvent(Action onComplete = null)
    {
        canvasObj.renderMode = RenderMode.ScreenSpaceCamera;
        canvasObj.worldCamera = worldCamera;

        int count = 0;

        Vector2 underPos = initPos;
        underPos.y -= 30f;

        canvasObj.gameObject.SetActive(true);
        crt.enabled = true;
        hatsuAudio.clip = hatsuVoiceSEClip;
        noiseAudio.clip = noiseSEClip;
        hatsuAudio.Play();
        noiseAudio.Play();
        while (count < 3)
        {
            yield return new WaitForSeconds(0.1f);
            hatsuRectTransform.anchoredPosition = underPos;
            yield return new WaitForSeconds(0.1f);
            hatsuRectTransform.anchoredPosition = initPos;
            count++;
        }
        yield return new WaitForSeconds(0.1f);
        hatsuRectTransform.anchoredPosition = underPos;
        hatsuAudio.Stop();
        noiseAudio.Stop();
        crt.enabled = false;
        if (onComplete != null)
        {
            onComplete();
        }
        canvasObj.gameObject.SetActive(false);
    }
}
