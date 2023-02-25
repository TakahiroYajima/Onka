using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using SoundSystem;
using SoundDistance;
using Onka.Manager.Event;
using Onka.Manager.Menu;

public class GameSceneManager : SceneBase
{
    [field: SerializeField] public FieldManager fieldObject { get; private set; }
    private GameObject managerObject = null;

    private NavMeshDataInstance navMeshInstance;

    protected override void Start()
    {
        base.Initialize();

        StartCoroutine(SetStart());
    }

    private IEnumerator SetStart()
    {
        var async = StartCoroutine(InitSceneSetUp());

        LoadingUIManager.Instance.SetMessage("ステージを構成しています");
        yield return async;
        StartScene();
    }

    private IEnumerator InitSceneSetUp()
    {
        yield return null;
        //LoadingUIManager.Instance.SetProgress(0f);
        //yield return LoadInScene();
        LoadingUIManager.Instance.SetProgress(0.25f);
        yield return LoadStage();
        LoadingUIManager.Instance.SetProgress(0.5f);
        //Debug.Log("0.5");
        //ステージが生成されてから
        yield return SetUpManager();
        LoadingUIManager.Instance.SetProgress(0.75f);
        //Debug.Log("0.75");
        yield return ActorSetUp();
        
        yield return null;
        InGameUtil.GCCollect();
        //Debug.Log("0.75_2");
        yield return null;
        StageManager.Instance.fieldObject.SetUp();
        //Debug.Log("1");
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
    
    private void StartScene()
    {
        SoundDistancePoint savePointSDP = StageManager.Instance.fieldObject.SavePointSDP;

        //オープニングイベント終了済み
        if (EventManager.Instance.IsEventEnded("Event_Opnening"))
        {
            //セーブ地点から再開
            StageManager.Instance.Player.transform.position = StageManager.Instance.fieldObject.restartPosition.transform.position;
            StageManager.Instance.Player.transform.rotation = Quaternion.Euler(0f, 180f, 0f);//セーブポイントの方を向かせる
            StageManager.Instance.Player.ChangeState(PlayerState.Free);
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

        LoadingUIManager.Instance.SetActive(false);
        FadeManager.Instance.FadeIn(FadeManager.FadeColorType.Black, 1f, null);
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