using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Onka.Manager.Data;

[RequireComponent(typeof(Collider))]
public abstract class KeyLockObject : MonoBehaviour
{
    [SerializeField] protected bool isAfterClearEnactive = false;//クリア後に非表示にさせるか。falseの場合はコライダーのみオフになる
    protected List<KeyLockTarget> keyLockTargetList = new List<KeyLockTarget>();
    private Collider collider = null;
    protected Vector3 initPosition;
    protected Quaternion initRotation;
    public float distanceFromCamera = 0.1f;//カメラの前に表示する時の適切な距離
    public Vector3 changeScaleInEvent = Vector3.one;//解錠イベント中、カメラに収まるように変化させるスケール
    public Vector3 initScale { get; private set; } = Vector3.one;
    //private List<GameObject> rendererObjectsChildList = new List<GameObject>();
    private GameObject childObject = null;

    protected virtual void Awake()
    {
        collider = GetComponent<Collider>();
        initPosition = transform.position;
        initRotation = transform.rotation;
        childObject = transform.GetChild(0).gameObject;//子オブジェクトは1つの想定。さらにオブジェクトがある場合は、その子オブジェクトとして生成させる必要がある
        initScale = transform.localScale;
    }

    public void SetInitialize(KeyLockTarget _keyLockTarget)
    {
        keyLockTargetList.Add(_keyLockTarget);
        //設計ミスにより、複数扉を開けられるキーロック機能は設定されるたびにいちいち初期化し直さないといけない
        collider.enabled = false;
        DoEnactive();
        for (int i = 0; i < keyLockTargetList.Count; i++)
        {
            //ひとまず、最初は非表示→何か未解決な要素があったら表示させるという、めんどくさい仕様に…
            if (!DataManager.Instance.IsKeyUnlocked(_keyLockTarget.UnlockTargetKey))
            {
                collider.enabled = true;
                childObject.SetActive(true);
            }
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
        transform.localScale = initScale;
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
            //gameObject.SetActive(false);
            childObject.SetActive(false);
        }
    }
}
