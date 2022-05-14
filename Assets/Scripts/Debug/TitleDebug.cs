#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleDebug : MonoBehaviour
{
    bool isEKeyPush = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isEKeyPush) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Ending", true);
        }
    }
}
#endif