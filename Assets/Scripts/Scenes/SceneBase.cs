using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;

/// <summary>
/// シーンマネージャースクリプトの基底クラス
/// </summary>
public abstract class SceneBase : SingletonMonoBehaviour<SceneBase>
{
    [SerializeField] protected string sceneBGMName = "";
    [SerializeField] protected SceneType thisScene;

    protected virtual void Start()
    {
        
        Initialize();
    }

    protected virtual void Initialize()
    {
        DataManager.Instance.SetCurrentSceneUseSound(thisScene);
        if (!string.IsNullOrEmpty(sceneBGMName))
        {
            SoundManager.Instance.PlayBGMWithFadeIn(sceneBGMName, 0f);
        }
    }

    protected void ChangeScene(string name)
    {
        SceneControlManager.Instance.ChangeSceneWithFade(name);
    }
}
