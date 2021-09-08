using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(WanderingActor))]
[RequireComponent(typeof(Raycastor))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(SoundPlayerObject))] 
public class Enemy_Yukie : Enemy
{
    public WanderingActor wanderingActor { get; private set; } = null;
    public PlayerObject player { get; private set; } = null;
    public Raycastor raycastor { get; private set; } = null;
    public CapsuleCollider capsuleCollider { get; private set; } = null;
    public SoundPlayerObject soundPlayerObject { get; private set; } = null;

    public Dictionary<EnemyState, StateBase> yukieStateDic { get; private set; } = new Dictionary<EnemyState, StateBase>();

    public UnityAction<EnemyState> onStateChangeCallback = null;
    public UnityAction<Collision> onCollisionEnterCallback = null;

    //プレイヤーとの距離計算用（Vector3では重いのでY軸を無視してVector2(x,z)に変換）
    [HideInInspector] public Vector2 yukieXZ = new Vector2(0, 0);
    [HideInInspector] public Vector2 playerXZ = new Vector2(0, 0);
    //プレイヤー検知用
    private float playerDetectionDistance = 8f;
    private float searchAngle = 60f;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        wanderingActor = GetComponent<WanderingActor>();
        player = StageManager.Instance.GetPlayer();
        raycastor = GetComponent<Raycastor>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        soundPlayerObject = GetComponent<SoundPlayerObject>();

        //State登録
        yukieStateDic.Add(EnemyState.Wandering, new YukieStateWandering());
        yukieStateDic.Add(EnemyState.RecognizedPlayer, new YukieStateRecognizedPlayer());
        yukieStateDic.Add(EnemyState.ChasePlayer, new YukieStateChasePlayer());
        yukieStateDic.Add(EnemyState.CaughtPlayer, new YukieStateCaughtPlayer());

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

    private void OnCollisionEnter(Collision collision)
    {
        if(onCollisionEnterCallback != null)
        {
            onCollisionEnterCallback(collision);
        }
        
    }
}
