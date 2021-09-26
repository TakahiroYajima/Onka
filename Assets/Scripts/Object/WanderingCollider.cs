using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 徘徊アクター通過地点の当たり判定
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public class WanderingCollider : MonoBehaviour
{
    public int ID { get; private set; }
    public void SetID(int _id)
    {
        ID = _id;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case Tags.Enemy:
                StageManager.Instance.Yukie.wanderingActor.SetWanderingID(ID + 1);
                break;
        }
    }
}
