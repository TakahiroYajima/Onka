using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using SoundSystem;

public class SceneControlManager : SingletonMonoBehaviour<SceneControlManager>
{
    public ChangeSceneMoveType changeSceneMoveType = ChangeSceneMoveType.Normal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeScene(string sceneName, bool isBGMStop = true, UnityAction onComplete = null, FadeManager.FadeColorType fadeOutColorType = FadeManager.FadeColorType.None, FadeManager.FadeColorType fadeInColorType = FadeManager.FadeColorType.None)
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

    //public void ChangeSceneWithFade(string sceneName, bool isBGMStop = true, UnityAction onComplete = null)
    //{
    //    if (isBGMStop)
    //    {
    //        StopBGMAndEnvironment();
    //    }
    //    FadeManager.Instance.FadeOut(FadeManager.FadeColorType.Black, 1f, () =>
    //    {
    //        SceneManager.LoadScene(sceneName);
    //        FadeManager.Instance.FadeIn(FadeManager.FadeColorType.Black, 1f, onComplete);
    //    });
    //}

    public void ChangeSceneAsyncWithLoading(string sceneName, bool isBGMStop = true, UnityAction onComplete = null, FadeManager.FadeColorType fadeOutColorType = FadeManager.FadeColorType.None, FadeManager.FadeColorType fadeInColorType = FadeManager.FadeColorType.None)
    {
        if (isBGMStop)
        {
            StopBGMAndEnvironment();
        }
        FadeManager.Instance.FadeOut(fadeOutColorType, 1f, () =>
        {
            LoadingUIManager.Instance.SetActive(true);
            AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
            //SceneManager.sceneLoaded += (Scene nextScene, LoadSceneMode mode) => {
            //    FadeManager.Instance.FadeIn(fadeInColorType, 1f, onComplete);
            //};
            LoadingUIManager.Instance.StartLoading(async, () =>
            {
                StartCoroutine(WaitSceneLoadAfterFadeIn(async, fadeInColorType, onComplete));
                //FadeManager.Instance.FadeIn(fadeInColorType, 1f, onComplete);
            });
            //StartCoroutine(ChangeSceneAsyncCoroutine(sceneName, onComplete, fadeInColorType));
        });
    }

    private IEnumerator WaitSceneLoadAfterFadeIn(AsyncOperation async, FadeManager.FadeColorType fadeInColorType = FadeManager.FadeColorType.None, UnityAction onComplete = null)
    {
        yield return async;
        FadeManager.Instance.FadeIn(fadeInColorType, 1f, onComplete);
    }

    private void StopBGMAndEnvironment()
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