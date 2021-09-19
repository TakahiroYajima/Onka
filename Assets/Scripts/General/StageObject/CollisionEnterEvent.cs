using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEnterEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent unityEvent = null;
    public Collision HitCollision = null;

    private void OnCollisionEnter(Collision collision)
    {
        HitCollision = collision;
        if (unityEvent != null)
        {
            unityEvent.Invoke();
        }
    }
}
