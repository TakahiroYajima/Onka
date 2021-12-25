using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : SceneBase
{

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Initialize();
        DataManager.Instance.InitializeDataLoad();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PressStartButton()
    {
        SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Opening",true, null, FadeManager.FadeColorType.Black, FadeManager.FadeColorType.None);
    }

    public void PressLoadButton()
    {
        SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Game", true, null, FadeManager.FadeColorType.Black, FadeManager.FadeColorType.Black);
    }

    public void Debug_Ending()
    {
        SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Ending", true, null, FadeManager.FadeColorType.Black, FadeManager.FadeColorType.None);
    }
}
