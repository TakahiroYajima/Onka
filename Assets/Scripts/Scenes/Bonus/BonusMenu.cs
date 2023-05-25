using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BonusMenu : MonoBehaviour
{
    [SerializeField] private BonusSceneManager manager = null;
    [SerializeField] private Image backgroundImage = null;
    [SerializeField] private GameObject mainMenuObj = null;
    [SerializeField] private Text openFileText = null;
    [SerializeField] private Text backToTitleText = null;
    [SerializeField] private Text twitterText = null;

    [SerializeField] private GameObject bonusMenuObj = null;
    [SerializeField] private Text thankYouLetterText = null;
    [SerializeField] private Text itemListText = null;
    [SerializeField] private Text characterListText = null;
    [SerializeField] private Text backButtonText = null;

    [SerializeField] private ItemListViewer itemListViewer = null;
    [SerializeField] private CharactorListViewer charactorListViewer = null;

    public void Initialize()
    {
        backgroundImage.enabled = false;
        SetMainViewTexts();
    }

    private void SetMainViewTexts()
    {
        openFileText.text = TextMaster.GetText("text_bonus");
        backToTitleText.text = TextMaster.GetText("text_back_to_title");
        twitterText.text = TextMaster.GetText("text_twitter");

        thankYouLetterText.text = TextMaster.GetText("bonus_thank_you_letter");
        itemListText.text = TextMaster.GetText("text_bonus_item_list");
        characterListText.text = TextMaster.GetText("text_bonus_character_list");
        backButtonText.text = TextMaster.GetText("text_back");
    }

    #region MainMenu
    public void OnPressBackToTitleButton()
    {
        SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Game", true, null, FadeManager.FadeColorType.Black, FadeManager.FadeColorType.Black);
    }

    public void OnPressOpenMenuButton()
    {
        mainMenuObj.SetActive(false);
        backgroundImage.color = new Color(0, 0, 0, 0);
        backgroundImage.enabled = true;
        StartCoroutine(FadeManager.Instance.FadeImage(backgroundImage, FadeType.Out, 0.47f, 0.35f, () =>
        {
            bonusMenuObj.SetActive(true);
        }));
    }

    public void OnPressBackToMainMenuButton()
    {
        bonusMenuObj.SetActive(false);
        StartCoroutine(FadeManager.Instance.FadeImage(backgroundImage, FadeType.In, 0f, 2.1f, () =>
        {
            mainMenuObj.SetActive(true);
            backgroundImage.enabled = false;
        }));
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
        bonusMenuObj.SetActive(false);
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

    public void OnPressTwitterButton()
    {
        Application.OpenURL("https://twitter.com/YajinProject");
    }
    #endregion
}
