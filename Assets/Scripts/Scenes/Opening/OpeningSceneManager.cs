using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningSceneManager : SceneBase
{
    [SerializeField] private OpeningEventManager openingEventManager = null;

    protected override void OnStartInitialize()
    {
        base.Initialize();
        //openingEventManager.StartAction();
    }
}
