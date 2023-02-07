using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class LoadingUIManager : SingletonMonoBehaviour<LoadingUIManager>
{
    public class LoadingParameter
    {
        public AsyncOperation asyncOperation;
        public Action onCompleted;
        public string message;
        public bool isAutoEnactive = true;
    }

    [SerializeField] private GameObject loadingObject = null;//Loadingの親Object
    [SerializeField] private Image loadingGauge = null;//ロード中のアニメーションゲージ
    [SerializeField] private Text loadingText = null;//ロード中テキスト

    private bool isAutoMove = false;
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

    public void SetMessage(string message)
    {
        loadingText.text = message;
    }

    public void SetProgress(float fill)
    {
        if (isAutoMove) return;
        loadingGauge.fillAmount = fill;
    }

    public void StartLoading(LoadingParameter param)
    {
        StartCoroutine(DoLoading(param.asyncOperation, param.onCompleted, param.message, param.isAutoEnactive));
    }

    private IEnumerator DoLoading(AsyncOperation asyncOperation, Action onComplete, string message, bool isAutoEnactive = true)
    {
        float currentTime = 0f;
        float needTime = 1f;
        float progress = 0f;
        float pTime = 0f;

        loadingText.text = message;
        loadingGauge.fillAmount = 0f;
        loadingObject.SetActive(true);
        isAutoMove = true;
        while (currentTime < needTime || progress < 1f) {
            progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            pTime = currentTime / needTime;
            if (progress < pTime) { loadingGauge.fillAmount = progress; }
            else { loadingGauge.fillAmount = pTime; }
            currentTime += Time.deltaTime;
            yield return null;
        }

        if(onComplete != null)
        {
            onComplete();
        }
        if (isAutoEnactive)
        {
            loadingObject.SetActive(false);
        }
        isAutoMove = false;
    }
}
