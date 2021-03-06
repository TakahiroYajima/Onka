using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LoadingUIManager : SingletonMonoBehaviour<LoadingUIManager>
{
    [SerializeField] private GameObject loadingObject = null;//Loadingの親Object
    [SerializeField] private Image loadingGauge = null;//ロード中のアニメーションゲージ
    //[SerializeField] private Text loadingText = null;//ロード中テキスト
    // Start is called before the first frame update
    void Start()
    {
        loadingObject.SetActive(false);
    }

    public void SetActive(bool _isActive)
    {
        loadingGauge.fillAmount = 0f;
        loadingObject.SetActive(_isActive);
    }

    public void StartLoading(AsyncOperation asyncOperation, UnityAction onComplete, bool isAutoEnactive = true)
    {
        StartCoroutine(DoLoading(asyncOperation, onComplete));
    }

    private IEnumerator DoLoading(AsyncOperation asyncOperation, UnityAction onComplete, bool isAutoEnactive = true)
    {
        float currentTime = 0f;
        float needTime = 1.4f;
        float progress = 0f;
        float pTime = 0f;
        asyncOperation.allowSceneActivation = false;

        loadingGauge.fillAmount = 0f;
        loadingObject.SetActive(true);
        while (currentTime < needTime || progress < 1f) {
            progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            pTime = currentTime / needTime;
            if (progress < pTime) { loadingGauge.fillAmount = progress; }
            else { loadingGauge.fillAmount = pTime; }
            currentTime += Time.deltaTime;
            yield return null;
        }
        asyncOperation.allowSceneActivation = true;
        
        if(onComplete != null)
        {
            onComplete();
        }
        if (isAutoEnactive)
        {
            loadingObject.SetActive(false);
        }
    }
}
