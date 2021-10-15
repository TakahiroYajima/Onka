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
}
