using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public abstract class DoorObject : MonoBehaviour
{
    //�h�A���J���邽�߂ɕK�v�ȃJ�M��ID
    [SerializeField] protected string openKey = "";
    public string DoorOpenKey { get { return openKey; } }

    protected BoxCollider thisCollider = null;

    protected bool isMoving = false;
    protected bool isOpenState = false;//�h�A���J���Ă����Ԃ�
    // Start is called before the first frame update
    void Start()
    {
        thisCollider = GetComponent<BoxCollider>();
        if(thisCollider == null)
        {
            thisCollider = gameObject.AddComponent<BoxCollider>();
            Debug.Log("�h�A�̃R���C�_�[������܂���ł���");
        }
    }

    public void OpenDoor()
    {
        if (!string.IsNullOrEmpty(openKey))
        {
            if (!DataManager.Instance.IsDoorKeyUnlocked(openKey))
            {
                Debug.Log("�����������Ă��� : " + openKey);
                return;
            }
        }
        
        OpenAction();
    }
    /// <summary>
    /// ���ۂɃh�A���J����
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
