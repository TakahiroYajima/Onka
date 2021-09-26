using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;

/// <summary>
/// ドアの施錠用オブジェクト
/// </summary>
[RequireComponent(typeof(Collider))]
public class DoorKeyLockObject : MonoBehaviour
{
    [SerializeField] private DoorObject doorObject = null;
    private Collider collider = null;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
        //if (DataManager.Instance.IsKeyUnlocked(doorObject.DoorOpenKey))
        //{
        //    collider.enabled = false;
        //}
    }

    public void DoUnlockDoorKey()
    {
        //DataManager.Instance.DoDoorUnlock(doorObject.DoorOpenKey,()=> 
        //{
        //    collider.enabled = false;
        //    SoundManager.Instance.PlaySeWithKey("menuse_key_unlock");
        //});
    }
}
