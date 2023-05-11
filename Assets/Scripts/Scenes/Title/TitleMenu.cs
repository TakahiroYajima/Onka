using UnityEngine;
using UnityEngine.UI;
using Onka.Manager.Data;

public class TitleMenu : MonoBehaviour
{
    [SerializeField] private TitleManager manager = null;
    [SerializeField] private GameObject mainMenuObj = null;
    [SerializeField] private SelectSaveDataView selectSaveDataView = null;
    [SerializeField] private GameObject bonusButtonObj = null;

    //マスタ
    [SerializeField] private Text titleText = null;
    [SerializeField] private Text startText = null;
    [SerializeField] private Text continueText = null;
    [SerializeField] private Text quitText = null;
    [SerializeField] private Text bonusText = null;
    [SerializeField] private Text settingText = null;

    public void Initialize()
    {
        bonusButtonObj.SetActive(DataManager.Instance.IsGameClearedInThePast());

        titleText.text = TextMaster.GetText("text_title");
        startText.text = TextMaster.GetText("text_new_game");
        continueText.text = TextMaster.GetText("text_continue_game");
        quitText.text = TextMaster.GetText("text_quit_game");
        bonusText.text = TextMaster.GetText("text_bonus");
        settingText.text = TextMaster.GetText("text_language_setting");
    }

    public void OnPressNewGameButton()
    {
        mainMenuObj.SetActive(false);
        DoActiveSelectSaveDataView();
        selectSaveDataView.View(SelectSaveDataView.ViewSelectType.NewGame);
    }
    public void OnPressLoadGameButton()
    {
        mainMenuObj.SetActive(false);
        DoActiveSelectSaveDataView();
        selectSaveDataView.View(SelectSaveDataView.ViewSelectType.LoadGame);
    }

    private void BackToMainMenu()
    {
        mainMenuObj.SetActive(true);
        selectSaveDataView.gameObject.SetActive(false);
    }

    private void DoActiveSelectSaveDataView()
    {
        selectSaveDataView.gameObject.SetActive(true);
        selectSaveDataView.onPlayDataDecisioned = OnDecisionedPlayGameData;
        selectSaveDataView.onClose = BackToMainMenu;
    }

    private void OnDecisionedPlayGameData(SelectSaveDataView.ViewSelectType selectType)
    {
        if (selectType == SelectSaveDataView.ViewSelectType.NewGame)
        {
            manager.PressStartButton();
        }
        else
        {
            //ロードでも家の中に入って一度もセーブせずに終わっていたらオープニングからになる
            if (DataManager.Instance.IsAfterFirstSavedCurrentSelectData())
            {
                manager.PressLoadButton();
            }
            else
            {
                manager.PressStartButton();
            }
        }
    }

    public void OnPressQuitButton()
    {
        manager.PressQuitButton();
    }

    public void OnPressSettingButton()
    {

    }
}
