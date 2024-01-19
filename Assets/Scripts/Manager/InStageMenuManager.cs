using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Onka.Manager.Data;

namespace Onka.Manager.Menu
{
    public class InStageMenuManager : SingletonMonoBehaviour<InStageMenuManager>
    {
        public bool isActivable { get; private set; } = false;//メニュー表示可能か
        public bool isInMenu { get; private set; } = false;//メニュー表示中か
        [SerializeField] private Image menuGuideImage = null;
        [SerializeField] private Text menuGuideText = null;
        [SerializeField] private Image inactiveImage = null;//メニュー表示不可能を表すUI
        [SerializeField] private Transform instanceParent = null;

        [SerializeField] private InStageMenuView menuView = null;
        [SerializeField] private OperationDescriptionView operationDescriptionViewPref = null;
        [SerializeField] private UserSettingMenuView userSettingMenuViewPref = null;
        [SerializeField] private ItemListViewer itemListViewerPref = null;
        [SerializeField]
        private HintListViewer hintListViewerPref;

        public UnityAction onOpenedMenu { private get; set; } = null;
        public UnityAction onClosedMenu { private get; set; } = null;

        protected override void Awake()
        {
            base.Awake();
            menuView.gameObject.SetActive(false);
        }

        public void Initialize()
        {
            menuGuideText.text = TextMaster.GetText("text_menu_title");
            menuView.Initialize();
            menuView.onClickOperateGuideButton = OpenOperationDesctiption;
            menuView.onClickSettingButton = OpenSettingView;
            menuView.onClickItemButton = OpenItemListView;
            menuView.onClickHintButton = OpenHintListView;
            menuView.onClickBackToTitleButton = OpenBackToTitleDialog;
            menuView.onClickCanselButton = CloseMenu;
        }

        public void SetMenuActivable(bool _isActivable)
        {
            isActivable = _isActivable;
            inactiveImage.gameObject.SetActive(!_isActivable);
        }
        public void ShowGuide()
        {
            menuGuideImage.gameObject.SetActive(true);
        }
        public void HideGuide()
        {
            menuGuideImage.gameObject.SetActive(false);
        }

        public void OpenMenu()
        {
            if (isInMenu) return;
            isInMenu = true;
            menuView.gameObject.SetActive(true);
            HideGuide();
            if (onOpenedMenu != null)
            {
                onOpenedMenu();
            }
        }

        public void CloseMenu()
        {
            if (!isInMenu) return;
            isInMenu = false;
            menuView.gameObject.SetActive(false);
            ShowGuide();
            if (onClosedMenu != null)
            {
                onClosedMenu();
            }
            InGameUtil.GCCollect();
        }

        private void OpenOperationDesctiption()
        {
            OperationDescriptionView v = Instantiate(operationDescriptionViewPref, instanceParent);
            v.InitAndShow(()=>
            {
                menuView.gameObject.SetActive(true);
            });
            menuView.gameObject.SetActive(false);
        }

        private void OpenSettingView()
        {
            UserSettingMenuView view = Instantiate(userSettingMenuViewPref, instanceParent);
            view.SetUp(() =>
            {
                menuView.gameObject.SetActive(true);
                Destroy(view.gameObject);
            });
            menuView.gameObject.SetActive(false);
        }

        private void OpenItemListView()
        {
            ItemListViewer v = Instantiate(itemListViewerPref, instanceParent);
            v.Initialize();
            v.ViewList(ItemListViewer.ViewMode.Playing);
        }

        private void OpenHintListView()
        {
            HintListViewer v = Instantiate(hintListViewerPref, instanceParent);
            v.ViewList(DataManager.Instance.GetAllItemData());
        }

        private void OpenBackToTitleDialog()
        {
            menuView.HideButtons();
            string message = TextMaster.GetText("text_menu_check_back_to_title");
            DialogManager.Instance.OpenTemplateDialog(message, TempDialogType.YesOrNo, (isBack) =>
            {
                if (isBack)
                {
                    SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Game", true, null, FadeManager.FadeColorType.Black, FadeManager.FadeColorType.Black, true, 1f, 1f, true);
                }
                else
                {
                    menuView.ShowButtons();
                }
            });
        }
    }
}