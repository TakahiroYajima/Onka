using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : SceneBase
{

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Initialize();
    }

    public void PressStartButton()
    {
        SceneControlManager.Instance.ChangeSceneWithFade("Game");
    }
}
