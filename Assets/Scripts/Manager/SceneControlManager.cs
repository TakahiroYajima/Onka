using System.Collections;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using SoundSystem;

public class SceneControlManager : SingletonMonoBehaviour<SceneControlManager>
{
    public ChangeSceneMoveType changeSceneMoveType = ChangeSceneMoveType.Normal;

    
    public void ChangeScene(string sceneName, bool isBGMStop = true, Action onComplete = null, FadeManager.FadeColorType fadeOutColorType = FadeManager.FadeColorType.None, FadeManager.FadeColorType fadeInColorType = FadeManager.FadeColorType.None)
    {
        if (isBGMStop)
        {
            StopBGMAndEnvironment();
        }
        FadeManager.Instance.FadeOut(fadeOutColorType, 1f, () =>
        {
            SceneManager.LoadScene(sceneName);
            FadeManager.Instance.FadeIn(fadeInColorType, 1f, onComplete);
        });
    }

    //public void ChangeSceneAsyncWithFadeOnly(string sceneName, FadeManager.FadeColorType fadeOutColorType = FadeManager.FadeColorType.None, FadeManager.FadeColorType fadeInColorType = FadeManager.FadeColorType.None, bool isBGMStop = true, UnityAction onComplete = null)
    //{
    //    if (isBGMStop)
    //    {
    //        StopBGMAndEnvironment();
    //    }
    //    FadeManager.Instance.FadeOut(fadeOutColorType, 1f, () =>
    //    {
    //        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
    //        StartCoroutine(WaitSceneLoadAfterFadeIn(async, fadeInColorType, onComplete));
    //    });
    //}

    public void ChangeSceneAsyncWithLoading(string sceneName, bool isBGMStop = true, Action onComplete = null, FadeManager.FadeColorType fadeOutColorType = FadeManager.FadeColorType.None, FadeManager.FadeColorType fadeInColorType = FadeManager.FadeColorType.None, bool isAfterLoadFadeOut = true)
    {
        //Debug.Log($"ChangeSceneAsyncWithLoading : {System.DateTime.Now.ToString()}");
        if (isBGMStop)
        {
            StopBGMAndEnvironment();
        }
        //Debug.Log($"FadeOutStart : {System.DateTime.Now.ToString()}");
        FadeManager.Instance.FadeOut(fadeOutColorType, 1f, () =>
        {
            //Debug.Log($"FadeOuted : {System.DateTime.Now.ToString()}");
            LoadingUIManager.Instance.SetActive(true);
            //Debug.Log($"asyncInstance : {System.DateTime.Now.ToString()}");
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

            //Debug.Log($"asyncInstanted : {System.DateTime.Now.ToString()}");
            async.allowSceneActivation = false;
            LoadingUIManager.LoadingParameter param = new LoadingUIManager.LoadingParameter()
            {
                asyncOperation = async,
                onCompleted = ()=>
                {
                    async.allowSceneActivation = true;
                    if (isAfterLoadFadeOut)
                    {
                        StartCoroutine(WaitSceneLoadAfterFadeIn(async, fadeInColorType, onComplete));
                    }
                    else
                    {
                        onComplete?.Invoke();
                    }
                },
                message = "ロード中",
                isAutoEnactive = isAfterLoadFadeOut
            };
            LoadingUIManager.Instance.StartLoading(param);
        });
    }
    ////動作確認用
    //private IEnumerator LoadSampleCor(string sceneName, FadeManager.FadeColorType fadeInColorType, UnityAction onComplete = null)
    //{
    //    Debug.Log($"FadeOuted : {System.DateTime.Now.ToString()}");
    //    yield return null;
    //    LoadingUIManager.Instance.SetActive(true);
    //    Debug.Log($"asyncInstance : {System.DateTime.Now.ToString()}");
    //    yield return null;
    //    AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
    //    //SceneManager.sceneLoaded += (Scene nextScene, LoadSceneMode mode) => {
    //    //    FadeManager.Instance.FadeIn(fadeInColorType, 1f, onComplete);
    //    //};
    //    yield return null;
    //    Debug.Log($"asyncInstanted : {System.DateTime.Now.ToString()}");
    //    yield return null;
    //    LoadingUIManager.Instance.StartLoading(async, () =>
    //    {
    //        StartCoroutine(LoadedSample(async, fadeInColorType, onComplete));
    //        //FadeManager.Instance.FadeIn(fadeInColorType, 1f, onComplete);
    //    });
    //}
    //private IEnumerator LoadedSample(AsyncOperation async, FadeManager.FadeColorType fadeInColorType, UnityAction onComplete = null)
    //{
    //    Debug.Log($"LoadEnd : {System.DateTime.Now.ToString()}");
    //    yield return null;
    //    StartCoroutine(WaitSceneLoadAfterFadeIn(async, fadeInColorType, onComplete));
    //}

    private IEnumerator WaitSceneLoadAfterFadeIn(AsyncOperation async, FadeManager.FadeColorType fadeInColorType = FadeManager.FadeColorType.None, Action onComplete = null)
    {
        yield return async;
        //Debug.Log($"AsyncEnded : {System.DateTime.Now.ToString()}");
        LoadingUIManager.Instance.SetActive(false);
        FadeManager.Instance.FadeIn(fadeInColorType, 1f, onComplete);
    }

    public void StopBGMAndEnvironment()
    {
        SoundManager.Instance.StopBGMWithFadeOut(1f);
        SoundManager.Instance.StopEnvironment();
    }

    //private IEnumerator ChangeSceneAsyncCoroutine(string sceneName, UnityAction onComplete = null, FadeManager.FadeColorType fadeInColorType = FadeManager.FadeColorType.None)
    //{
    //    AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
    //    LoadingUIManager.Instance.StartLoading(async, () =>
    //    {
    //        FadeManager.Instance.FadeIn(fadeInColorType, 1f, onComplete);
    //    });
    //}
}

public enum ChangeSceneMoveType
{
    NewGame,//新しく始める
    LoadData,//セーブデータからの再開
    Normal,//その他、通常の遷移
}