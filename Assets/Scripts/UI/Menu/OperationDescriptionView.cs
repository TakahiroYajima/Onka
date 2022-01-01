using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OperationDescriptionView : MonoBehaviour
{
    [SerializeField] private GameObject layoutBaseObject = null;
    public UnityAction onClosed = null;

    public void InitAndShow(UnityAction _onClosed)
    {
        layoutBaseObject.SetActive(true);
        onClosed = _onClosed;
    }

    public void Close()
    {
        layoutBaseObject.SetActive(false);
        if (onClosed != null)
        {
            onClosed();
        }
        Destroy(gameObject);
    }
}
