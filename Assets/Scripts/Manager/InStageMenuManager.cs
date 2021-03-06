using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Onka.Manager.Menu
{
    public class InStageMenuManager : SingletonMonoBehaviour<InStageMenuManager>
    {
        public bool isActivable { get; private set; } = false;//メニュー表示可能か
        public bool isInMenu { get; private set; } = false;//メニュー表示中か
        [SerializeField] private Image menuGuideImage = null;
        [SerializeField] private Image inactiveImage = null;//メニュー表示不可能を表すUI
        [SerializeField] private Transform instanceParent = null;

        [SerializeField] private InStageMenuView menuView = null;
        [SerializeField] private OperationDescriptionView operationDescriptionViewPref = null;
        [SerializeField] private ItemListViewer itemListViewerPref = null;

        public UnityAction onOpenedMenu { private get; set; } = null;
        public UnityAction onClosedMenu { private get; set; } = null;

        protected override void Awake()
        {
            base.Awake();
            menuView.gameObject.SetActive(false);
        }

        public void Initialize()
        {
            menuView.onClickOperateGuideButton = OpenOperationDesctiption;
            menuView.onClickItemButton = OpenItemListView;
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

        private void OpenItemListView()
        {
            ItemListViewer v = Instantiate(itemListViewerPref, instanceParent);
            v.Initialize();
            v.ViewList(ItemListViewer.ViewMode.Playing);
        }
        private void OpenBackToTitleDialog()
        {
            menuView.HideButtons();
            string message = @"タイトルに戻りますか？
（セーブしていないデータは破棄されます）";
            DialogManager.Instance.OpenTemplateDialog(message, TempDialogType.YesOrNo, (isBack) =>
            {
                if (isBack)
                {
                    SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Title", true, null, FadeManager.FadeColorType.Black, FadeManager.FadeColorType.Black);
                }
                else
                {
                    menuView.ShowButtons();
                }
            });
        }
    }
}