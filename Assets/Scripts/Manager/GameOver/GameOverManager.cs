using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using SoundSystem;

/// <summary>
/// ゲームオーバーの演出とデータ操作
/// </summary>
public class GameOverManager : SingletonMonoBehaviour<GameOverManager>
{
    [SerializeField] private GameOverTable gameOverEventPrefs = null;
    private GameOverEventBase instanceEvent = null;
    [SerializeField] private GameObject canvasObj = null;
    [SerializeField] private Text gameOverText = null;

    private bool isDoingAction = false;
    // Start is called before the first frame update
    void Start()
    {
        canvasObj.SetActive(false);
    }

    public void StartGameOver(GameOverType type)
    {
        if (isDoingAction) return;
        isDoingAction = true;
        instanceEvent = Instantiate(gameOverEventPrefs.GetTable()[type], transform);
        instanceEvent.Initialize();
        StageManager.Instance.AllEnemyInactive();
        gameOverText.text = TextMaster.GetText("text_game_over");
        StartCoroutine(StartGameOverAction());
    }

    private IEnumerator StartGameOverAction()
    {
        yield return null;
        instanceEvent.StartEvent();
    }

    private IEnumerator GameOverAction()
    {
        Color c = gameOverText.color;
        c.a = 0f;
        gameOverText.color = c;
        canvasObj.SetActive(true);
        SoundManager.Instance.StopAllBGM();
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.PlayBGMWithKey("bgm_gameover");
        yield return StartCoroutine(FadeManager.Instance.FadeAction(gameOverText, FadeType.Out, 2f));
        yield return new WaitForSeconds(1.5f);
        InGameUtil.DoCursorFree();
        DialogManager.Instance.OpenTemplateDialog(TextMaster.GetText("text_game_over_back_to_title"), TempDialogType.InButtonMessage, BackToTitleCallback);
    }

    public void EndEventAction()
    {
        instanceEvent.EndEvent();
        StageManager.Instance.FinishGameOver_DataControl();
        StartCoroutine(GameOverAction());
    }

    private void BackToTitleCallback(bool b)
    {
        SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Game", true, null,FadeManager.FadeColorType.Black, FadeManager.FadeColorType.Black);
    }
}

public enum GameOverType
{
    YukieArrested,
    ShioriArrested,
    AzuYuzuArrested,
    KozoArrested,
    HatsuArrested,
}

/// <summary>
/// ジェネリックを隠すために継承してしまう
/// [System.Serializable]を書くのを忘れない
/// </summary>
[System.Serializable]
public class GameOverTable : Serialize.TableBase<GameOverType, GameOverEventBase, GameOverPair>
{


}

/// <summary>
/// ジェネリックを隠すために継承してしまう
/// [System.Serializable]を書くのを忘れない
/// </summary>
[System.Serializable]
public class GameOverPair : Serialize.KeyAndValue<GameOverType, GameOverEventBase>
{

    public GameOverPair(GameOverType key, GameOverEventBase value) : base(key, value)
    {

    }
}

