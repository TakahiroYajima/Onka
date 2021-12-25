using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(BoxCollider))]
public abstract class DoorObject : MonoBehaviour
{
    //�h�A���J���邽�߂ɕK�v�ȃJ�M��ID
    [SerializeField] protected string openKey = "";
    //public string DoorOpenKey { get { return openKey; } }
    [SerializeField] protected KeyHoleTarget keyHoleTarget = null;
    [SerializeField] protected KeyLockTarget keyLockTarget = null;

    protected BoxCollider thisCollider = null;

    protected bool isMoving = false;
    protected bool isOpenState = false;//�h�A���J���Ă����Ԃ�
    public bool isForceOpenable = false;//�C�x���g�Ȃǂŋ����I�Ƀh�A���J������悤�ɂ��邩

    bool isKeyHoleUnlocked = true;
    bool isKeyLockUnlocked = true;

    //�ߋ��Ƀv���C���[���h�A���J���Ă��邩�iItem��isUsed�Ŕ���j
    public bool isUnlocked
    {
        get {
            if (keyHoleTarget == null && keyLockTarget == null) return true;
            else if (keyHoleTarget != null && keyLockTarget == null) return keyHoleTarget.isUnlocked;
            else if (keyLockTarget != null && keyHoleTarget == null) return keyLockTarget.isUnlocked;
            else return keyHoleTarget.isUnlocked && keyLockTarget.isUnlocked;
        }
    }
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
        if (isForceOpenable)
        {
            OpenAction();
            return;
        }

        isKeyHoleUnlocked = true;
        isKeyLockUnlocked = true;
        if (keyHoleTarget != null)
        {
            if (string.IsNullOrEmpty(keyHoleTarget.UnlockKey)) { Debug.LogError("�����L�[���ݒ肳��Ă��܂���"); return; }
            if (!keyHoleTarget.isUnlocked) {
                Debug.Log("�����������Ă��� : " + openKey);
                isKeyHoleUnlocked = false;
            }
            else
            {
                Debug.Log("�����ς� : " + openKey);
            }
        }
        if(keyLockTarget != null)
        {
            if (string.IsNullOrEmpty(keyLockTarget.UnlockTargetKey)) { Debug.LogError("�����L�[���ݒ肳��Ă��܂���"); return; }
            if (!keyLockTarget.isUnlocked)
            {
                Debug.Log("�����������Ă��� : " + openKey);
                isKeyLockUnlocked = false;
            }
        }
        if (isKeyHoleUnlocked && isKeyLockUnlocked)
        {
            OpenAction();
        }
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
            if ((transform.position - StageManager.Instance.Player.Position).sqrMagnitude >= DistanceRequiredToCloseDoorSqrMagnitude &&
                 !StageManager.Instance.Player.inRoomChecker.isEnterRoom && !StageManager.Instance.Yukie.inRoomChecker.isEnterRoom &&
                 isOpenState)
            {
                break;
            }
        }
        //�����Ńh�A��߂鏈���΍�̂��߁A��U�m�F������
        if (isOpenState)
        {
            CloseDoor();
        }
    }
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(DoorObject))]
//public class DoorObjectInspector : Editor
//{
//    public bool isUseKeyLock = false;

//    public override void OnInspectorGUI()
//    {
//        DoorObject door = target as DoorObject;

//        EditorGUILayout.BeginHorizontal();
//        {
//            isUseKeyLock = EditorGUILayout.Toggle("���V�X�e�����g�p���邩", isUseKeyLock, GUILayout.Width(120));
//        }
//        EditorGUILayout.EndHorizontal();
//        EditorGUILayout.LabelField("���V�X�e�����g�p���邩");

//    }
//}
//#endif