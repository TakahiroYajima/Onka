using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SelectSaveDataViewItem : MonoBehaviour
{
    private int id = -1;
    [SerializeField] private GameObject activeBase = null;
    [SerializeField] private GameObject noDataTextObj = null;
    [SerializeField] private Image rateOfProgressionImage = null;
    [SerializeField] private Text titleText = null;
    [SerializeField] private Text dateTimeText = null;
    [SerializeField] private Text noDataText = null;

    UnityAction<int> onPress = null;

    /// <summary>
    /// 空のセーブデータかを返す
    /// </summary>
    public bool IsNoData {
        get
        {
            if (id < 0) return false;//未初期化対策
            else return noDataTextObj.activeSelf;
        }
    }

    public void Initialize(int _id, string saveDateTime, float rateOfProgression, UnityAction<int> _onPress)
    {
        id = _id;
        //Debug.Log($"セーブデータ{id} {saveDateTime}");
        if (!string.IsNullOrEmpty(saveDateTime))
        {
            rateOfProgressionImage.fillAmount = rateOfProgression;
            titleText.text = string.Format(TextMaster.GetText("text_select_save_data_item_title"), id + 1);
            dateTimeText.text = saveDateTime;
            activeBase.SetActive(true);
            noDataTextObj.SetActive(false);
        }
        else
        {
            activeBase.SetActive(false);
            noDataTextObj.SetActive(true);
            noDataText.text = TextMaster.GetText("text_select_save_data_item_no_data");
        }
        onPress = _onPress;
    }

    public void OnPressButton()
    {
        if(onPress != null)
        {
            onPress(id);
        }
    }
}
