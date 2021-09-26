using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EventDataSettingEditor : EditorWindow
{
    private static EventDataSettingEditor thisEditor = null;
    private Vector2 eventScrolPos = Vector2.zero;
    public Color defaultColor { get; private set; }

    private EventDataList scriptableObject = null;

    [MenuItem("Editor/イベントデータ設定")]
    public static void Create()
    {
        if (thisEditor == null)
        {
            thisEditor = ScriptableObject.CreateInstance<EventDataSettingEditor>();
        }
        thisEditor.Init();
        thisEditor.ShowUtility();

        thisEditor.defaultColor = new Color();
    }

    private void Init()
    {
        scriptableObject = FileManager.LoadSaveData<EventDataList>(DataManager.EventDataFileName);
        if (scriptableObject == null)
        {
            scriptableObject = new EventDataList();
        }
        else
        {
            Import();
        }
        defaultColor = GUI.backgroundColor;
    }

    private void OnGUI()
    {
        if (scriptableObject == null)
        {
            Import();
        }

        EditorGUILayout.LabelField("イベントキー登録エディタ");
        defaultColor = GUI.backgroundColor;
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            using (new GUILayout.HorizontalScope(GUI.skin.box))
            {
                if (GUILayout.Button("読み込み"))
                {
                    Import();
                }
                if (GUILayout.Button("書き込み"))
                {
                    Export();
                }
            }
            GUI.backgroundColor = Color.gray;
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label("設定");
            }
            GUI.backgroundColor = defaultColor;

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("イベントキーを追加", GUILayout.Width(120)))
                {
                    scriptableObject.list.Add(new EventData(""));
                }
            }
            EditorGUILayout.EndHorizontal();

            eventScrolPos = EditorGUILayout.BeginScrollView(eventScrolPos, GUI.skin.box);
            {
                EditorGUILayout.BeginVertical();
                {
                    for (int findID = 0; findID < scriptableObject.list.Count; findID++)
                    {
                        scriptableObject.list[findID].eventKey = EditorGUILayout.TextField("イベントキー", scriptableObject.list[findID].eventKey);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    /// <summary>
    /// データの読み込み
    /// </summary>
    void Import()
    {
        if (scriptableObject == null)
        {
            scriptableObject = FileManager.LoadSaveData<EventDataList>(DataManager.EventDataFileName);
        }

        //EventDataList eventData = FileManager.LoadSaveData<EventDataList>(DataManager.EventDataFileName);
        //if (scriptableObject == null || scriptableObject == default) { return; }
    }
    /// <summary>
    /// データの書き出し
    /// </summary>
    void Export()
    {
        if (scriptableObject == null || scriptableObject == default)
        {
            Debug.Log("NewClass");
            scriptableObject = new EventDataList();
        }
        //scriptableObject.SplitSoundDatas();

        FileManager.DataSave<EventDataList>(scriptableObject, DataManager.EventDataFileName, () =>
        {
            // エディタを最新の状態にする
            AssetDatabase.Refresh();
        });
    }

    //エディタ消去時に呼び出される
    private void OnDestroy()
    {
        thisEditor = null;
    }
}
