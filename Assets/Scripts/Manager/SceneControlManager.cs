using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using SoundSystem;

public class SceneControlManager : SingletonMonoBehaviour<SceneControlManager>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeSceneWithFade(string sceneName, bool isBGMStop = true, UnityAction onComplete = null)
    {
        if (isBGMStop)
        {
            SoundManager.Instance.StopBGMWithFadeOut(1f);
        }
        FadeManager.Instance.FadeOut(FadeManager.FadeColorType.Black, 1f, () =>
        {
            SceneManager.LoadScene(sceneName);
            FadeManager.Instance.FadeIn(FadeManager.FadeColorType.Black, 1f, onComplete);
        });
    }
}
