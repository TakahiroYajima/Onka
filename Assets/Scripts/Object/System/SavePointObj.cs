using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Onka.Manager.Data;

/// <summary>
/// セーブポイントにアタッチする
/// </summary>
public class SavePointObj : MonoBehaviour
{
    public void StartSaveAction()
    {
        DialogManager.Instance.OpenTemplateDialog("セーブしますか？", TempDialogType.YesOrNo, SaveAction);
    }

    private void SaveAction(bool isSave)
    {
        if (isSave)
        {
            DataManager.Instance.SaveGameData();
        }
    }
}
