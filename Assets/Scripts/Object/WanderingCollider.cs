using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 徘徊アクター通過地点の当たり判定
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class WanderingCollider : MonoBehaviour
{
    

    public UnityAction onArrivaledEvent = null;

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case Tags.Enemy:
                if(onArrivaledEvent != null)
                {
                    onArrivaledEvent();
                }
                break;
        }
    }
}

