using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 孝蔵の日記3を取得した後のイベント
/// </summary>
public class Event_AfterGetKozoDiary3 : EventBase
{
    [SerializeField] private BoxCollider boxCollider = null;
    [SerializeField] private CollisionEnterEvent collisionEnterEvent = null;
    [SerializeField] private SoundPlayerObject soundPlayer = null;

    //地下にあるキメラみたいな像を動かす。音も鳴らす（ビビらせ要員）
    [SerializeField] private Transform moveObject = null;

    private void Awake()
    {
        boxCollider.enabled = false;
    }
    public override void ProgressEvent()
    {
        if (!EventManager.Instance.IsEventEnded(eventKey))
        {
            if (DataManager.Instance.GetItemData("diary_kozo_3").geted)
            {
                boxCollider.enabled = true;
                Debug.Log("孝蔵の日記3取得後イベント解放");
            }
        }
    }

    public override void EventStart()
    {
        Debug.Log("孝蔵の日記3取得後イベント");
        soundPlayer.PlaySE("se_lug");
        StartCoroutine(MoveStatus(()=>
        {
            EventManager.Instance.EventClear(EventKey);
        }));
    }
    public override void EventUpdate()
    {
        
    }
    public override void EventEnd()
    {
        Debug.Log("孝蔵の日記3取得後イベント終了");
        boxCollider.enabled = false;
    }

    private IEnumerator MoveStatus(UnityAction onComplete)
    {
        float currentTime = 0f;
        while(currentTime < 0.5)
        {
            moveObject.Rotate(0, 45 * (Time.deltaTime / 0.5f), 0);
            currentTime += Time.deltaTime;
            yield return null;
        }
        moveObject.rotation = Quaternion.Euler(0, 45, 0);
        onComplete();
    }

    public void OnCollisionEnterEvent()
    {
        if (Utility.Instance.IsTagNameMatch(collisionEnterEvent.HitCollision.gameObject, Tags.Player))
        {
            EventManager.Instance.EventStart(managementID);
        }
    }
}
