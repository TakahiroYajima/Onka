using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StageObjectBase : MonoBehaviour
{
    protected Collider thisCollider = null;
    // Start is called before the first frame update
    void Start()
    {
        thisCollider = GetComponent<Collider>();
    }

    /// <summary>
    /// タップした時の動作
    /// </summary>
    public abstract void OnTapObject();
}
