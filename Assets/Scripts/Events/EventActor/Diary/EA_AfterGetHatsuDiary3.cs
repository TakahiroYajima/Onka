using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EA_AfterGetHatsuDiary3 : EventActorBase
{
    [SerializeField] private Canvas canvasObj = null;
    [SerializeField] private RectTransform hatsuRectTransform = null;
    [SerializeField] private AudioSource hatsuAudio = null;
    [SerializeField] private AudioSource noiseAudio = null;
    [SerializeField] private AudioClip hatsuVoiceSEClip = null;
    [SerializeField] private AudioClip noiseSEClip = null;
    public Event_AfterGetHatsuDiary3 eventBase { private get; set; }
    private Vector2 initPos = Vector2.zero;

    protected override void Initialize()
    {
        canvasObj.gameObject.SetActive(false);
        
        
        initPos = hatsuRectTransform.anchoredPosition;
    }

    public override void EventStart()
    {
        Debug.Log("初イベントスタート");
        canvasObj.renderMode = RenderMode.ScreenSpaceCamera;
        canvasObj.worldCamera = eventBase.WorldCamera;
        StartCoroutine(HatsuEvent(() =>
        {
            parent.EventClearContact();
        }));
    }
    public override void EventUpdate()
    {

    }
    public override void EventEnd()
    {
        eventBase.CRT.enabled = false;
    }

    private IEnumerator HatsuEvent(UnityAction onComplete = null)
    {
        int count = 0;
        
        Vector2 underPos = initPos;
        underPos.y -= 30f;
        yield return new WaitForSeconds(3f);
        //プレイヤーがアイテムを見ていたら強制で通常の状態に戻す
        if(StageManager.Instance.Player.currentState == PlayerState.ItemGet)
        {
            ItemManager.Instance.FinishWatchingItemEnforcement();
        }
        canvasObj.gameObject.SetActive(true);
        eventBase.CRT.enabled = true;
        hatsuAudio.clip = hatsuVoiceSEClip;
        noiseAudio.clip = noiseSEClip;
        hatsuAudio.Play();
        noiseAudio.Play();
        while (count < 3)
        {
            yield return new WaitForSeconds(0.1f);
            hatsuRectTransform.anchoredPosition = underPos;
            yield return new WaitForSeconds(0.1f);
            hatsuRectTransform.anchoredPosition = initPos;
            count++;
        }
        yield return new WaitForSeconds(0.1f);
        hatsuRectTransform.anchoredPosition = underPos;
        hatsuAudio.Stop();
        noiseAudio.Stop();
        eventBase.CRT.enabled = false;
        if (onComplete != null)
        {
            onComplete();
        }
    }
}
