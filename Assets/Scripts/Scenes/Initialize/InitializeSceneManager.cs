using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializeSceneManager : SceneBase
{
    // Start is called before the first frame update
    protected override void Start()
    {
        SceneManager.LoadScene("FirstSplash");
    }

}
