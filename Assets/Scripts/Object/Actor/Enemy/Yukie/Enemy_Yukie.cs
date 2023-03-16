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
    private SoundPlayerObject emitterSoundPlayer = null;
    [SerializeField] private CapsuleCollider toPlayerWallCollider = null;//プレイヤーと一定の距離を保つためにあるコライダー
    public CapsuleCollider ToPlayerWallCollider { get { return toPlayerWallCollider; } }
    [SerializeField] private Transform faceTransform = null;
    public Transform FaceTransform { get { return faceTransform; } }
    [SerializeField] private Transform eyeTransform = null;
    public Transform EyeTransform { get { return eyeTransform; } }
    [SerializeField] private SoundDistanceEmitter soundEmitter;
    public SoundDistanceEmitter SoundEmitter { get { return soundEmitter; } }

    public Dictionary<EnemyState, StateBase> yukieStateDic { get; private set; } = new Dictionary<EnemyState, StateBase>();

    public UnityAction<EnemyState> onStateChangeCallback = null;
    public UnityAction<Collider> onColliderEnterCallback = null;
    public UnityAction<Collider> onColliderStayCallback = null;

    //プレイヤーとの距離計算用（Y軸を無視してVector2(x,z)に変換）
    private Vector2 yukieXZ = new Vector2(0, 0);
    private Vector2 playerXZ = new Vector2(0, 0);

    //プレイヤー検知用
    private float playerDetectionDistance = 13f;
    private float searchAngle = 60f;
    private const float DefaultRotationSpeed = 4f;
    public const int DoUpdateFrameCount = 6;//Rayは重いので6フレームに1回処理

    //プレイヤーが自分の視界の範囲外をうろつく＝挑発しているので、それに対する不意打ち要素
    private const float NoticeProvocationTime = 10f;//挑発に気付くまでの時間

    //特定ステート内フラグ
    [System.NonSerialized] public bool isEternalChaseMode = false;//追いかけるステートの時、プレイヤーが部屋に隠れるまで見失わないようにするか
    [System.NonSerialized] public bool isIgnoreInRoom = false;//部屋の中に入ってもプレイヤーを見失わずに強制で追いかけるか
    [System.NonSerialized] public bool isRecognizedPlayerInRoom = false;//部屋を覗くアクションの時にプレイヤーを発見していたか

    protected override void Awake()
    {
        base.Awake();
        currentState = EnemyState.Init;
    }
    /// <summary>
    /// 初期設定
    /// </summary>
    public void SetUp()
    {
        wanderingActor = GetComponent<WanderingActor>();
        inRoomWanderingActor = GetComponent<InRoomWanderingActor>();
        player = StageManager.Instance.Player;
        raycastor = GetComponent<Raycastor>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        inRoomChecker = GetComponent<InRoomChecker>();
        provokedSystem = GetComponent<ProvokedSystem>();
        movingObject = GetComponent<MovingObject>();
        rigidbody = GetComponent<Rigidbody>();
        emitterSoundPlayer = SoundDistanceManager.Instance.Maker.GetComponent<SoundPlayerObject>();

        //State登録
        yukieStateDic.Add(EnemyState.Init, new YukieStateInit(this));
        yukieStateDic.Add(EnemyState.Wandering, new YukieStateWandering(this));
        yukieStateDic.Add(EnemyState.InRoomWandering, new YukieStateInRoomWandering(this));
        yukieStateDic.Add(EnemyState.RotateToPlayer, new YukieStateTurnAroundToPlayer(this));
        yukieStateDic.Add(EnemyState.RecognizedPlayer, new YukieStateRecognizedPlayer(this));
        yukieStateDic.Add(EnemyState.ChasePlayer, new YukieStateChasePlayer(this));
        yukieStateDic.Add(EnemyState.CaughtPlayer, new YukieStateCaughtPlayer(this));
        yukieStateDic.Add(EnemyState.CanNotAction, new YukieStateCanNotAction(this));

        //コールバック登録
        inRoomChecker.onEnterRoomAction = OnEnterRoomAction;
        inRoomChecker.onExitRoomAction = OnExitRoomAction;

        inRoomWanderingActor.SetActive(false, null);
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
    /// <returns>終了フラグ</returns>
    public bool TurnAroundToTargetAngle_Update(Vector3 targetPosition, bool isFaceRotationHeight = false, float rotationSpeed = DefaultRotationSpeed)
    {
        Vector3 yukieTargetPos = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        Vector3 yukieTargetDir = yukieTargetPos - transform.position;
        Vector3 normalized = yukieTargetDir.normalized;
        float dot = Vector3.Dot(transform.forward, normalized);

        float deg = Mathf.Acos(dot) * Mathf.Rad2Deg;
        if (dot >= 1f || deg <= rotationSpeed)
        {
            transform.LookAt(yukieTargetPos);
            return true;
        }
        else
        {
            Vector3 newDir = Vector3.RotateTowards(transform.forward, yukieTargetDir, rotationSpeed * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDir);
            if (isFaceRotationHeight)
            {
                Vector3 faceLookTarget = targetPosition;//transform.forward;
                //faceLookTarget.y = targetPosition.y;
                Vector3 faceTargetDir = faceLookTarget - faceTransform.position;//new Vector3(0f, faceLookTarget.y - faceTransform.position.y, 0f);//XZは本体がやってくれるので、顔はYの高さの分回転すればよい
                float faceDot = Vector3.Dot(faceTransform.forward, faceTargetDir);
                float faceDeg = Mathf.Acos(faceDot) * Mathf.Rad2Deg;
                if (dot < 1f && deg > rotationSpeed)
                {
                    Vector3 faceNewDir = Vector3.RotateTowards(faceTransform.forward, faceTargetDir, rotationSpeed * Time.deltaTime, 0f);
                    faceTransform.rotation = Quaternion.LookRotation(faceNewDir);
                }
                else
                {
                    faceTransform.LookAt(faceLookTarget);
                }
                //LookRotationFaceToTarget(targetPosition, rotationSpeed);
            }
        }
        return false;
    }

    public void LookRotationFaceToTarget(Vector3 targetPosition, float rotationSpeed = DefaultRotationSpeed)
    {
        Vector3 faceLookTarget = targetPosition;//transform.forward;
                                                //faceLookTarget.y = targetPosition.y;
        Vector3 faceTargetDir = faceLookTarget - faceTransform.position;//new Vector3(0f, faceLookTarget.y - faceTransform.position.y, 0f);//XZは本体がやってくれるので、顔はYの高さの分回転すればよい
        float faceDot = Vector3.Dot(faceTransform.forward, faceTargetDir);
        float faceDeg = Mathf.Acos(faceDot) * Mathf.Rad2Deg;
        if (faceDot < 1f && faceDeg > rotationSpeed)
        {
            Vector3 faceNewDir = Vector3.RotateTowards(faceTransform.forward, faceTargetDir, rotationSpeed * Time.deltaTime, 0f);
            faceTransform.rotation = Quaternion.LookRotation(faceNewDir);
            //faceTransform.rotation = Quaternion.LookRotation(player.eyePosition - faceTransform.position);
        }
        else
        {
            //faceTransform.LookAt(faceLookTarget);
        }
    }

    #region サウンド関連
    public void PlaySoundOne(int arrayNum, float volume = 1f)
    {
        emitterSoundPlayer.audioSource.loop = false;
        SoundDistanceManager.Instance.StartSoundDistanceMaker(emitterSoundPlayer.GetClip(arrayNum), volume);
    }
    public void PlaySoundLoop(int arrayNum, float volume = 1f)
    {
        emitterSoundPlayer.audioSource.loop = true;
        SoundDistanceManager.Instance.StartSoundDistanceMaker(emitterSoundPlayer.GetClip(arrayNum), volume);
    }
    public void SetMaxVolume(float _volume)
    {
        SoundDistanceManager.Instance.SetMaxVolumeToMaker(_volume);
    }
    public void StopSound(bool isAudioClipClear = false)
    {
        SoundDistanceManager.Instance.StopSoundDistanceMaker(isAudioClipClear);
    }
    public void SetVolumeONEnable(bool _isEnable)
    {
        SoundDistanceManager.Instance.SetVolumeONEnable(_isEnable);
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
