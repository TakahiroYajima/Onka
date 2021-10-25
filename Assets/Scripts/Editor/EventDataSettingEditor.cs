using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class EventDataSettingEditor : EditorWindow
{
    private static EventDataSettingEditor thisEditor = null;
    private Vector2 eventScrolPos = Vector2.zero;
    public Color defaultColor { get; private set; }

    private EventDataList scriptableObject = null;
    [SerializeField] private ReorderableList reorderableList;

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
        scriptableObject = FileManager.LoadSaveData<EventDataList>(SaveType.Normal, DataManager.EventDataFileName);
        if (scriptableObject == null)
        {
            scriptableObject = new EventDataList();
        }
        else
        {
            Import();
        }
        defaultColor = GUI.backgroundColor;
        CreateReorderableList();
    }

    private void CreateReorderableList()
    {
        reorderableList = new ReorderableList(scriptableObject.list, typeof(EventData))
        {

            //要素の追加・削除ができないようにさせる
            onCanAddCallback = (ReorderableList list) =>
            {
                return false;
            },
            onCanRemoveCallback = (ReorderableList list) =>
            {
                return false;
            },

            drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "イベントデータ");
            },

            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                rect = EditorGUI.PrefixLabel(rect, new GUIContent("イベントキー  " + (index + 1)));
                scriptableObject.list[index].eventKey = EditorGUI.TextField(rect, scriptableObject.list[index].eventKey);
                rect.y += rect.height;
            },
            elementHeightCallback = index => EditorGUIUtility.singleLineHeight * 2,
        };
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
                    //for (int findID = 0; findID < scriptableObject.list.Count; findID++)
                    //{
                    //    scriptableObject.list[findID].eventKey = EditorGUILayout.TextField("イベントキー", scriptableObject.list[findID].eventKey);
                    //}
                    this.reorderableList.DoLayoutList();
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
            scriptableObject = FileManager.LoadSaveData<EventDataList>(SaveType.Normal, DataManager.EventDataFileName);
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

        FileManager.DataSave<EventDataList>(scriptableObject, SaveType.Normal, DataManager.EventDataFileName, () =>
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
