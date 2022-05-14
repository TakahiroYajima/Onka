using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RuntimeScript : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    static void RuntimeInit()
    {
        Application.targetFrameRate = 60;
        //Debug.unityLogger.logEnabled = false;
        //Debug.unityLogger.logEnabled = false;
#if UNITY_EDITOR
        if (SceneManager.GetActiveScene().name == "撮影用") return;
#endif

        if (SceneManager.GetActiveScene().name != "Initialize")
        {
            SceneManager.LoadScene("Initialize");
        }
    }
}
