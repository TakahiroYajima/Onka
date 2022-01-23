using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBackgroundManager : SingletonMonoBehaviour<UIBackgroundManager>
{
    [SerializeField] private Canvas canvas = null;
    [SerializeField] private Image uiBackgroundPanel = null;
    public Image UI { get { return uiBackgroundPanel; } }
    public void ShowPanel()
    {
        canvas.gameObject.SetActive(true);
        //uiBackgroundPanel.enabled = true;
    }
    public void HidePanel()
    {
        canvas.gameObject.SetActive(false);
        //uiBackgroundPanel.enabled = false;
    }
}
