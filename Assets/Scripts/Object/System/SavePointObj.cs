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
        DialogManager.Instance.OpenTemplateDialog(TextMaster.GetText("text_save_point_dialog_title"), TempDialogType.YesOrNo, SaveAction);
    }

    private void SaveAction(bool isSave)
    {
        if (isSave)
        {
            DataManager.Instance.SaveGameData();
        }
    }
}
