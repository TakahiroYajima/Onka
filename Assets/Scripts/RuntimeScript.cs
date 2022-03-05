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
        if(SceneManager.GetActiveScene().name != "Initialize")
        {
            SceneManager.LoadScene("Initialize");
        }   
    }
}
