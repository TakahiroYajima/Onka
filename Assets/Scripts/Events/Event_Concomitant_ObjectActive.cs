using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 何らかのイベントに付随する形で行われるイベント（コライダーのアクティブの切り替えなどに使う）
/// </summary>
public class Event_Concomitant_ObjectActive : MonoBehaviour
{
    [SerializeField] private EventBase targetEventBase = null;
    [SerializeField] private List<GameObject> doActiveObject = new List<GameObject>();

    private void Awake()
    {
        targetEventBase.AddOnEventActiveCallback(DoActiveObject);
        targetEventBase.AddOnEventEndCallback(DoInactiveObject);
        DoInactiveObject();
    }

    private void DoActiveObject()
    {
        for(int i = 0; i < doActiveObject.Count; i++)
        {
            doActiveObject[i].SetActive(true);
        }
    }
    private void DoInactiveObject()
    {
        for (int i = 0; i < doActiveObject.Count; i++)
        {
            doActiveObject[i].SetActive(false);
        }
    }
}
