using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ターゲットが一定時間自分の視野外且つすぐ近くでうろついていたら気づくようにするシステム（対プレイヤー）
/// </summary>
[RequireComponent(typeof(Raycastor))]
public class ProvokedSystem : MonoBehaviour
{
    private UnityAction onNoticedAction = null;
    private Raycastor raycastor = null;

    //ターゲットが自分の視界の範囲外をうろつく＝挑発しているので、それに対する不意打ち要素
    private float ProvokedJudgeDistance = 8f;//8メートル以内なら煽りと判断
    private float NoticeProvocationTime = 7f;//挑発に気付くまでの時間
    [HideInInspector] public float provocationingTime = 0f;//ターゲットに挑発されている時間
    [HideInInspector] public Transform rayStartPointTransform = null;//Rayを発射する地点

    //private UnityAction PreciseSerachedPlayer_True = null;

    public void Initialize(float _noticeProvocationTime, UnityAction _onNoticed, Transform _rayStartPointTransform = null)
    {
        NoticeProvocationTime = _noticeProvocationTime;
        onNoticedAction = _onNoticed;
        provocationingTime = 0f;
        if(_rayStartPointTransform == null)
        {
            rayStartPointTransform = transform;
        }
        else
        {
            rayStartPointTransform = _rayStartPointTransform;
        }
    }

    void Awake()
    {
        raycastor = GetComponent<Raycastor>();
        //PreciseSerachedPlayer_True = () =>
        //{
        //    provocationingTime += Time.deltaTime * Enemy_Yukie.doUpdateFrameCount;
        //    if (provocationingTime >= NoticeProvocationTime)
        //    {
        //        //さんざん煽られた or 部屋内徘徊中にプレイヤーを察知したので何かする（不意打ち要素・実況者殺し）
        //        onNoticedAction();
        //        provocationingTime = 0f;
        //    }
        //};
    }

    /// <summary>
    /// 挑発検知（Rayは重いので絶対に6フレームに1回呼ぶこと。）
    /// </summary>
    /// <param name="targetPosition"></param>
    public void ProvokedUpdate_ToPlayer_6Frame(Vector3 targetPosition)
    {
        //if(frameCount < doUpdateFrameCount)
        //{
        //    frameCount++;
        //    return;
        //}
        //frameCount = 0;

        raycastor.ObjectToRayAction(rayStartPointTransform.position, targetPosition, (RaycastHit hit) =>
        {
            if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, Tags.Player))
            {
                //距離が近いほどすぐ気づくようにする
                float distance = (hit.transform.position - transform.position).sqrMagnitude;
                if(distance < 0.1f)
                {
                    distance = 0.1f;//0対策（念のため）
                }
                float rate = distance / (ProvokedJudgeDistance * ProvokedJudgeDistance * 0.8f);//2乗だけでは絶対に少数になるので若干下げる
                if(rate < 0.2f)
                {
                    rate = 0f;//遠すぎる時は通常の時間
                }
                rate = Mathf.Clamp01(rate);
                float addTime = Time.deltaTime * Enemy_Yukie.DoUpdateFrameCount;
                addTime += addTime * (1 - rate);//真後ろで2倍近く早く気付く
                
                provocationingTime += addTime;
                if (provocationingTime >= NoticeProvocationTime)
                {
                    //さんざん煽られた or 部屋内徘徊中にプレイヤーを察知したので何かする（不意打ち要素・実況者殺し）
                    onNoticedAction();
                    provocationingTime = 0f;
                }
            }
            else
            {
                provocationingTime -= Time.deltaTime * Enemy_Yukie.DoUpdateFrameCount * 0.2f;//加算と同じカウントだと中々振り返らないので減衰を入れる
                if (provocationingTime < 0f) { provocationingTime = 0f; }
            }
        }, ProvokedJudgeDistance);
    }

    //public void ProvokedUpdate_ToPlayer_Vertical_6Frame(Vector3 targetPosition)
    //{
    //    raycastor.ObjectToRayAction(rayStartPointTransform.position, targetPosition, (RaycastHit hit) =>
    //    {
    //        if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, Tags.Player))
    //        {
    //            provocationingTime += Time.deltaTime * Enemy_Yukie.doUpdateFrameCount;
    //            if (provocationingTime >= NoticeProvocationTime)
    //            {
    //                //さんざん煽られた or 部屋内徘徊中にプレイヤーを察知したので何かする（不意打ち要素・実況者殺し）
    //                onNoticedAction();
    //                provocationingTime = 0f;
    //            }
    //        }
    //        else
    //        {
    //            provocationingTime -= Time.deltaTime * Enemy_Yukie.doUpdateFrameCount;
    //            if (provocationingTime < 0f) { provocationingTime = 0f; }
    //        }
    //    }, 6f);//6メートル以内なら煽りと判断
    //}

    /// <summary>
    /// さらに精密にプレイヤーを察知させる
    /// </summary>
    /// <param name="targetPosition"></param>
    //public void ProvokedUpdateToPlayer_Precise(Vector3 targetPosition)
    //{
    //    //targetPositionだと中心だけになるので、若干ずらしてコライダーがはみ出ているところまで検知させる
    //    if(raycastor.IsRaycastHitObjectMatch(transform.position, targetPosition, Tags.Player, 6f))
    //    {
    //        PreciseSerachedPlayer_True();
    //    }
    //    else
    //    {
    //        targetPosition.x += 0.1f;
    //        if (raycastor.IsRaycastHitObjectMatch(transform.position, targetPosition, Tags.Player, 6f))
    //        {
    //            PreciseSerachedPlayer_True();
    //        }
    //        else
    //        {
    //            targetPosition.x -= 0.2f;
    //            if (raycastor.IsRaycastHitObjectMatch(transform.position, targetPosition, Tags.Player, 6f))
    //            {
    //                PreciseSerachedPlayer_True();
    //            }
    //            else
    //            {
    //                targetPosition.x += 0.1f;
    //                targetPosition.z += 0.1f;
    //                if (raycastor.IsRaycastHitObjectMatch(transform.position, targetPosition, Tags.Player, 6f))
    //                {
    //                    PreciseSerachedPlayer_True();
    //                }
    //                else
    //                {
    //                    targetPosition.z -= 0.2f;
    //                    if (raycastor.IsRaycastHitObjectMatch(transform.position, targetPosition, Tags.Player, 6f))
    //                    {
    //                        PreciseSerachedPlayer_True();
    //                    }
    //                    else
    //                    {
    //                        provocationingTime -= Time.deltaTime * Enemy_Yukie.doUpdateFrameCount;
    //                        if (provocationingTime < 0f) { provocationingTime = 0f; }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
}
