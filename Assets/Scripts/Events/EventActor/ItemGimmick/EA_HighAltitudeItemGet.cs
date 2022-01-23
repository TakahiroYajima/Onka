using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EA_HighAltitudeItemGet : EventActorBase
{
    [HideInInspector] public Event_HighAltitudeItemGet eventBase = null;

    [SerializeField] private GameObject longStickObjectPref = null;
    [SerializeField] private GameObject longStickItemInitPosition = null;
    [SerializeField] private GameObject longStickItemMoveTargetPosition = null;
    [SerializeField] private GameObject itemObjectMoveTargetPosition = null;
    [SerializeField] private GameObject itemObjectMoveMidPosition = null;
    
    protected override void Initialize()
    {

    }
    public override void EventStart()
    {
        StartCoroutine(ItemGetEvent());
    }
    public override void EventUpdate()
    {
        
    }
    public override void EventEnd()
    {

    }

    private IEnumerator ItemGetEvent()
    {
        GameObject stick = Instantiate(longStickObjectPref, transform);
        stick.transform.position = longStickItemInitPosition.transform.position;
        stick.transform.rotation = longStickItemInitPosition.transform.rotation;

        Vector3 initItemPos = eventBase.getItemObject.transform.position;
        Vector3 itemMoveTarget = itemObjectMoveTargetPosition.transform.position;
        Vector3 itemMoveMidPos = itemObjectMoveMidPosition.transform.position;

        yield return new WaitForSeconds(0.35f);

        float t = 0f;
        while (t < 1f)
        {
            stick.transform.position = Vector3.Lerp(longStickItemInitPosition.transform.position, longStickItemMoveTargetPosition.transform.position, t);
            eventBase.getItemObject.transform.position = CalcLerpPoint(initItemPos, itemMoveMidPos, itemMoveTarget, t);
            t += Time.deltaTime;
            yield return null;
        }

        Destroy(stick);
        eventBase.getItemObject.gameObject.SetActive(false);

        FinishEvent();
    }
    
    private Vector3 CalcLerpPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        var a = Vector3.Lerp(p0, p1, t);
        var b = Vector3.Lerp(p1, p2, t);
        return Vector3.Lerp(a, b, t);
    }
}
