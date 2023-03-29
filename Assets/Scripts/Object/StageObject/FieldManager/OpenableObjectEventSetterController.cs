using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class OpenableObjectEventSetterController : MonoBehaviour
{
    [SerializeField, ReadOnly] private OpenableObjectEventSetter[] openableObjectEventSetters;

    public void SetUp()
    {
        foreach(var v in openableObjectEventSetters)
        {
            v.SetUp();
        }
    }

#if UNITY_EDITOR
    public void SetOpenableObjectEventSetter()
    {
        var list = GetComponentsInChildren<OpenableObjectEventSetter>();
        openableObjectEventSetters = list;
    }
#endif
}
#if UNITY_EDITOR
[CustomEditor(typeof(OpenableObjectEventSetterController))]
public class CustomOpenableObjectEventSetterManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var component = (OpenableObjectEventSetterController)target;
        //イベントのタイルを一斉にセットするボタンを表示
        if (GUILayout.Button("AllEventTileSetup"))
        {
            //https://light11.hatenadiary.com/entry/2019/10/12/170109
            //https://bluebirdofoz.hatenablog.com/entry/2021/08/17/224314
            component.SetOpenableObjectEventSetter();
            EditorUtility.SetDirty(component);
        }
    }
}
#endif