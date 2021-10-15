using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Event_AfterGetNobuyukiDiary3 : EventBase
{
    [SerializeField] private BoxCollider boxCollider = null;
    [SerializeField] private CollisionEnterEvent collisionEnterEvent = null;

    private void Start()
    {
        boxCollider.gameObject.SetActive(false);
    }

    public override void ProgressEvent()
    {
        if (!EventManager.Instance.IsEventEnded(eventKey))
        {
            if (DataManager.Instance.GetItemData("diary_nobuyuki_3").geted)
            {
                boxCollider.gameObject.SetActive(true);
                Debug.Log("信之の日記3取得後イベント解放");
            }
        }
    }

    public override void EventStart()
    {
        Debug.Log("信之の日記3取得後イベント");
        //soundPlayer.PlaySE("se_lug");
        StartCoroutine(MovePanel(() =>
        {
            EventManager.Instance.EventClear(EventKey);
        }));
    }
    public override void EventUpdate()
    {

    }
    public override void EventEnd()
    {
        Debug.Log("信之の日記3取得後イベント終了");
        boxCollider.gameObject.SetActive(false);
    }

    private IEnumerator MovePanel(UnityAction onComplete)
    {
        //プレイヤーのカメラの向きの先に信之生成（位置調整だけ）
        //プレイヤーに向かって信之が迫る
        //プレイヤーの目の前に来たら消える
        yield return null;
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
