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

    private void OnPlayerStateChanged(PlayerState state)
    {
        
    }
    private void OnYukieStateChanged(EnemyState state)
    {

    }
}
