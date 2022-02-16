using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;
using Onka.Manager.Data;

/// <summary>
/// シーンマネージャースクリプトの基底クラス
/// </summary>
public abstract class SceneBase : SingletonMonoBehaviour<SceneBase>
{
    [SerializeField] protected string sceneBGMKey = "";
    [SerializeField] protected SceneType thisScene;

    protected virtual void Start()
    {
        
        Initialize();
    }

    protected virtual void Initialize()
    {
        DataManager.Instance.SetCurrentSceneUseSound(thisScene);
        if (!string.IsNullOrEmpty(sceneBGMKey))
        {
            SoundManager.Instance.PlayBGMWithKeyAndFadeIn(sceneBGMKey, 0f);
        }
    }

    protected void ChangeScene(string name)
    {
        SceneControlManager.Instance.ChangeSceneWithFade(name);
    }
}
