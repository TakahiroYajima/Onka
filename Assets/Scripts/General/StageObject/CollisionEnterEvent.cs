using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEnterEvent : MonoBehaviour
{
    [SerializeField] private UnityEvent unityEvent = null;
    [SerializeField] private UnityEvent exitEvent = null;
    [SerializeField] private UnityEvent onTriggerEnterEvent = null;
    [SerializeField] private UnityEvent onTriggerExitEvent = null;
    [HideInInspector] public Collision HitCollision = null;
    [HideInInspector] public Collider HitCollider = null;

    private void OnCollisionEnter(Collision collision)
    {
        HitCollision = collision;
        if (unityEvent != null)
        {
            unityEvent.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HitCollider = other;
        if (onTriggerEnterEvent != null)
        {
            onTriggerEnterEvent.Invoke();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        HitCollision = collision;
        if (exitEvent != null)
        {
            exitEvent.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HitCollider = other;
        if (onTriggerExitEvent != null)
        {
            onTriggerExitEvent.Invoke();
        }
    }
}
