using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Onka.Manager.Data;

public class SelectSaveDataView : MonoBehaviour
{
    [SerializeField] private SelectSaveDataViewItem selectSaveDataViewItemPref = null;
    private List<SelectSaveDataViewItem> instantedList = new List<SelectSaveDataViewItem>();
    [SerializeField] private Transform instantedListParent = null;

    private IReadOnlyList<GameData> saveDataList;

    public UnityAction<ViewSelectType> onPlayDataDecisioned = null;//遊ぶデータを決定したときの挙動
    public UnityAction onClose = null;
    public enum ViewSelectType
    {
        NewGame,
        LoadGame,
    }
    private ViewSelectType viewSelectType = ViewSelectType.LoadGame;

    public void View(ViewSelectType _viewSelectType)
    {
        viewSelectType = _viewSelectType;
        saveDataList = DataManager.Instance.GetAllGameDatas();
        InstanceList();
    }
    private void InstanceList()
    {
        for(int i = 0; i < saveDataList.Count; i++)
        {
            if(i >= instantedList.Count)
            {
                instantedList.Add(Instantiate(selectSaveDataViewItemPref, instantedListParent));
            }
            int id = i;
            instantedList[i].Initialize(id, saveDataList[i].saveDate, GetRateOfProgression(id), OnPressSaveDataButton);
        }
    }

    private float GetRateOfProgression(int dataArrayNum)
    {
        int allEventAndItemCount = saveDataList[dataArrayNum].itemDataList.itemDataList.Count + saveDataList[dataArrayNum].eventDataList.list.Count;
        allEventAndItemCount -= 2;//最後のノートとエンディングイベントはセーブできないのでその分引いておく（エンディング直前で最後のノート以外入手済みでMAXになる）
        int getedItemCount = saveDataList[dataArrayNum].itemDataList.itemDataList.FindAll(x => x.geted == true).Count;
        int endedEventCount = saveDataList[dataArrayNum].eventDataList.list.FindAll(x => x.isEnded == true).Count;
        float rateOfPregress = (float)(getedItemCount + endedEventCount) / (float)allEventAndItemCount;
        return rateOfPregress;
    }
    /// <summary>
    /// 各セーブデータのボタンをクリックした際の挙動。新規作成かデータロードで処理を分ける
    /// </summary>
    /// <param name="_selectSaveDataArrayNum"></param>
    private void OnPressSaveDataButton(int _selectSaveDataArrayNum)
    {
        //Debug.Log($"OnPressSaveDataButton {_selectSaveDataArrayNum} : {viewSelectType.ToString()}");
        if(viewSelectType == ViewSelectType.NewGame)
        {
            string message = "";
            if (instantedList[_selectSaveDataArrayNum].IsNoData)
            {
                message = "ここにセーブしますか？";
            }
            else
            {
                message = @"データを上書きします。
本当によろしいですか？";
            }
            DialogManager.Instance.OpenTemplateMessageBoxDialog(message, TempDialogType.YesOrNo, (bool _isUpdateNewGameData) =>
            {
                if (_isUpdateNewGameData)
                {
                    UpdateSaveDataWithNewGame(_selectSaveDataArrayNum);
                }
            });
        }
        else
        {
            if (instantedList[_selectSaveDataArrayNum].IsNoData) { return; }

            string message = "このデータをロードしますか？";
            DialogManager.Instance.OpenTemplateMessageBoxDialog(message, TempDialogType.YesOrNo, (bool _isOK) =>
            {
                if (_isOK)
                {
                    SelectLoadGameData(_selectSaveDataArrayNum);
                }
            });
        }
    }
    /// <summary>
    /// ニューゲーム用にセーブデータを上書きする
    /// </summary>
    private void UpdateSaveDataWithNewGame(int _selectSaveDataArrayNum)
    {
        DataManager.Instance.NewCreateGameDataAndSave(_selectSaveDataArrayNum, ()=>
        {
            if(onPlayDataDecisioned != null)
            {
                onPlayDataDecisioned(ViewSelectType.NewGame);
            }
        });
    }
    /// <summary>
    /// 続きから遊ぶデータを選択したときの挙動
    /// </summary>
    /// <param name="_selectSaveDataArrayNum"></param>
    private void SelectLoadGameData(int _selectSaveDataArrayNum)
    {
        DataManager.Instance.SelectPlayGameData(_selectSaveDataArrayNum);
        if (onPlayDataDecisioned != null)
        {
            onPlayDataDecisioned(ViewSelectType.LoadGame);
        }
    }

    public void OnPressBackButton()
    {
        if(onClose != null)
        {
            onClose();
        }
    }
}
