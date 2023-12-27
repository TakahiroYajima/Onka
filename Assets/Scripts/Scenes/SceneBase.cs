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
    public SceneType CurrentScene => thisScene;

    private void Start()
    {
#if UNITY_EDITOR
        OnkaDebug.Instance.SetCurrentScene(this);
#endif
        OnStartInitialize();
    }

    protected virtual void OnStartInitialize()
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

    public virtual void SceneStart()
    {

    }
    public virtual void SceneStartWithOpening()
    {

    }
}
