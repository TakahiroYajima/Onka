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
    protected const float DistanceRequiredToCloseDoorSqrMagnitude = 100f;//�����Ńh�A���܂邽�߂ɕK�v�ȃv���C���[�Ƃ̋����F������10�BsqrMagnitude�̌v�Z�Ȃ̂� 10 * 10 = 100

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
            if ((transform.position - StageManager.Instance.GetPlayer().Position).sqrMagnitude >= DistanceRequiredToCloseDoorSqrMagnitude)
            {
                break;
            }
        }
        CloseDoor();
    }
}
