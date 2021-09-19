using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Raycastor))]
[RequireComponent(typeof(InRoomChecker))]
public class PlayerObject : MonoBehaviour
{
    [SerializeField] private FirstPersonAIO firstPersonAIO = null;
    public FirstPersonAIO FirstPersonAIO { get { return firstPersonAIO; } }
    public Raycastor raycastor { get; private set; } = null;
    public InRoomChecker inRoomChecker { get; private set; } = null;
    public Vector3 Position { get { return transform.position; } }

    private Dictionary<PlayerState, StateBase> playerStateDic = new Dictionary<PlayerState, StateBase>();
    public PlayerState currentState { get; private set; } = PlayerState.Free;

    public UnityAction<PlayerState> onStateChangeCallback = null;
    
    private void Awake()
    {
        raycastor = GetComponent<Raycastor>();
        inRoomChecker = GetComponent<InRoomChecker>();

        //Stateパターン初期化
        playerStateDic.Add(PlayerState.Free, new PlayerStateFree());
        playerStateDic.Add(PlayerState.ItemGet, new PlayerStateItemGet());
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
        playerStateDic[currentState].EndAction();
        currentState = nextState;
        playerStateDic[currentState].StartAction();
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
        //if (Utility.Instance.IsTagNameMatch(collision.transform, "Door"))
        //{
        //    DoorObject doorObject = collision.gameObject.GetComponent<DoorObject>();
        //    if(doorObject != null)
        //    {
        //        doorObject.OpenDoor();
        //    }
        //}
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
    Event,
    ItemGet,
    Chased,//敵に追われているとき
}