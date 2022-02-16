using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;

public class TitleManager : SceneBase
{

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Initialize();
        //DataManager.Instance.InitializeDataLoad();
        InGameUtil.DoCursorFree();
    }

    public void PressStartButton()
    {
        SoundManager.Instance.PlaySeWithKeyOne("menuse_enter");
        SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Opening",true, null, FadeManager.FadeColorType.Black, FadeManager.FadeColorType.None);
    }

    public void PressLoadButton()
    {
        SoundManager.Instance.PlaySeWithKeyOne("menuse_enter");
        SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Game", true, null, FadeManager.FadeColorType.Black, FadeManager.FadeColorType.Black);
    }

    public void PressBonusButton()
    {
        SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Bonus", true, null, FadeManager.FadeColorType.Black, FadeManager.FadeColorType.Black);
    }

    public void Debug_Ending()
    {
        SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Ending", true, null, FadeManager.FadeColorType.Black, FadeManager.FadeColorType.None);
    }
}
