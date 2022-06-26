using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EA_AfterGetNobuyukiDiary3 : EventActorBase
{
    [SerializeField] private BoxCollider boxCollider = null;
    [SerializeField] private CollisionEnterEvent collisionEnterEvent = null;
    [SerializeField] private MovingObject nobuyukiObj = null;

    protected override void Initialize()
    {
        boxCollider.enabled = true;
        nobuyukiObj.gameObject.SetActive(false);
        parent.SetCanBeStarted(false);
    }

    public override void EventStart()
    {
        NobuyukiEvent(() =>
        {
            parent.EventClearContact();
        });
    }
    public override void EventUpdate()
    {

    }
    public override void EventEnd()
    {

    }

    private void NobuyukiEvent(UnityAction onComplete)
    {
        StageManager.Instance.Player.raycastor.ObjectToRayAction(StageManager.Instance.Player.transform.position, StageManager.Instance.Player.transform.position + StageManager.Instance.Player.transform.forward, (RaycastHit hit) =>
        {
            Vector3 hitPoint = hit.point;
            hitPoint.y = nobuyukiObj.transform.position.y;
            nobuyukiObj.transform.position = hitPoint;
            nobuyukiObj.gameObject.SetActive(true);
            StartCoroutine(MovePanel(onComplete));

        }, 20f);
    }

    private IEnumerator MovePanel(UnityAction onComplete)
    {
        yield return new WaitForEndOfFrame();
        //プレイヤーのカメラの向きの先に信之生成（位置調整だけ）
        Vector3 playerPos = StageManager.Instance.Player.transform.position;
        playerPos.y = nobuyukiObj.transform.position.y;
        Vector3 toPlayerNormal;
        while ((nobuyukiObj.transform.position - playerPos).sqrMagnitude > 2f)
        {
            playerPos = StageManager.Instance.Player.transform.position;
            toPlayerNormal = (playerPos - nobuyukiObj.transform.position).normalized;
            nobuyukiObj.MoveToTargetDir_Update(toPlayerNormal, 9f);
            yield return null;
        }
        //プレイヤーに向かって信之が迫る
        //プレイヤーの目の前に来たら消える
        nobuyukiObj.gameObject.SetActive(false);
        onComplete();
    }

    public void OnCollisionEnterEvent()
    {
        if (Utility.Instance.IsTagNameMatch(collisionEnterEvent.HitCollision.gameObject, Tags.Player))
        {
            parent.SetCanBeStarted(true);
            parent.InitiationContact();
        }
    }
}
