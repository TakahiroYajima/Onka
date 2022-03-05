using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;
using Onka.Manager.Data;

public class TitleManager : SceneBase
{
    [SerializeField] private TitleMenu titleMenu = null;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Initialize();
        DataManager.Instance.LoadAllSavedGameData();
        titleMenu.Initialize();
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

    public void PressQuitButton()
    {
        SoundManager.Instance.PlaySeWithKeyOne("menuse_enter");
        FadeManager.Instance.FadeOut(FadeManager.FadeColorType.Black, 1.4f, () =>
         {
             Application.Quit();
         });
    }

    public void Debug_Ending()
    {
        SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Ending", true, null, FadeManager.FadeColorType.Black, FadeManager.FadeColorType.None);
    }
}
