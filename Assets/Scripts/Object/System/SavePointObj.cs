using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// セーブポイントにアタッチする
/// </summary>
public class SavePointObj : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartSaveAction()
    {
        DialogManager.Instance.OpenTemplateDialog("セーブしますか？", TempDialogType.YesOrNo, SaveAction);
    }

    private void SaveAction(bool isSave)
    {
        DataManager.Instance.SaveGameData();
    }
}
