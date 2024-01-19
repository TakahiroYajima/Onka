using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EventBase�������Ă����X�N���v�g���瑀��ł���悤�ɋ��ʉ�������SeeThePast�̃R���g���[��
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
