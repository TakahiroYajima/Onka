using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class KeyLockObject : MonoBehaviour
{
    [SerializeField] protected bool isAfterClearEnactive = false;//クリア後に非表示にさせるか。falseの場合はコライダーのみオフになる
    protected KeyLockTarget keyLockTarget = null;
    private Collider collider = null;
    protected Vector3 initPosition;
    protected Quaternion initRotation;
    public float distanceFromCamera = 0.1f;//カメラの前に表示する時の適切な距離

    protected virtual void Awake()
    {
        collider = GetComponent<Collider>();
        initPosition = transform.position;
        initRotation = transform.rotation;
    }

    public void SetInitialize(KeyLockTarget _keyLockTarget)
    {
        keyLockTarget = _keyLockTarget;
        if (DataManager.Instance.IsKeyUnlocked(keyLockTarget.UnlockTargetKey))
        {
            collider.enabled = false;
            DoEnactive();
        }
    }

    public virtual void TapObject()
    {
        collider.enabled = false;
        SolveKeylockManager.Instance.StartSolveEvent(this);
    }
    
    public void RemoveInitPos()
    {
        transform.position = initPosition;
        transform.rotation = initRotation;
    }
    public void RemoveInitState()
    {
        RemoveInitPos();
        collider.enabled = true;
    }

    public void DoEnactive()
    {
        if (isAfterClearEnactive)
        {
            gameObject.SetActive(false);
        }
    }
}
