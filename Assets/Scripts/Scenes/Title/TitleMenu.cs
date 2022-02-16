using UnityEngine;
using Onka.Manager.Data;

public class TitleMenu : MonoBehaviour
{
    [SerializeField] private TitleManager manager = null;
    [SerializeField] private GameObject mainMenuObj = null;
    [SerializeField] private SelectSaveDataView selectSaveDataView = null;

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
}
