using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;
using Onka.Manager.Data;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private TitleMenu titleMenu = null;
    [SerializeField] protected string sceneBGMKey = "";
    [SerializeField] protected SceneType thisScene;

    public void Initialize()
    {
        DataManager.Instance.SetCurrentSceneUseSound(thisScene);
        if (!string.IsNullOrEmpty(sceneBGMKey))
        {
            SoundManager.Instance.PlayBGMWithKeyAndFadeIn(sceneBGMKey, 0f);
        }

        DataManager.Instance.LoadAllSavedGameData();
        titleMenu.Initialize();
        InGameUtil.DoCursorFree();
    }

    public void PressStartButton()
    {
        SoundManager.Instance.PlaySeWithKeyOne("menuse_enter");
        //SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Opening",true, null, FadeManager.FadeColorType.Black, FadeManager.FadeColorType.None);
        SceneControlManager.Instance.StopBGMAndEnvironment();
        FadeManager.Instance.FadeOut(FadeManager.FadeColorType.Black, FadeManager.DefaultDuration, ChangeSceneGameWithOpening);
    }

    public void PressLoadButton()
    {
        SoundManager.Instance.PlaySeWithKeyOne("menuse_enter");
        //SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Game", true, null, FadeManager.FadeColorType.Black, FadeManager.FadeColorType.Black, false);
        SceneControlManager.Instance.StopBGMAndEnvironment();
        FadeManager.Instance.FadeOut(FadeManager.FadeColorType.Black, FadeManager.DefaultDuration, ChangeSceneGame);
    }

    private void ChangeSceneGame()
    {
        titleMenu.gameObject.SetActive(false);
        GameSceneManager.Instance.SceneStart();
    }

    private void ChangeSceneGameWithOpening()
    {
        titleMenu.gameObject.SetActive(false);
        GameSceneManager.Instance.SceneStartWithOpening();
    }

    //private IEnumerator ChangeToGame()
    //{
    //    FadeManager.Instance.FadeOut();
    //}

    public void PressBonusButton()
    {
        SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Bonus", true, null, FadeManager.FadeColorType.Black, FadeManager.FadeColorType.Black, true, 1f, 2.1f);
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
