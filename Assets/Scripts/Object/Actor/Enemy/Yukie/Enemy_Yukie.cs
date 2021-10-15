using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SoundDistance;

[RequireComponent(typeof(WanderingActor))]
[RequireComponent(typeof(Raycastor))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(InRoomChecker))]
public class Enemy_Yukie : Enemy
{
    public WanderingActor wanderingActor { get; private set; } = null;
    public InRoomWanderingActor inRoomWanderingActor { get; private set; } = null;
    public PlayerObject player { get; private set; } = null;
    public Raycastor raycastor { get; private set; } = null;
    public CapsuleCollider capsuleCollider { get; private set; } = null;
    public InRoomChecker inRoomChecker { get; private set; } = null;
    [SerializeField] private SoundPlayerObject emitterSoundPlayer = null;
    [SerializeField] private CapsuleCollider toPlayerWallCollider = null;//プレイヤーと一定の距離を保つためにあるコライダー
    public CapsuleCollider ToPlayerWallCollider { get { return toPlayerWallCollider; } }

    public Dictionary<EnemyState, StateBase> yukieStateDic { get; private set; } = new Dictionary<EnemyState, StateBase>();

    public UnityAction<EnemyState> onStateChangeCallback = null;
    public UnityAction<Collider> onColliderEnterCallback = null;
    public UnityAction<Collider> onColliderStayCallback = null;

    //プレイヤーとの距離計算用（Vector3では重いのでY軸を無視してVector2(x,z)に変換）
    [HideInInspector] public Vector2 yukieXZ = new Vector2(0, 0);
    [HideInInspector] public Vector2 playerXZ = new Vector2(0, 0);
    //プレイヤー検知用
    private float playerDetectionDistance = 8f;
    private float searchAngle = 60f;

    

    protected override void Awake()
    {
        base.Awake();
        wanderingActor = GetComponent<WanderingActor>();
        inRoomWanderingActor = GetComponent<InRoomWanderingActor>();
        player = StageManager.Instance.Player;
        raycastor = GetComponent<Raycastor>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        inRoomChecker = GetComponent<InRoomChecker>();

        //State登録
        yukieStateDic.Add(EnemyState.Wandering, new YukieStateWandering());
        yukieStateDic.Add(EnemyState.InRoomWandering, new YukieStateInRoomWandering());
        yukieStateDic.Add(EnemyState.RotateToPlayer, new YukieStateTurnAroundToPlayer());
        yukieStateDic.Add(EnemyState.RecognizedPlayer, new YukieStateRecognizedPlayer());
        yukieStateDic.Add(EnemyState.ChasePlayer, new YukieStateChasePlayer());
        yukieStateDic.Add(EnemyState.CaughtPlayer, new YukieStateCaughtPlayer());
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //コールバック登録
        inRoomChecker.onEnterRoomAction = OnEnterRoomAction;
        inRoomChecker.onExitRoomAction = OnExitRoomAction;

        inRoomWanderingActor.SetActive(false, null);
        StartCoroutine(StartAction());
    }

    private IEnumerator StartAction()
    {
        yield return null;//他のスクリプトの初期化待ち
        currentState = EnemyState.Wandering;//デバッグ
        yukieStateDic[currentState].StartAction();
        
        ChangeState(EnemyState.Wandering);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != EnemyState.Init)
        {
            yukieStateDic[currentState].UpdateAction();
        }
    }

    public void ChangeState(EnemyState nextState)
    {
        //Debug.Log("ChangeState : " + nextState);
        yukieStateDic[currentState].EndAction();
        currentState = nextState;
        yukieStateDic[currentState].StartAction();
        if(onStateChangeCallback != null)
        {
            onStateChangeCallback(currentState);
        }
    }

    /// <summary>
    /// プレイヤーの位置情報と自分の位置情報を更新（Vector2(x,z)変換）
    /// </summary>
    public void UpdatePositionXZ()
    {
        yukieXZ.x = transform.position.x;
        yukieXZ.y = transform.position.z;

        playerXZ.x = player.transform.position.x;
        playerXZ.y = player.transform.position.z;
    }
    /// <summary>
    /// プレイヤーが視界に入っているか
    /// </summary>
    public bool IsInSightPlayer()
    {
        if ((yukieXZ - playerXZ).sqrMagnitude < playerDetectionDistance * playerDetectionDistance)
        {
            //視界に入っていたら検知
            var positionDiff = player.transform.position - transform.position;
            var angle = Vector3.Angle(transform.forward, positionDiff);
            if (angle <= searchAngle)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 渡したものが視界に入っているか
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public bool IsInSight(GameObject target)
    {
        Vector2 targetXZ = new Vector2(target.transform.position.x, target.transform.position.z);
        if ((yukieXZ - targetXZ).sqrMagnitude < playerDetectionDistance * playerDetectionDistance)
        {
            //視界に入っていたら検知
            var positionDiff = target.transform.position - transform.position;
            var angle = Vector3.Angle(transform.forward, positionDiff);
            if (angle <= searchAngle)
            {
                return true;
            }
        }
        return false;
    }

    public void PlaySoundLoop(int arrayNum, float volume = 1f)
    {
        SoundDistanceManager.Instance.StartSoundDistanceMaker(emitterSoundPlayer.GetClip(arrayNum), volume);
    }
    public void SetMaxVolume(float _volume)
    {
        SoundDistanceManager.Instance.SetMaxVolumeToMaker(_volume);
    }
    public void StopSound()
    {
        SoundDistanceManager.Instance.StopSoundDistanceMaker();
    }

    private void OnEnterRoomAction(RoomWanderingManager _roomWanderingManager)
    {
        if (currentState != EnemyState.InRoomWandering)
        {
            inRoomWanderingActor.currentManager = _roomWanderingManager;
            ChangeState(EnemyState.InRoomWandering);
        }
    }

    private void OnExitRoomAction(RoomWanderingManager _roomWanderingManager)
    {
        if (currentState == EnemyState.InRoomWandering)
        {
            ChangeState(EnemyState.Wandering);
            inRoomWanderingActor.currentManager = null;
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(onColliderEnterCallback != null)
    //    {
    //        //onColliderEnterCallback(collision);
    //    }

    //}

    private void OnTriggerEnter(Collider other)
    {
        if (onColliderEnterCallback != null)
        {
            onColliderEnterCallback(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(onColliderStayCallback != null)
        {
            onColliderStayCallback(other);
        }
    }
}
