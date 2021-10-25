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
    public Raycastor raycastor { get; private set; } = null;
    public InRoomChecker inRoomChecker { get; private set; } = null;
    public Vector3 Position { get { return transform.position; } }
    public Rigidbody rigidbody { get; private set; } = null;
    private CapsuleCollider capsuleCollider = null;
    public float colliderHeightHalf { get { return capsuleCollider.height * 0.48f; } }//0.5だと完全に頂点になるため、若干下げる

    private Dictionary<PlayerState, StateBase> playerStateDic = new Dictionary<PlayerState, StateBase>();
    public PlayerState currentState { get; private set; } = PlayerState.Free;

    public UnityAction<PlayerState, PlayerState> onStateChangeCallback = null;
    public UnityAction onStateChangedInPlayerScriptOnly = null;//PlayerObjectのステートスクリプト専用のコールバック

    public int chasedCount { get; private set; } = 0;//自分を追いかけている敵の数
    public void AddChasedCount() { if (chasedCount == 0) { ChangeState(PlayerState.Chased); } chasedCount++; }
    public void RemoveChasedCount() { chasedCount--; if(chasedCount < 0) { chasedCount = 0; } if (chasedCount == 0) { ChangeState(PlayerState.Free); } }

    private void Awake()
    {
        raycastor = GetComponent<Raycastor>();
        inRoomChecker = GetComponent<InRoomChecker>();
        rigidbody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        //Stateパターン初期化
        playerStateDic.Add(PlayerState.Free, new PlayerStateFree());
        playerStateDic.Add(PlayerState.ItemGet, new PlayerStateItemGet());
        playerStateDic.Add(PlayerState.Chased, new PlayerStateChased());
        playerStateDic.Add(PlayerState.InMenu, new PlayerStateInMenu());
        //playerStateDic.Add(PlayerState.Event, new PlayerStateWatchItem());
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        string tagName = collision.gameObject.tag;
        switch (tagName)
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

    private void OnCollisionExit(Collision collision)
    {
        string tagName = collision.gameObject.tag;
        switch (tagName)
        {
            case "CrouchingArea":
                //firstPersonAIO.isStandable = true;
                break;
        }
    }
}

public enum PlayerState
{
    Free,
    InMenu,
    Event,
    ItemGet,
    Chased,//敵に追われているとき
}