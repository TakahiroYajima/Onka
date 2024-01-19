using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EventBaseが無くても他スクリプトから操作できるように共通化させたSeeThePastのコントローラ
/// </summary>
public class SeeThePastController : MonoBehaviour
{
    [SerializeField]
    private string yukieSetPositionSDPKey = "sd_point_outer_0000";
    [SerializeField]
    private string yukieSetTargetWanderingPointKey = "wandering_point_yukie_0000";

    public void OnActive()
    {
        StageManager.Instance.SetYukieActive(false);
    }

    public void OnFinish()
    {
        StageManager.Instance.SetYukieActive(true);
        StageManager.Instance.ForceOperationYukiePositionWithSDP(yukieSetPositionSDPKey, yukieSetTargetWanderingPointKey);
    }
}
