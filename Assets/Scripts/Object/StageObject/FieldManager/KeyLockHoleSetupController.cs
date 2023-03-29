using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class KeyLockHoleSetupController : MonoBehaviour
{
    [SerializeField, ReadOnly] private KeyLockTarget[] keyLockTargets;
    [SerializeField, ReadOnly] private KeyHoleTarget[] keyHoleTargets;

    public void SetUp()
    {
        foreach (var v in keyLockTargets)
        {
            v.SetUp();
        }
        foreach (var v in keyHoleTargets)
        {
            v.SetUp();
        }
    }

#if UNITY_EDITOR
    public void SetObjects()
    {
        keyLockTargets = GetComponentsInChildren<KeyLockTarget>();
        keyHoleTargets = GetComponentsInChildren<KeyHoleTarget>();
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(KeyLockHoleSetupController))]
public class CustomKeyLockHoleSetupControllerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var component = (KeyLockHoleSetupController)target;
        //一斉にセットするボタンを表示
        if (GUILayout.Button("AllObjectSet"))
        {
            //https://light11.hatenadiary.com/entry/2019/10/12/170109
            //https://bluebirdofoz.hatenablog.com/entry/2021/08/17/224314
            component.SetObjects();
            EditorUtility.SetDirty(component);
        }
    }
}
#endif