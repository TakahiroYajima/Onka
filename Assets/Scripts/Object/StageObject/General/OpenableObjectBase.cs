using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class OpenableObjectBase : MonoBehaviour
{
    [NonSerialized] public Action onOpened;
    [NonSerialized] public Action onClosed;
}
