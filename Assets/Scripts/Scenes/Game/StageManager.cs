using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoundSystem;
using SoundDistance;
using Onka.Manager.Menu;
using Onka.Manager.InputKey;
using Onka.Manager.Data;

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    //[SerializeField] private FieldManager houseFieldPrefab;
    public FieldManager fieldObject { get; private set; }
    [SerializeField] private PlayerObject playerPrefab;
    private PlayerObject playerObject = null;
    public PlayerObject Player { get { return playerObject; } }
    [SerializeField] private Enemy_Yukie yukiePrefab;
    private Enemy_Yukie yukieObject = null;
    public Enemy_Yukie Yukie { get { return yukieObject; } }
    [System.NonSerialized] public Enemy_Kozo Kozo = null;
    [System.NonSerialized] public Enemy_Hatsu Hatsu = null;
    [System.NonSerialized] public Enemy_Shiori Shiori = null;
    [System.NonSerialized] public Enemy_Azuha Azuha = null;
    [System.NonSerialized] public Enemy_Yuzuha Yuzuha = null;

    

    private PlayerState prevPlayerState = PlayerState.Free;
    private EnemyState prevYukieState = EnemyState.Init;
    private EnemyState prevKozoState = EnemyState.Init;
    private EnemyState prevHatsuState = EnemyState.Init;
    private Enemy_ShioriState prevShioriState = Enemy_ShioriState.Init;
    private EnemyState prevAzuhaState = EnemyState.Init;
    private EnemyState prevYuzuhaState = EnemyState.Init;

    public void Initialize(FieldManager field)
    {
        fieldObject = field;
        //if(fieldObject == null)
        //{
        //    fieldObject = Instantiate(houseFieldPrefab, this.transform);
        //}

        InStageMenuManager.Instance.onOpenedMenu = OpenedMenuAction;
        InStageMenuManager.Instance.onClosedMenu = ClosedMenuAction;
        InputKeyManager.Instance.onEscKeyPress = OnEscKeyPress;
        InputKeyManager.Instance.onF12KeyPress = OnF12KeyPress;
    }

    public void ActorSetUp()
    {
        if (playerObject == null)
        {
            playerObject = Instantiate(playerPrefab, this.transform);
        }
        playerObject.onStateChangeCallback = OnPlayerStateChanged;
        if (yukieObject == null)
        {
            yukieObject = Instantiate(yukiePrefab, this.transform);
            yukieObject.SetUp();
        }
        yukieObject.onStateChangeCallback = OnYukieStateChanged;
    }

    private void OnPlayerStateChanged(PlayerState prev, PlayerState current)
    {
        if (current == PlayerState.Free)
        {
            InStageMenuManager.Instance.SetMenuActivable(true);
        }
        else if(current != PlayerState.InMenu)
        {
            InStageMenuManager.Instance.SetMenuActivable(false);
        }

        if (prev == PlayerState.ItemGet && current == PlayerState.Chased){
            //アイテム閲覧から追いかけられていたら、UIが表示されたままになっている可能性があるので、いったん初期化
            ItemManager.Instance.WatchDiaryManager.EndWatchingItem();
            ItemManager.Instance.FinishWatchItem(true);
        }else if(prev == PlayerState.SolveKeylock && current == PlayerState.Chased)
        {
            SolveKeylockManager.Instance.ForceFinish();
        }
    }
    private void OnYukieStateChanged(EnemyState state)
    {

    }

    public void InactiveYukieAndInitListenerPointID(int initPointID = 0)
    {
        SoundDistanceManager.Instance.SetInactive(initPointID, initPointID);
        Yukie.gameObject.SetActive(false);
    }

    /// <summary>
    /// SoundDistancePointの位置に強制移動させる
    /// </summary>
    /// <param name="soundDistancePointKey"></param>
    public void ForceOperationYukiePositionWithSDP(string soundDistancePointKey, string targetWanderingPointKey)
    {
        var soundDistancePoint = SoundDistanceManager.Instance.GetSoundDistancePoint(soundDistancePointKey);
        var currentWanderingPoint = WanderingPointManager.Instance.GetWanderingPoint(targetWanderingPointKey);
        //var nextWanderingPoint = WanderingPointManager.Instance.GetWanderingPoint(nextWanderingPointKey);
        yukieObject.ChangeState(EnemyState.CanNotAction);
        var yukiePos = soundDistancePoint.transform.position;
        yukiePos.y = yukieObject.transform.position.y;
        yukieObject.transform.position = yukiePos;
        yukieObject.ChangeState(EnemyState.Wandering);
        yukieObject.wanderingActor.SetWanderingID(currentWanderingPoint.PointNum);

        SoundDistanceManager.Instance.Emitter.SetPointID(soundDistancePoint.ID);
        SoundDistanceManager.Instance.ForceInitCalc();
    }

    /// <summary>
    /// Escキーを押した時の挙動（メニュー表示）
    /// </summary>
    private void OnEscKeyPress()
    {
        if (playerObject.currentState == PlayerState.Free && !InStageMenuManager.Instance.isInMenu)
        {
            InStageMenuManager.Instance.OpenMenu();
        }
        else if (playerObject.currentState == PlayerState.InMenu && InStageMenuManager.Instance.isInMenu)
        {
            //InStageMenuManager.Instance.CloseMenu();
        }
    }
    private void OnF12KeyPress()
    {
        //if(Cursor.lockState == CursorLockMode.Locked)
        //{
        //    InGameUtil.DoCursorFree();
        //}
        //else
        //{
        //    InGameUtil.DoCursorLock();
        //}
        Application.Quit();
    }
    private void OpenedMenuAction()
    {
        CrosshairManager.Instance.SetCrosshairActive(false);
        InGameUtil.DoCursorFree();
        if (playerObject != null)
        {
            playerObject.ChangeState(PlayerState.InMenu);
            playerObject.ForcedStopFPS();
        }
        if (Yukie != null)
        {
            prevYukieState = yukieObject.currentState;
            yukieObject.ChangeState(EnemyState.CanNotAction);
        }
    }
    private void ClosedMenuAction()
    {
        CrosshairManager.Instance.SetCrosshairActive(true);
        InGameUtil.DoCursorLock();
        if (playerObject != null)
        {
            playerObject.ChangeState(PlayerState.Free);
        }
        if (Yukie != null)
        {
            yukieObject.ChangeState(prevYukieState);
        }
    }

    public void OnStartPlayerWatchItem()
    {
        if (Yukie != null && GameManager.Instance.GetSettingData().difficulty == Difficulty.Normal)
        {
            prevYukieState = yukieObject.currentState;
            yukieObject.ChangeState(EnemyState.CanNotAction);
        }
    }

    public void OnEndPlayerWatchItem()
    {
        if (Yukie != null && GameManager.Instance.GetSettingData().difficulty == Difficulty.Normal)
        {
            yukieObject.ChangeState(prevYukieState);
        }
    }

    public void StartSaveAction()
    {
        prevPlayerState = playerObject.currentState;
        prevYukieState = yukieObject.currentState;
        playerObject.ChangeState(PlayerState.InMenu);
        yukieObject.ChangeState(EnemyState.CanNotAction);
        InGameUtil.DoCursorFree();
        DialogManager.Instance.OpenTemplateDialog(TextMaster.GetText("text_save_point_dialog_title"), TempDialogType.YesOrNo, SaveAction);
    }

    private void SaveAction(bool isSave)
    {
        if (isSave)
        {
            SoundSystem.SoundManager.Instance.PlaySeWithKey("menuse_save");
            DataManager.Instance.SaveGameData(EndSaveAction);
        }
        else
            EndSaveAction();
    }
    private void EndSaveAction()
    {
        InGameUtil.DoCursorLock();
        playerObject.ChangeState(prevPlayerState);
        yukieObject.ChangeState(prevYukieState);
    }

    public void AllEnemyForceChangeStateCanNotAction()
    {
        if (Yukie != null)
        {
            prevYukieState = yukieObject.currentState;
            yukieObject.ChangeState(EnemyState.CanNotAction);
        }
        if(Kozo != null)
        {
            prevKozoState = Kozo.currentState;
            Kozo.ChangeState(EnemyState.Init);
        }
        if (Hatsu != null)
        {
            prevHatsuState = Hatsu.currentState;
            Hatsu.ChangeState(EnemyState.Init);
        }
        if (Shiori != null)
        {
            prevShioriState = Shiori.currentShioriState;
            Shiori.ChangeState(Enemy_ShioriState.Init);
        }
        if (Azuha != null)
        {
            prevAzuhaState = Azuha.currentState;
            Azuha.ChangeState(EnemyState.CanNotAction);
        }
        if (Yuzuha != null && Yuzuha.gameObject.activeSelf)
        {
            prevYukieState = Yuzuha.currentState;
            Yuzuha.ChangeState(EnemyState.CanNotAction);
        }
    }

    public void AllEnemyRestoreState()
    {
        if (Yukie != null)
        {
            yukieObject.ChangeState(prevYukieState);
        }
        if (Kozo != null)
        {
            Kozo.ChangeState(prevKozoState);
        }
        if (Hatsu != null)
        {
            Hatsu.ChangeState(prevHatsuState);
        }
        if (Shiori != null)
        {
            Shiori.ChangeState(prevShioriState);
        }
        if (Azuha != null)
        {
            Azuha.ChangeState(prevAzuhaState);
        }
        if (Yuzuha != null && Yuzuha.gameObject.activeSelf)
        {
            Yuzuha.ChangeState(prevYukieState);
        }
    }

    public void AllEnemyInactive()
    {
        if(Yukie != null)
        {
            yukieObject.StopSound();
            yukieObject.gameObject.SetActive(false);
        }
        if (Kozo != null)
        {
            Kozo.soundPlayerObject.StopSound();
            Kozo.gameObject.SetActive(false);
        }
        if (Hatsu != null)
        {
            Hatsu.soundPlayerObject.StopSound();
            Hatsu.gameObject.SetActive(false);
        }
        if (Shiori != null)
        {
            Shiori.gameObject.SetActive(false);
        }
        if(Azuha != null)
        {
            Azuha.ChangeState(EnemyState.CanNotAction);
            Azuha.gameObject.SetActive(false);
        }
        if(Yuzuha != null && Yuzuha.gameObject.activeSelf)
        {
            Yuzuha.ChangeState(EnemyState.CanNotAction);
            Yuzuha.gameObject.SetActive(false);
        }
    }

    public void FinishGameOver_DataControl()
    {
        yukieObject.ChangeState(EnemyState.CanNotAction);

    }
}
