using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningSceneManager : SceneBase
{
    [SerializeField] private OpeningEventManager openingEventManager = null;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Initialize();
        //openingEventManager.StartAction();
    }
}
