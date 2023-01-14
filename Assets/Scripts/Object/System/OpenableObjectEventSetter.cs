using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 引き出しの中のアイテムなど、開けたら取得可能になる機能を設定する
/// </summary>
[RequireComponent(typeof(OpenableObjectBase))]
public class OpenableObjectEventSetter : MonoBehaviour
{
    [SerializeField] private OpenableObjectBase openableObject;
    [SerializeField] private bool isSetParent;//引き出しの時など、動いたオブジェクトに追従させたい時にtrue
    [SerializeField] private string[] inItemKeys;

    void Start()
    {
        if (inItemKeys.Length <= 0) return;
        foreach(var v in inItemKeys)
        {
            ItemObject item = ItemManager.Instance.GetItemObjectWithKey(v);
            if(item == null)
            {
                Debug.LogError($"item key is not found :: {v}");
                continue;
            }
            openableObject.onOpened += ()=> { if (item != null) { item.DoGettableItem(); } } ;
            openableObject.onClosed += () => { if (item != null) { item.DoNotGettableItem(); } };
            if (isSetParent)
            {
                item.SetHiddenParent(this.transform);
            }
        }
    }

#if UNITY_EDITOR
    void Reset()
    {
        openableObject = GetComponent<OpenableObjectBase>();
    }
#endif
}
