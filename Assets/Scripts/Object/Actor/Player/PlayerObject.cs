using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Raycastor))]
[RequireComponent(typeof(InRoomChecker))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerObject : MonoBehaviour
{
    [SerializeField] private FirstPersonAIO firstPersonAIO = null;
    public FirstPersonAIO FirstPersonAIO { get { return firstPersonAIO; } }
    [SerializeField] private GameObject cameraObj = null;
    public GameObject CameraObj { get { return cameraObj; } }
    public Raycastor raycastor { get; private set; } = null;
    public InRoomChecker inRoomChecker { get; private set; } = null;
    public Vector3 Position { get { return transform.position; } }
    public Vector3 eyePosition { get { return transform.position + new Vector3(0,0.7f,0); } }
    public Rigidbody rigidbody { get; private set; } = null;
    public MovingObject cameraMovingObject { get; private set; } = null;
    public CapsuleCollider capsuleCollider { get; private set; } = null;
    public float colliderHeightHalf { get { return capsuleCollider.height * 0.48f; } }//0.5だと完全に頂点になるため、若干下げる

    private Dictionary<PlayerState, StateBase> playerStateDic = new Dictionary<PlayerState, StateBase>();
    public PlayerState currentState { get; private set; } = PlayerState.Init;

    public UnityAction<PlayerState, PlayerState> onStateChangeCallback = null;
    public UnityAction onStateChangedInPlayerScriptOnly = null;//PlayerObjectのステートスクリプト専用のコールバック

    private List<Enemy> chasedEnemys = new List<Enemy>();
    public int chasedCount { get { return chasedEnemys.Count; } }//自分を追いかけている敵の数
    public void AddChasedCount(Enemy _enemy)
    {
        if (chasedCount == 0) { ChangeState(PlayerState.Chased); }
        if (!chasedEnemys.Contains(_enemy)) { chasedEnemys.Add(_enemy); }
        //Debug.Log("AddChasedCount current : " + chasedCount.ToString());
    }
    public void RemoveChasedCount(Enemy _enemy)
    {
        if (chasedEnemys.Contains(_enemy)) { chasedEnemys.Remove(_enemy); }
        //Debug.Log("RemoveChasedCount current : " + chasedCount.ToString()); OrganizeChasedEnemys();
        if (chasedCount == 0) { ChangeState(PlayerState.Free); }
    }
    private void OrganizeChasedEnemys() { foreach(Enemy e in chasedEnemys) { if(e == null) { chasedEnemys.Remove(e); } } }

    public bool isEventEnabled { get { return currentState == PlayerState.Init || currentState == PlayerState.Free; } }

    private void Awake()
    {
        raycastor = GetComponent<Raycastor>();
        inRoomChecker = GetComponent<InRoomChecker>();
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        cameraMovingObject = cameraObj.GetComponent<MovingObject>();

        //Stateパターン初期化
        playerStateDic.Clear();
        playerStateDic.Add(PlayerState.Init, new PlayerStateInit(this));
        playerStateDic.Add(PlayerState.Free, new PlayerStateFree(this));
        playerStateDic.Add(PlayerState.ItemGet, new PlayerStateItemGet(this));
        playerStateDic.Add(PlayerState.SolveKeylock, new PlayerStateKeylock(this));
        playerStateDic.Add(PlayerState.Chased, new PlayerStateChased(this));
        playerStateDic.Add(PlayerState.Arrested, new PlayerStateArrested(this));
        playerStateDic.Add(PlayerState.InMenu, new PlayerStateInMenu(this));
        playerStateDic.Add(PlayerState.Event, new PlayerStateEvent(this));

        currentState = PlayerState.Init;
    }

    // Start is called before the first frame update
    void Start()
    {
        //デバッグ用
        playerStateDic[currentState].StartAction();
    }

    // Update is called once per frame
    void Update()
    {
        playerStateDic[currentState].UpdateAction();
    }
    /// <summary>
    /// State切り替え
    /// </summary>
    /// <param name="nextState"></param>
    public void ChangeState(PlayerState nextState)
    {
        PlayerState prevState = currentState;
        playerStateDic[currentState].EndAction();
        currentState = nextState;
        playerStateDic[currentState].StartAction();
        if(onStateChangeCallback != null)
        {
            onStateChangeCallback(prevState, currentState);
        }
        if(onStateChangedInPlayerScriptOnly != null)
        {
            onStateChangedInPlayerScriptOnly();
        }
        //Debug.Log("PlayerState :: " + currentState.ToString());
    }

    public void TurnAroundCamera_Update(Vector3 targetPosition, float rotateSpeed)
    {
        cameraMovingObject.TurnAroundSmooth_Update(targetPosition, rotateSpeed);
    }

    public void ForcedStopFPS()
    {
        firstPersonAIO.enabled = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }
    /// <summary>
    /// プレイヤーの視界に入っているか
    /// </summary>
    /// <param name="judgePos"></param>
    /// <param name="searchAngle"></param>
    /// <returns></returns>
    public bool IsInSightFromPlayer(Vector3 judgePos, float searchAngle = 60f)
    {
        //視界に入っていたら検知
        var positionDiff = judgePos - transform.position;
        var angle = Vector3.Angle(transform.forward, positionDiff);
        return angle <= searchAngle;
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case Tags.Door:
                DoorObject doorObject = collision.gameObject.GetComponent<DoorObject>();
                if (doorObject != null)
                {
                    doorObject.OpenDoor();
                }
                break;
            case "CrouchingArea":
                //firstPersonAIO.isStandable = false;
                break;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == Tags.Door)
        {
            DoorObject doorObject = collision.gameObject.GetComponent<DoorObject>();
            if (doorObject != null)
            {
                doorObject.OpenDoor();
            }
        }
    }

    //private void OnCollisionExit(Collision collision)
    //{
    //    string tagName = collision.gameObject.tag;
    //    switch (tagName)
    //    {
    //        case "CrouchingArea":
    //            //firstPersonAIO.isStandable = true;
    //            break;
    //    }
    //}
}

public enum PlayerState
{
    Init,
    Free,
    InMenu,
    Event,
    ItemGet,
    SolveKeylock,//ナンバーロックを解いている時
    Chased,//敵に追われているとき
    Arrested,//敵に捕まった時
    GameOver,
}