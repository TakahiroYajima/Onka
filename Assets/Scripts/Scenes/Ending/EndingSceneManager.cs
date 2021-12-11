using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingSceneManager : SceneBase
{
    [SerializeField] private EndingEventManager endingEventManager = null;
    [SerializeField] private GameObject camera = null;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Initialize();
        endingEventManager.Initialize(camera);
        endingEventManager.StartAction();
    }
}
