using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using SoundSystem;
using SoundDistance;
using Onka.Manager.Event;
using Onka.Manager.Menu;

/// <summary>
/// ゲーム本編のScene制御
/// </summary>
/// <remark>もともとは1つのSceneずつで制御していたが、タイトルの読み込みを0にしたいためTitleManagerから直接制御されるように変更
public class GameSceneManager : SceneBase
{
    [SerializeField] private TitleManager titleManagerPrefab = null;
    private TitleManager titleManager = null;
    [SerializeField] private OpeningEventManager openingEventManagerPrefab = null;
    private OpeningEventManager openingEventManager = null;
    [SerializeField] private EndingSceneManager endingSceneManagerPrefab = null;
    private EndingSceneManager endingSceneManager = null;

    [field: SerializeField] public FieldManager fieldObject { get; private set; }
    private GameObject managerObject = null;

    private NavMeshDataInstance navMeshInstance;

    protected override void Start()
    {
        titleManager = Instantiate(titleManagerPrefab);
        titleManager.Initialize();
        SceneControlManager.Instance.FadeInScene(FadeManager.FadeColorType.Black, null, 1f);
    }

    private void DeleteTitle()
    {
        if (titleManager != null)
        {
            DestroyImmediate(titleManager.gameObject);
        }
    }

    private void DeleteOpening()
    {
        if (openingEventManager != null)
        {
            DestroyImmediate(openingEventManager.gameObject);
        }
    }

    public override void SceneStart()
    {
        base.Initialize();
        StartCoroutine(SetStart());
    }

    public override void SceneStartWithOpening()
    {
        base.Initialize();
        DeleteTitle();
        openingEventManager = Instantiate(openingEventManagerPrefab);
        openingEventManager.StartAction(() => { StartCoroutine(SetStart()); });
    }
    
    private IEnumerator SetStart()
    {
        var async = StartCoroutine(InitSceneSetUp());

        LoadingUIManager.Instance.SetActive(true);
        LoadingUIManager.Instance.SetProgress(0f);
        LoadingUIManager.Instance.SetMessage(TextMaster.GetText("text_loading"));
        yield return async;
        DeleteTitle();
        DeleteOpening();
        StartCoroutine(StartScene());
    }

    private IEnumerator InitSceneSetUp()
    {
        var await = new WaitForSeconds(0.14f);
        yield return await;
        
        //yield return LoadInScene();
        LoadingUIManager.Instance.SetProgress(0.25f);
        yield return LoadStage();
        yield return await;
        LoadingUIManager.Instance.SetProgress(0.5f);
        //Debug.Log("0.5");
        //ステージが生成されてから
        yield return SetUpManager();
        yield return await;
        LoadingUIManager.Instance.SetProgress(0.75f);
        yield return ActorSetUp();
        yield return await;
        InGameUtil.GCCollect();
        StageManager.Instance.fieldObject.SetUp();
        LoadingUIManager.Instance.SetProgress(1f);
    }
    //private IEnumerator LoadInScene()
    //{
    //    //NavMesh読み込み
    //    var async = Resources.LoadAsync<NavMeshData>("Stages/NavMesh/NavMesh");
    //    yield return async;
    //    navMeshInstance = NavMesh.AddNavMeshData(async.asset as NavMeshData);
    //}
    private IEnumerator LoadStage()
    {
        if (managerObject == null)
        {
            var async = Resources.LoadAsync<GameObject>("Managers/Managers");
            yield return async;
            managerObject = Instantiate(async.asset as GameObject, this.transform);
        }
        InStageMenuManager.Instance.Initialize();
        StageManager.Instance.Initialize(fieldObject);
        yield return null;
    }
    private IEnumerator SetUpManager()
    {
        WanderingPointManager.Instance.Initialize();
        EventManager.Instance.Initialize();
        ItemManager.Instance.Initialize();
        yield return null;
    }
    private IEnumerator ActorSetUp()
    {
        SoundDistanceManager.Instance.Initialize();
        StageManager.Instance.ActorSetUp();
        SoundDistanceManager.Instance.SetUp(StageManager.Instance.Player.SoundListener, StageManager.Instance.Yukie.SoundEmitter);
        
        yield return null;
    }
    
