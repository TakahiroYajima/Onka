using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    [SerializeField] private PlayerObject playerObject = null;
    public PlayerObject Player { get { return playerObject; } }
    [SerializeField] private Enemy_Yukie yukieObject = null;
    public Enemy_Yukie Yukie { get { return yukieObject; } }
    public Enemy_Shiori Shiori = null;
    public Enemy_Azuha Azuha = null;
    public Enemy_Yuzuha Yuzuha = null;

    private PlayerState prevPlayerState = PlayerState.Free;
    private EnemyState prevYukieState = EnemyState.Init;
    private Enemy_ShioriState prevShioriState = Enemy_ShioriState.Init;
    private EnemyState prevAzuhaState = EnemyState.Init;
    private EnemyState prevYuzuhaState = EnemyState.Init;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (playerObject != null)
        {
            playerObject.onStateChangeCallback = OnPlayerStateChanged;
        }
        if (yukieObject != null)
        {
            yukieObject.onStateChangeCallback = OnYukieStateChanged;
        }
    }

    private void OnPlayerStateChanged(PlayerState prev, PlayerState current)
    {
        if(prev == PlayerState.ItemGet && current == PlayerState.Chased){
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

    public void StartSaveAction()
    {
        prevPlayerState = playerObject.currentState;
        prevYukieState = yukieObject.currentState;
        playerObject.ChangeState(PlayerState.InMenu);
        yukieObject.ChangeState(EnemyState.CanNotAction);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        DialogManager.Instance.OpenTemplateDialog("セーブしますか？", TempDialogType.YesOrNo, SaveAction);
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        if (Shiori != null)
        {
            prevShioriState = Shiori.currentState;
            Shiori.ChangeState(Enemy_ShioriState.Init);
        }
        if (Azuha != null)
        {
            prevAzuhaState = Azuha.currentState;
            Azuha.ChangeState(EnemyState.CanNotAction);
        }
        if (Yuzuha != null)
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
        if (Shiori != null)
        {
            Shiori.ChangeState(prevShioriState);
        }
        if (Azuha != null)
        {
            Azuha.ChangeState(prevAzuhaState);
        }
        if (Yuzuha != null)
        {
            Yuzuha.ChangeState(prevYukieState);
        }
    }

    public void StartGameOverDataControl()
    {
        if(Yukie != null)
        {
            yukieObject.gameObject.SetActive(false);
        }
        if(Shiori != null)
        {
            Shiori.gameObject.SetActive(false);
        }
        if(Azuha != null)
        {
            Azuha.ChangeState(EnemyState.CanNotAction);
            Azuha.gameObject.SetActive(false);
        }
        if(Yuzuha != null)
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
