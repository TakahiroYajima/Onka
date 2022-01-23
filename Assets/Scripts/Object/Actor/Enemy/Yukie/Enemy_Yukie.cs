using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SoundDistance;

[RequireComponent(typeof(WanderingActor))]
[RequireComponent(typeof(Raycastor))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(InRoomChecker))]
[RequireComponent(typeof(ProvokedSystem))]
public class Enemy_Yukie : Enemy
{
    public WanderingActor wanderingActor { get; private set; } = null;
    public InRoomWanderingActor inRoomWanderingActor { get; private set; } = null;
    public PlayerObject player { get; private set; } = null;
    public Raycastor raycastor { get; private set; } = null;
    public CapsuleCollider capsuleCollider { get; private set; } = null;
    public InRoomChecker inRoomChecker { get; private set; } = null;
    public ProvokedSystem provokedSystem { get; private set; } = null;
    public MovingObject movingObject { get; private set; } = null;
    public Rigidbody rigidbody { get; private set; } = null;
    [SerializeField] private SoundPlayerObject emitterSoundPlayer = null;
    [SerializeField] private CapsuleCollider toPlayerWallCollider = null;//プレイヤーと一定の距離を保つためにあるコライダー
    public CapsuleCollider ToPlayerWallCollider { get { return toPlayerWallCollider; } }
    [SerializeField] private Transform faceTransform = null;
    public Transform FaceTransform { get { return faceTransform; } }

    public Dictionary<EnemyState, StateBase> yukieStateDic { get; private set; } = new Dictionary<EnemyState, StateBase>();

    public UnityAction<EnemyState> onStateChangeCallback = null;
    public UnityAction<Collider> onColliderEnterCallback = null;
    public UnityAction<Collider> onColliderStayCallback = null;

    //プレイヤーとの距離計算用（Vector3では重いのでY軸を無視してVector2(x,z)に変換）
    private Vector2 yukieXZ = new Vector2(0, 0);
    private Vector2 playerXZ = new Vector2(0, 0);
    //プレイヤー検知用
    private float playerDetectionDistance = 8f;
    private float searchAngle = 60f;
    private float rotationSpeed = 3f;
    public const int doUpdateFrameCount = 6;
    //プレイヤーが自分の視界の範囲外をうろつく＝挑発しているので、それに対する不意打ち要素
    private const float NoticeProvocationTime = 10f;//挑発に気付くまでの時間
    private float provocationingTime = 0f;//プレイヤーに挑発されている時間

    //特定ステート内フラグ
    public bool isEternalChaseMode = false;//追いかけるステートの時、プレイヤーが部屋に隠れるまで見失わないようにするか

    protected override void Awake()
    {
        base.Awake();
        wanderingActor = GetComponent<WanderingActor>();
        inRoomWanderingActor = GetComponent<InRoomWanderingActor>();
        player = StageManager.Instance.Player;
        raycastor = GetComponent<Raycastor>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        inRoomChecker = GetComponent<InRoomChecker>();
        provokedSystem = GetComponent<ProvokedSystem>();
        movingObject = GetComponent<MovingObject>();
        rigidbody = GetComponent<Rigidbody>();

        //State登録
        yukieStateDic.Add(EnemyState.Init, new YukieStateInit());
        yukieStateDic.Add(EnemyState.Wandering, new YukieStateWandering());
        yukieStateDic.Add(EnemyState.InRoomWandering, new YukieStateInRoomWandering());
        yukieStateDic.Add(EnemyState.RotateToPlayer, new YukieStateTurnAroundToPlayer());
        yukieStateDic.Add(EnemyState.RecognizedPlayer, new YukieStateRecognizedPlayer());
        yukieStateDic.Add(EnemyState.ChasePlayer, new YukieStateChasePlayer());
        yukieStateDic.Add(EnemyState.CaughtPlayer, new YukieStateCaughtPlayer());
        yukieStateDic.Add(EnemyState.CanNotAction, new YukieStateCanNotAction());

        currentState = EnemyState.Init;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        //コールバック登録
        inRoomChecker.onEnterRoomAction = OnEnterRoomAction;
        inRoomChecker.onExitRoomAction = OnExitRoomAction;

        inRoomWanderingActor.SetActive(false, null);
        //StartCoroutine(StartAction());
    }

    //private IEnumerator StartAction()
    //{
    //    yield return null;//他のスクリプトの初期化待ち
    //    currentState = EnemyState.Init;//デバッグ
    //    yukieStateDic[currentState].StartAction();
        
    //    ChangeState(EnemyState.Wandering);
    //}

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
        //Debug.Log("Yukie_ChangeState : " + nextState);
        if (!gameObject.activeSelf) return;

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
    private void UpdatePositionXZ()
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
        UpdatePositionXZ();
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
        UpdatePositionXZ();
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
    /// <summary>
    /// 指定された方向へ向く（Y軸は雪絵と同じになる）
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="onRotationEnded"></param>
    public void TurnAroundToTargetAngle_Update(Vector3 targetPosition, UnityAction onRotationEnded)
    {
        Vector3 playerPos = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        Vector3 targetDir = playerPos - transform.position;
        Vector3 normalized = targetDir.normalized;
        float dot = Vector3.Dot(transform.forward, normalized);

        float deg = Mathf.Acos(dot) * Mathf.Rad2Deg;
        if (deg <= rotationSpeed)
        {
            transform.LookAt(playerPos);
            onRotationEnded();
        }
        else
        {
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotationSpeed * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    #region サウンド関連
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
    #endregion

    #region コライダーイベント関連
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
    #endregion
}
