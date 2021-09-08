using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public abstract class DoorObject : MonoBehaviour
{
    //ドアを開けるために必要なカギのID
    [SerializeField] protected string openKey = "";
    public string DoorOpenKey { get { return openKey; } }

    protected BoxCollider thisCollider = null;

    protected bool isMoving = false;
    protected bool isOpenState = false;//ドアが開いている状態か
    // Start is called before the first frame update
    void Start()
    {
        thisCollider = GetComponent<BoxCollider>();
        if(thisCollider == null)
        {
            thisCollider = gameObject.AddComponent<BoxCollider>();
            Debug.Log("ドアのコライダーがありませんでした");
        }
    }

    public void OpenDoor()
    {
        if (!string.IsNullOrEmpty(openKey))
        {
            if (!DataManager.Instance.IsDoorKeyUnlocked(openKey))
            {
                Debug.Log("鍵がかかっている : " + openKey);
                return;
            }
        }
        
        OpenAction();
    }
    /// <summary>
    /// 実際にドアを開ける
    /// </summary>
    protected abstract void OpenAction();

    public abstract void CloseDoor();
    
    protected IEnumerator DoorCloseWithWateTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            if (Vector3.Distance(transform.position, StageManager.Instance.GetPlayer().Position) >= 10f)
            {
                break;
            }
        }
        CloseDoor();
    }
}
