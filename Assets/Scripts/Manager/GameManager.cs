﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{

    protected override void Awake()
    {
        base.Awake();
        LayerMaskData.Initialize();
    }
}
