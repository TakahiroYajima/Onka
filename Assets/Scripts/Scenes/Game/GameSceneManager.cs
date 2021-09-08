using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;

public class GameSceneManager : SceneBase
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Initialize();
        SoundManager.Instance.PlayEnvironmentWithKey("ambient_in_house");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
