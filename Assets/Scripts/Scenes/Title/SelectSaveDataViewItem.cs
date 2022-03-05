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
            titleText.text = $"データ {(id + 1).ToString()}";
            dateTimeText.text = saveDateTime;
            activeBase.SetActive(true);
            noDataTextObj.SetActive(false);
        }
        else
        {
            activeBase.SetActive(false);
            noDataTextObj.SetActive(true);
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
