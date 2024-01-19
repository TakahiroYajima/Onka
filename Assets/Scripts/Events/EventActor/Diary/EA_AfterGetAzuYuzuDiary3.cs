using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EA_AfterGetAzuYuzuDiary3 : EventActorBase
{
    private BoxCollider boxCollider1 = null;
    [SerializeField] private CollisionEnterEvent collisionEnterEvent1 = null;
    private BoxCollider boxCollider2 = null;
    [SerializeField] private CollisionEnterEvent collisionEnterEvent2 = null;
    [SerializeField] private SoundPlayerObject soundPlayer = null;

    [SerializeField] private Image thunderImage = null;
    [SerializeField] private Enemy_Azuha azuha = null;
    [SerializeField] private Enemy_Yuzuha yuzuha = null;

    private enum AYDState
    {
        Thunder,//雷で脅かす
        EnemyInstance,//2回目の雷、彩珠波と柚子羽登場、プレイヤーを追いかける
        Chase,
    }
    private AYDState currentState = AYDState.Thunder;

    protected override void Initialize()
    {
        StageManager.Instance.Azuha = azuha;
        StageManager.Instance.Yuzuha = yuzuha;
        azuha.gameObject.SetActive(false);
        yuzuha.gameObject.SetActive(false);
        boxCollider1 = collisionEnterEvent1.GetComponent<BoxCollider>();
        boxCollider1.enabled = true;
        boxCollider2 = collisionEnterEvent2.GetComponent<BoxCollider>();
        boxCollider2.enabled = false;
        thunderImage.gameObject.SetActive(false);
        
        parent.SetCanBeStarted(false);
    }

    public override void EventStart()
    {
        //soundPlayer.PlaySE("se_lug");
        //StartCoroutine(MoveStatus(() =>
        //{
        //    parent.EventClearContact();
        //}));
    }
    public override void EventUpdate()
    {
        if (currentState != AYDState.Chase) return;
        if(azuha == null && yuzuha == null)
        {
            FinishEvent();
        }
    }
    public override void EventEnd()
    {

    }

    public void OnThunderColliderEvent()
    {
        parent.SetCanBeStarted(true);
        parent.InitiationContact();
        
        StartCoroutine(ThunderEventAction(AYDState.Thunder, ()=>
        {
            currentState = AYDState.EnemyInstance;
        }));
    }

    public void OnInstanceAzYzColliderEvent()
    {
        StartCoroutine(ThunderEventAction(AYDState.EnemyInstance, () =>
        {
            currentState = AYDState.Chase;
            StageManager.Instance.Azuha = null;
            StageManager.Instance.Yuzuha = null;
            FinishEvent();
        }));
    }

    private IEnumerator ThunderEventAction(AYDState state, UnityAction onComplete)
    {
        switch (state)
        {
            case AYDState.Thunder:
                StageManager.Instance.Player.ChangeState(PlayerState.Event);
                yield return StartCoroutine(Thunder(state));
                boxCollider2.enabled = true;
                boxCollider1.enabled = false;
                StageManager.Instance.Player.ChangeState(PlayerState.Free);
                break;
            case AYDState.EnemyInstance:
                boxCollider2.enabled = false;
                StartCoroutine(Thunder(state));
                yield return new WaitForSeconds(0.3f);
                azuha.gameObject.SetActive(true);
                yuzuha.gameObject.SetActive(true);
                yield return null;
                azuha.ChangeState(EnemyState.ChasePlayer);
                yuzuha.ChangeState(EnemyState.ChasePlayer);
                yield return new WaitForSeconds(5f);
                azuha.ChangeState(EnemyState.CanNotAction);
                yuzuha.ChangeState(EnemyState.CanNotAction);
                azuha.gameObject.SetActive(false);
                yuzuha.gameObject.SetActive(false);
                break;
        }

        if(onComplete != null)
        {
            onComplete();
        }
    }

    private IEnumerator Thunder(AYDState state)
    {
        switch (state)
        {
            case AYDState.Thunder:
                thunderImage.color = Color.white;
                break;
            case AYDState.EnemyInstance:
                thunderImage.color = Color.red;
                break;
        }
        Color c = thunderImage.color;
        c.a = 0f;
        thunderImage.color = c;
        thunderImage.gameObject.SetActive(true);
        yield return StartCoroutine(FadeManager.Instance.FadeImage(thunderImage, FadeType.Out, 0.4f, 0.1f));
        soundPlayer.PlaySE(0);
        yield return new WaitForSeconds(0.7f);
        yield return StartCoroutine(FadeManager.Instance.FadeImage(thunderImage, FadeType.In, 0f, 1f));
        thunderImage.gameObject.SetActive(false);
    }
}
