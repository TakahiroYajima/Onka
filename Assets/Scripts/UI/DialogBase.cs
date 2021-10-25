using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DialogBase : MonoBehaviour
{
    public virtual void Open()
    {

    }

    public virtual void Close()
    {
        Destroy(gameObject);
        DialogManager.Instance.CloseCheck();
    }
}
