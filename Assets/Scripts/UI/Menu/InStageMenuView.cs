using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Onka.Manager.Menu
{
    public class InStageMenuView : MonoBehaviour
    {
        [SerializeField] private GameObject buttonRootObj = null;
        [SerializeField] private Text operationText = null;
        [SerializeField] private Text ItemText = null;
        [SerializeField] private Text backToTitleText = null;
        [SerializeField] private Text backText = null;
        public UnityAction onClickOperateGuideButton = null;
        public UnityAction onClickItemButton = null;
        public UnityAction onClickBackToTitleButton = null;
        public UnityAction onClickCanselButton = null;

        public void Initialize()
        {
            operationText.text = TextMaster.GetText("text_menu_content_operation");
            ItemText.text = TextMaster.GetText("text_menu_content_item");
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
        public void ClickItemButton()
        {
            if(onClickItemButton != null)
            {
                onClickItemButton();
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