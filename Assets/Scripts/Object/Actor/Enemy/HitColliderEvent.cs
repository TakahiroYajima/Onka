using System;
using UnityEngine;

public class HitColliderEvent : MonoBehaviour
{
    public Action<Collider> OnEnter = null;
    public Action<Collider> OnStay = null;

    private void OnTriggerEnter(Collider other)
    {
        if (OnEnter != null)
        {
            OnEnter(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (OnStay != null)
        {
            OnStay(other);
        }
    }
}
