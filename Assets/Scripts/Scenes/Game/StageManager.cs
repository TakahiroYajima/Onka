using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    [SerializeField] private PlayerObject playerObject = null;
    public PlayerObject Player { get { return playerObject; } }
    [SerializeField] private Enemy_Yukie yukieObject = null;
    public Enemy_Yukie Yukie { get { return yukieObject; } }
    public Enemy_Shiori Shiori { get; set; } = null;

    private PlayerState prevPlayerState = PlayerState.Free;
    private EnemyState prevYukieState = EnemyState.Init;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        playerObject.onStateChangeCallback = OnPlayerStateChanged;
        yukieObject.onStateChangeCallback = OnYukieStateChanged;
    }

    private void OnPlayerStateChanged(PlayerState prev, PlayerState current)
    {
        if(prev == PlayerState.ItemGet && current == PlayerState.Chased){
            //アイテム閲覧から追いかけられていたら、UIが表示されたままになっている可能性があるので、いったん初期化
            ItemManager.Instance.WatchDiaryManager.EndWatchingItem();
            ItemManager.Instance.FinishWatchItem(true);
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
}
