using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Onka.Manager.Menu
{
    public class InStageMenuView : MonoBehaviour
    {
        [SerializeField] private GameObject buttonRootObj = null;
        [SerializeField] private Text operationText = null;
        [SerializeField] private Text settingText = null;
        [SerializeField] private Text ItemText = null;
        [SerializeField] private Text hintText = null;
        [SerializeField] private Text backToTitleText = null;
        [SerializeField] private Text backText = null;
        public Action onClickOperateGuideButton = null;
        public Action onClickSettingButton = null;
        public Action onClickItemButton = null;
        public Action onClickHintButton = null;
        public Action onClickBackToTitleButton = null;
        public Action onClickCanselButton = null;

        public void Initialize()
        {
            operationText.text = TextMaster.GetText("text_menu_content_operation");
            settingText.text = TextMaster.GetText("text_menu_content_setting");
            ItemText.text = TextMaster.GetText("text_menu_content_item");
            hintText.text = TextMaster.GetText("text_menu_content_hint");
            backToTitleText.text = TextMaster.GetText("text_menu_content_back_to_title");
            backText.text = TextMaster.GetText("text_menu_content_back");
        }

        public void ClickOperateGuideButton()
        {
            if (onClickOperateGuideButton != null)
            {
                onClickOperateGuideButton();
            }
        }
        public void ClickSettingButton()
        {
            if (onClickSettingButton != null)
            {
                onClickSettingButton();
            }
        }
        public void ClickItemButton()
        {
            if(onClickItemButton != null)
            {
                onClickItemButton();
            }
        }
        public void ClickHintButton()
        {
            if (onClickHintButton != null)
            {
                onClickHintButton();
            }
        }
        public void ClickBackToTitleButton()
        {
            if(onClickBackToTitleButton != null)
            {
                onClickBackToTitleButton();
            }
        }
        public void ClickCanselButton()
        {
            if(onClickCanselButton != null)
            {
                onClickCanselButton();
            }
        }

        public void ShowButtons()
        {
            buttonRootObj.SetActive(true);
        }
        public void HideButtons()
        {
            buttonRootObj.SetActive(false);
        }
    }
}