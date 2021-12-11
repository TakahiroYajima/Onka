using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverAzuYuzuArrested : GameOverEventBase
{

    public override void Initialize()
    {

    }

    public override void StartEvent()
    {
        GameOverManager.Instance.EndEventAction();
    }
    public override void EndEvent()
    {

    }
}
