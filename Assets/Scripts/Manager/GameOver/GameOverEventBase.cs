using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameOverEventBase : MonoBehaviour
{
    public abstract void Initialize();

    public abstract void StartEvent();
    public abstract void EndEvent();
}
