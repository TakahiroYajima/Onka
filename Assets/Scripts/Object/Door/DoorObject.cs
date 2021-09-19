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
    public bool isUnlocked { get { return DataManager.Instance.IsDoorKeyUnlocked(openKey); } }//�ߋ��Ƀv���C���[���h�A���J���Ă��邩�iItem��isUsed�Ŕ���j
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
            if (!isUnlocked)
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
        //�v���C���[���h�A���痣��Ă��銎�v���C���[�Ɛ�G�������̊O�ɂ��鎞�Ƀh�A��߂�i��G��NavMesh���r�؂�Ă��܂����߁A���̑΍�j
        while (true)
        {
            yield return new WaitForSeconds(3f);
            if ((transform.position - StageManager.Instance.GetPlayer().Position).sqrMagnitude >= DistanceRequiredToCloseDoorSqrMagnitude &&
                 !StageManager.Instance.GetPlayer().inRoomChecker.isEnterRoom && !StageManager.Instance.GetYukie().inRoomChecker.isEnterRoom)
            {
                break;
            }
        }
        CloseDoor();
    }
}
