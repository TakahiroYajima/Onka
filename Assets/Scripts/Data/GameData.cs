﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public ItemDataList itemDataList = null;
    public EventDataList eventDataList = null;

    /// <summary>
    /// データ新規作成用初期化
    /// </summary>
    public void AllInitialize()
    {
        for(int i = 0; i < itemDataList.itemDataList.Count; i++)
        {
            itemDataList.itemDataList[i].geted = false;
            itemDataList.itemDataList[i].used = false;
        }
        for(int i = 0; i < eventDataList.list.Count; i++)
        {
            eventDataList.list[i].isAppeared = false;
            eventDataList.list[i].isEnded = false;
        }
    }
}