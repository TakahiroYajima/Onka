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
    private float NoticeProvocationTime = 10f;//挑発に気付くまでの時間
    public float provocationingTime = 0f;//ターゲットに挑発されている時間

    private UnityAction PreciseSerachedPlayer_True = null;

    public void Initialize(float _noticeProvocationTime, UnityAction _onNoticed)
    {
        NoticeProvocationTime = _noticeProvocationTime;
        onNoticedAction = _onNoticed;
        provocationingTime = 0f;
    }

    void Awake()
    {
        raycastor = GetComponent<Raycastor>();
        PreciseSerachedPlayer_True = () =>
        {
            provocationingTime += Time.deltaTime * Enemy_Yukie.doUpdateFrameCount;
            if (provocationingTime >= NoticeProvocationTime)
            {
                //さんざん煽られた or 部屋内徘徊中にプレイヤーを察知したので何かする（不意打ち要素・実況者殺し）
                onNoticedAction();
                provocationingTime = 0f;
            }
        };
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

        raycastor.ObjectToRayAction(transform.position, targetPosition, (RaycastHit hit) =>
        {
            if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, Tags.Player))
            {
                provocationingTime += Time.deltaTime * Enemy_Yukie.doUpdateFrameCount;
                if (provocationingTime >= NoticeProvocationTime)
                {
                    //さんざん煽られた or 部屋内徘徊中にプレイヤーを察知したので何かする（不意打ち要素・実況者殺し）
                    onNoticedAction();
                    provocationingTime = 0f;
                }
            }
            else
            {
                provocationingTime -= Time.deltaTime * Enemy_Yukie.doUpdateFrameCount;
                if (provocationingTime < 0f) { provocationingTime = 0f; }
            }
        }, 6f);//6メートル以内なら煽りと判断
    }

    public void ProvokedUpdate_ToPlayer_Vertical_6Frame(Vector3 targetPosition)
    {
        raycastor.ObjectToRayAction(transform.position, targetPosition, (RaycastHit hit) =>
        {
            if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, Tags.Player))
            {
                provocationingTime += Time.deltaTime * Enemy_Yukie.doUpdateFrameCount;
                if (provocationingTime >= NoticeProvocationTime)
                {
                    //さんざん煽られた or 部屋内徘徊中にプレイヤーを察知したので何かする（不意打ち要素・実況者殺し）
                    onNoticedAction();
                    provocationingTime = 0f;
                }
            }
            else
            {
                provocationingTime -= Time.deltaTime * Enemy_Yukie.doUpdateFrameCount;
                if (provocationingTime < 0f) { provocationingTime = 0f; }
            }
        }, 6f);//6メートル以内なら煽りと判断
    }

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
