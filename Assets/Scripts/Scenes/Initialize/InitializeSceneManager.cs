using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializeSceneManager : SceneBase
{
    protected override void OnStartInitialize()
    {
        SceneManager.LoadScene("FirstSplash");
    }

}