    private IEnumerator StartScene()
    {
        SoundDistancePoint savePointSDP = StageManager.Instance.fieldObject.SavePointSDP;

        //オープニングイベント終了済み
        if (EventManager.Instance.IsEventEnded("Event_Opnening"))
        {
            //セーブ地点から再開
            StageManager.Instance.Player.transform.position = StageManager.Instance.fieldObject.restartPosition.transform.position;
            StageManager.Instance.Player.transform.rotation = Quaternion.Euler(0f, 180f, 0f);//セーブポイントの方を向かせる
        }
        //オープニングイベントだけ終了済み
        if (!EventManager.Instance.IsEventEnded("Event_YukieHint"))
        {
            StageManager.Instance.InactiveYukieAndInitListenerPointID();
            SoundDistanceManager.Instance.Listener.SetCurrentPointID(savePointSDP.ID);
            SoundDistanceManager.Instance.Listener.SetNextTargetPointID(savePointSDP.ID);
        }
        //応接室で雪絵の影を見るイベント終了済み
        else if (!EventManager.Instance.IsEventEnded("Event_YukieHintEnded"))
        {
            StageManager.Instance.InactiveYukieAndInitListenerPointID();
            SoundDistanceManager.Instance.Listener.SetCurrentPointID(savePointSDP.ID);
            SoundDistanceManager.Instance.Listener.SetNextTargetPointID(savePointSDP.ID);
        }
        //キッチンまで気配を追ってきたが、まだ雪絵に追いかけられていない状態
        if(EventManager.Instance.IsEventEnded("Event_YukieHintEnded") && !EventManager.Instance.IsEventEnded("Event_FirstChasedFromYukie"))
        {
            StageManager.Instance.InactiveYukieAndInitListenerPointID();
            SoundDistanceManager.Instance.Listener.SetCurrentPointID(savePointSDP.ID);
            SoundDistanceManager.Instance.Listener.SetNextTargetPointID(savePointSDP.ID);
        }

        //キッチンで雪絵から追いかけられ、逃げるイベント終了済み
        if (EventManager.Instance.IsEventEnded("Event_FirstChasedFromYukie"))
        {
            //雪絵をランダムに配置（プレイヤーから見えないところ）
            var yukieInitInstancePoints = StageManager.Instance.fieldObject.YukieInitInstancePoints;
            var yukieInitWanderingPoints = StageManager.Instance.fieldObject.YukieInitWanderingPoints;
            int arrayNum = UnityEngine.Random.Range(0, yukieInitInstancePoints.Count - 1);
            Vector3 yukiePos = new Vector3(yukieInitInstancePoints[arrayNum].transform.position.x, StageManager.Instance.Yukie.transform.position.y, yukieInitInstancePoints[arrayNum].transform.position.z);
            StageManager.Instance.Yukie.transform.position = yukiePos;
            StageManager.Instance.Yukie.wanderingActor.SetWanderingID(yukieInitWanderingPoints[arrayNum].PointNum);
            StageManager.Instance.Yukie.ChangeState(EnemyState.Wandering);

            SoundDistanceManager.Instance.Emitter.SetPointID(yukieInitInstancePoints[0].ID);
            SoundDistanceManager.Instance.Listener.SetCurrentPointID(savePointSDP.ID);
            SoundDistanceManager.Instance.Listener.SetNextTargetPointID(savePointSDP.ID);
            SoundDistanceManager.Instance.Maker.SetVolume(0f);
            SoundDistanceManager.Instance.isActive = true;
            SoundDistanceManager.Instance.ForceInitCalc();
            SoundManager.Instance.PlayEnvironmentWithKey("ambient_in_house");
        }
        EventManager.Instance.isEnable = true;
        EventManager.Instance.InitProgressEach();

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.2f);
        FadeManager.Instance.FadeIn(FadeManager.FadeColorType.Black, 1f, ()=>
        {
            StageManager.Instance.Player.ChangeState(PlayerState.Free);
        });
        yield return new WaitForSeconds(0.1f);
        LoadingUIManager.Instance.SetInactiveWithFadeOut();
    }

    public void StartEnding()
    {
        if(endingSceneManager == null)
        {
            endingSceneManager = Instantiate(endingSceneManagerPrefab);
        }
        endingSceneManager.StartEnding();
    }
}

public class InGameUtil
{
    /// <summary>
    /// カーソルを操作可能にする
    /// </summary>
    public static void DoCursorFree()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    /// <summary>
    /// カーソルを操作不可にする
    /// </summary>
    public static void DoCursorLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void GCCollect()
    {
        GC.Collect();
    }
}