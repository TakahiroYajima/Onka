using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// UseEventObjectをEventManagerへセットする初期設定用スクリプト
/// </summary>
public class UseEventObjectParent : MonoBehaviour
{
    [SerializeField, ReadOnly] private List<UseEventObject> useEventObjectList = new List<UseEventObject>();
    // Start is called before the first frame update
    void Start()
    {
        if (useEventObjectList.Count > 0)
        {
            Debug.Log($"AddEventObject : {useEventObjectList[0].objectKey}");
            Onka.Manager.Event.EventManager.Instance.AddUseEventObjects(useEventObjectList);
        }
    }

#if UNITY_EDITOR
    public void SetUseEventObjects()
    {
        useEventObjectList.Clear();
        useEventObjectList = transform.GetComponentsInChildren<UseEventObject>().ToList();
    }
#endif
}
#if UNITY_EDITOR
[CustomEditor(typeof(UseEventObjectParent))]
public class CustomSoundDistanceManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var component = (UseEventObjectParent)target;
        //イベントのタイルを一斉にセットするボタンを表示
        if (GUILayout.Button("UseEventObjectSetup"))
        {
            //https://light11.hatenadiary.com/entry/2019/10/12/170109
            //https://bluebirdofoz.hatenablog.com/entry/2021/08/17/224314
            component.SetUseEventObjects();
            EditorUtility.SetDirty(component);
        }
    }
}
#endif