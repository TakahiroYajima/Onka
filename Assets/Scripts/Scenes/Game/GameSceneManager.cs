using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using SoundSystem;
using SoundDistance;
using Onka.Manager.Event;
using Onka.Manager.Menu;

public class GameSceneManager : SceneBase
{
    [SerializeField] private GameObject restartPosition = null;//セーブ地点から再開する時の場所
    [SerializeField] private SoundDistancePoint savePointSDP = null;
    [SerializeField] private List<WanderingPoint> yukieInitWanderingPoints = new List<WanderingPoint>();
    [SerializeField] private List<SoundDistancePoint> yukieInitInstancePoints = new List<SoundDistancePoint>();
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Initialize();
        InStageMenuManager.Instance.Initialize();
        WanderingPointManager.Instance.Initialize();
        StageManager.Instance.Initialize();
        EventManager.Instance.Initialize();
        ItemManager.Instance.Initialize();
        SoundDistanceManager.Instance.Initialize();

        InGameUtil.GCCollect();

        StartScene();
    }
    
    private void StartScene()
    {
        //オープニングイベント終了済み
        if (EventManager.Instance.IsEventEnded("Event_Opnening"))
        {
            //セーブ地点から再開
            StageManager.Instance.Player.transform.position = restartPosition.transform.position;
            StageManager.Instance.Player.transform.rotation = Quaternion.Euler(0f, 90f, 0f);//セーブポイントの方を向かせる
            StageManager.Instance.Player.ChangeState(PlayerState.Free);
        }
        //オープニングイベントだけ終了済み
        if (!EventManager.Instance.IsEventEnded("Event_YukieHint"))
        {
            StageManager.Instance.InactiveYukie();
        }
        //応接室で雪絵の影を見るイベント終了済み
        else if (!EventManager.Instance.IsEventEnded("Event_YukieHintEnded"))
        {
            StageManager.Instance.InactiveYukie();
        }
        //キッチンまで気配を追ってきたが、まだ雪絵に追いかけられていない状態
        if(EventManager.Instance.IsEventEnded("Event_YukieHintEnded") && !EventManager.Instance.IsEventEnded("Event_FirstChasedFromYukie"))
        {
            StageManager.Instance.InactiveYukie();
        }

        //キッチンで雪絵から追いかけられ、逃げるイベント終了済み
        if (EventManager.Instance.IsEventEnded("Event_FirstChasedFromYukie"))
        {
            //雪絵をランダムに配置（プレイヤーから見えないところ）
            int arrayNum = UnityEngine.Random.Range(0, yukieInitInstancePoints.Count - 1);
            Vector3 yukiePos = new Vector3(yukieInitInstancePoints[arrayNum].transform.position.x, StageManager.Instance.Yukie.transform.position.y, yukieInitInstancePoints[arrayNum].transform.position.z);
            StageManager.Instance.Yukie.transform.position = yukiePos;
            StageManager.Instance.Yukie.wanderingActor.SetWanderingID(yukieInitWanderingPoints[arrayNum].PointNum);
            SoundManager.Instance.PlayEnvironmentWithKey("ambient_in_house");
        }

        EventManager.Instance.ProgressEvent();
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