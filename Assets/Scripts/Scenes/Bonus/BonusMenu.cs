using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusMenu : MonoBehaviour
{
    [SerializeField] private BonusSceneManager manager = null;
    [SerializeField] private GameObject mainMenuObj = null;
    [SerializeField] private GameObject bonusMenuObj = null;

    [SerializeField] private ItemListViewer itemListViewer = null;
    [SerializeField] private CharactorListViewer charactorListViewer = null;

    #region MainMenu
    /// <summary>
    /// タイトルへ戻るボタンクリック時
    /// </summary>
    public void OnPressBackToTitleButton()
    {
        SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Title", true, null, FadeManager.FadeColorType.Black, FadeManager.FadeColorType.Black);
    }

    public void OnPressOpenMenuButton()
    {
        mainMenuObj.SetActive(false);
        bonusMenuObj.SetActive(true);
    }

    public void OnPressBackToMainMenuButton()
    {
        mainMenuObj.SetActive(true);
        bonusMenuObj.SetActive(false);
    }
    #endregion

    #region BonusMenu
    public void OnPressItemListViewButton()
    {
        bonusMenuObj.SetActive(false);
        itemListViewer.gameObject.SetActive(true);
        itemListViewer.Initialize();
        itemListViewer.ViewList(ItemListViewer.ViewMode.Master);
        itemListViewer.onClosed = OnCloseItemView;
    }
    private void OnCloseItemView()
    {
        bonusMenuObj.SetActive(true);
        itemListViewer.gameObject.SetActive(false);
    }

    public void OnPressCharactorListViewButton()
    {
        charactorListViewer.gameObject.SetActive(true);
        charactorListViewer.Initialize();
        charactorListViewer.ViewList(manager.charactorDataList);
        charactorListViewer.onClosed = OnCloseCharactorView;
    }
    private void OnCloseCharactorView()
    {
        bonusMenuObj.SetActive(true);
        charactorListViewer.gameObject.SetActive(false);
    }

    public void OnPressStoryButton()
    {

    }
    #endregion
}
