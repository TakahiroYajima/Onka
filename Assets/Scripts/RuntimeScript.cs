using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RuntimeScript : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod]
    static void RuntimeInit()
    {
        if(SceneManager.GetActiveScene().name != "Initialize")
        {
            SceneManager.LoadScene("Initialize");
        }   
    }
}
