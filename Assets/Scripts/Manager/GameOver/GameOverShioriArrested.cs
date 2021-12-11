using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverShioriArrested : GameOverEventBase
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
