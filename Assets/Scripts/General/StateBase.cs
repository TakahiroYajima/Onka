using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class StateBase
{
    //[SerializeField] protected GameObject[] rootObj = null;

    private bool isInitialized = false;
    //private bool isSuspended = false;

    //private void OnApplicationPause(bool isPause)
    //{
    //    if (isPause)
    //    {
    //        if (this.isSuspended) return;
    //        this.isSuspended = true;
    //        this.OnSuspend();
    //    }
    //    else
    //    {
    //        if (!this.isSuspended) return;
    //        this.isSuspended = false;
    //        this.OnResume();
    //    }
    //}

    //public virtual void OnSuspend() { }
    //public virtual void OnResume() { }

    protected void Awake()
    {
        this.Initialize();
    }

    protected virtual void Initialize()
    {
        if (this.isInitialized)
        {
            return;
        }

        //if (this.rootObj == null)
        //{
        //    this.rootObj = new GameObject[0];
        //}

        this.isInitialized = true;
    }

    //public abstract void InitManagerSetting<Types>(Types manager);
    public abstract void StartAction();
    public abstract void UpdateAction();
    public abstract void EndAction();
}
