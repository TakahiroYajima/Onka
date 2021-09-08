using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ上のイベント用スクリプトの基底クラス
/// </summary>
public abstract class EventBase : MonoBehaviour
{
    [SerializeField] protected string eventKey = "";
    public string EventKey { get { return eventKey; } }
    [HideInInspector] public int managementID = -1;//配列で管理する際のID

    /// <summary>
    /// イベント進行のキーにより、自身のイベントが発行可能か判断する
    /// </summary>
    /// <param name="key"></param>
    public abstract void ProgressEvent(string key);

    public abstract void EventStart();
    public abstract void EventUpdate();
    public abstract void EventEnd();
}
