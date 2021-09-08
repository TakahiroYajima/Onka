using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    [SerializeField] private PlayerObject playerObject = null;
    [SerializeField] private Enemy_Yukie yukieObject = null;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        playerObject.onStateChangeCallback = OnPlayerStateChanged;
        yukieObject.onStateChangeCallback = OnYukieStateChanged;
    }

    public PlayerObject GetPlayer()
    {
        return playerObject;
    }
    public Enemy_Yukie GetYukie()
    {
        return yukieObject;
    }

    private void OnPlayerStateChanged(PlayerState state)
    {
        
    }
    private void OnYukieStateChanged(EnemyState state)
    {

    }
}
