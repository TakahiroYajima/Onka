using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EA_AfterGetHatsuDiary3 : EventActorBase
{
    [SerializeField] private HatsuThreatenAction threatenAction = null;
    
    public Event_AfterGetHatsuDiary3 eventBase { private get; set; }

    protected override void Initialize()
    {
        threatenAction.Initialize();
    }

    public override void EventStart()
    {
        StartCoroutine(HatsuEvent());
    }
    public override void EventUpdate()
    {

    }
    public override void EventEnd()
    {
        

    }

    private IEnumerator HatsuEvent()
    {
        float t = 0f;
        while(t < 3f)
        {
            if(StageManager.Instance.Player.currentState == PlayerState.Free ||
                StageManager.Instance.Player.currentState == PlayerState.Chased)
            {
                t += Time.deltaTime;
            }
            yield return null;
        }
        //プレイヤーがアイテムを見ていたら強制で通常の状態に戻す
        if(StageManager.Instance.Player.currentState == PlayerState.ItemGet)
        {
            ItemManager.Instance.FinishWatchingItemEnforcement();
        }

        StartCoroutine(threatenAction.HatsuEvent(() =>
        {
            parent.EventClearContact();
        }));
    }
}
