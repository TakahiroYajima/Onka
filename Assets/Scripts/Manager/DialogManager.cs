using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogManager : SingletonMonoBehaviour<DialogManager>
{
    [SerializeField] private GameObject canvas = null;
    [SerializeField] private Transform dialogParent = null;

    [SerializeField] private TemplateDialog templateDialogPref = null;
    [SerializeField] private TemplateDialog templateMessageBoxDialogPref = null;

    // Start is called before the first frame update
    void Start()
    {
        canvas.gameObject.SetActive(false);
    }

    private void BeforeOpenCheck()
    {
        canvas.gameObject.SetActive(true);
    }

    public void CloseCheck()
    {
        StartCoroutine(CloseCheckCoroutine());
    }
    private IEnumerator CloseCheckCoroutine()
    {
        yield return null;
        if (dialogParent.childCount == 0)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    public void OpenTemplateDialog(string message, TempDialogType tempDialogType, UnityAction<bool> callback)
    {
        BeforeOpenCheck();
        TemplateDialog instance = Instantiate(templateDialogPref, dialogParent);
        instance.Open(message, tempDialogType, callback);
    }

    public void OpenTemplateMessageBoxDialog(string message, TempDialogType tempDialogType, UnityAction<bool> callback)
    {
        BeforeOpenCheck();
        TemplateDialog instance = Instantiate(templateMessageBoxDialogPref, dialogParent);
        instance.Open(message, tempDialogType, callback);
    }

    //public void CloseAllDialogs()
    //{
    //    foreach(Transform child in dialogParent)
    //    {

    //    }
    //}
}
