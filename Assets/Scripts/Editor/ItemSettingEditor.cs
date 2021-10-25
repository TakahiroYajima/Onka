using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class ItemSettingEditor : EditorWindow
{
    private static ItemSettingEditor thisEditor = null;

    private Vector2 eventScrolPos = Vector2.zero;
    public Color defaultColor { get; private set; }

    private ItemDataList scriptableObject = null;
    private List<bool> settingDataActiveList = new List<bool>();

    [MenuItem("Editor/アイテムデータ設定")]
    public static void Create()
    {
        if (thisEditor == null)
        {
            thisEditor = ScriptableObject.CreateInstance<ItemSettingEditor>();
        }
        thisEditor.Init();
        thisEditor.ShowUtility();

        thisEditor.defaultColor = new Color();
    }

    private void Init()
    {
        scriptableObject = FileManager.LoadSaveData<ItemDataList>(SaveType.Normal, DataManager.ItemDataFileName);
        if (scriptableObject == null)
        {
            scriptableObject = new ItemDataList();
        }
        else
        {
            Import();
        }
        defaultColor = GUI.backgroundColor;
    }
    
    private void OnGUI()
    {
        if(ItemReorderableListEditor.thisEditor == null)
        {
            NormalLayout();
        }
    }

    private void NormalLayout()
    {
        if (scriptableObject == null)
        {
            Import();
        }

        EditorGUILayout.LabelField("アイテム設定エディタ");
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
                if (GUILayout.Button("並び替え"))
                {
                    CreateReorderableEditor();
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
                if (GUILayout.Button("アイテムを追加", GUILayout.Width(120)))
                {
                    scriptableObject.itemDataList.Add(new ItemData());
                    settingDataActiveList.Add(false);
                }
            }
            EditorGUILayout.EndHorizontal();

            eventScrolPos = EditorGUILayout.BeginScrollView(eventScrolPos, GUI.skin.box);
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    for (int findID = 0; findID < scriptableObject.itemDataList.Count; findID++)
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        {
                            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                            {
                                GUILayout.Label("アイテム" + (findID + 1) + " " + scriptableObject.itemDataList[findID].name);
                            }
                            EditorGUILayout.BeginHorizontal();
                            {
                                string s = "+";
                                bool act = !settingDataActiveList[findID];
                                s = GetPlusOrMinus(settingDataActiveList[findID]);
                                if (GUILayout.Button(s, GUILayout.Width(20)))
                                {
                                    settingDataActiveList[findID] = act;
                                }

                                if (settingDataActiveList[findID])
                                {
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    {
                                        scriptableObject.itemDataList[findID].key = EditorGUILayout.TextField("Key", scriptableObject.itemDataList[findID].key);
                                        scriptableObject.itemDataList[findID].type = (ItemType)EditorGUILayout.EnumPopup("種類", scriptableObject.itemDataList[findID].type);
                                        scriptableObject.itemDataList[findID].name = EditorGUILayout.TextField("アイテム名", scriptableObject.itemDataList[findID].name);
                                        scriptableObject.itemDataList[findID].spriteName = EditorGUILayout.TextField("ファイル名", scriptableObject.itemDataList[findID].spriteName);
                                        scriptableObject.itemDataList[findID].description = EditorGUILayout.TextField("説明", scriptableObject.itemDataList[findID].description);

                                        if (scriptableObject.itemDataList[findID].type == ItemType.WatchOnly)
                                        {
                                            //新規作成の時は生成
                                            if (scriptableObject.itemDataList[findID].fileItem == null) { scriptableObject.itemDataList[findID].fileItem = new FileItem(); }
                                            EditorGUILayout.BeginHorizontal(GUI.skin.box);
                                            {
                                                GUILayout.Space(30);
                                                EditorGUILayout.BeginVertical(GUI.skin.box);
                                                {
                                                    GUI.backgroundColor = Color.gray;
                                                    using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                                                    {
                                                        GUILayout.Label("記憶ファイル設定");
                                                    }
                                                    scriptableObject.itemDataList[findID].fileItem.fontType = (FontType)EditorGUILayout.EnumPopup("フォント選択", scriptableObject.itemDataList[findID].fileItem.fontType);
                                                    Color color = scriptableObject.itemDataList[findID].fileItem.GetTextColor();
                                                    color = EditorGUILayout.ColorField("テキストカラー", color);
                                                    scriptableObject.itemDataList[findID].fileItem.red = (int)(color.r * 255f);
                                                    scriptableObject.itemDataList[findID].fileItem.green = (int)(color.g * 255f);
                                                    scriptableObject.itemDataList[findID].fileItem.blue = (int)(color.b * 255f);
                                                    scriptableObject.itemDataList[findID].fileItem.alpha = (int)(color.a * 100f);
                                                    GUILayout.Space(30);
                                                    using (new GUILayout.HorizontalScope(GUI.skin.box))
                                                    {
                                                        if (GUILayout.Button("ファイル追加"))
                                                        {
                                                            scriptableObject.itemDataList[findID].fileItem.content.Add("");
                                                        }
                                                    }
                                                    GUI.backgroundColor = defaultColor;

                                                    for (int fileCount = 0; fileCount < scriptableObject.itemDataList[findID].fileItem.content.Count; fileCount++)
                                                    {
                                                        EditorGUILayout.BeginVertical(GUI.skin.box);
                                                        {
                                                            EditorGUILayout.LabelField("ページ " + (fileCount + 1));
                                                            EditorGUILayout.BeginHorizontal(GUI.skin.box);
                                                            {
                                                                scriptableObject.itemDataList[findID].fileItem.content[fileCount] = EditorGUILayout.TextArea(scriptableObject.itemDataList[findID].fileItem.content[fileCount]);
                                                                GUILayout.Space(10);
                                                                if (GUILayout.Button("ページ削除", GUILayout.Width(80)))
                                                                {
                                                                    scriptableObject.itemDataList[findID].fileItem.content.RemoveAt(fileCount);
                                                                }
                                                            }
                                                            EditorGUILayout.EndHorizontal();
                                                        }
                                                        EditorGUILayout.EndVertical();
                                                    }
                                                }
                                                EditorGUILayout.EndVertical();
                                            }
                                            EditorGUILayout.EndHorizontal();
                                        }

                                    }
                                    EditorGUILayout.EndVertical();
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    /// <summary>
    /// アクティブ判定から展開・閉じるの文字を返す
    /// </summary>
    /// <param name="targetActiveList"></param>
    /// <returns></returns>
    public string GetPlusOrMinus(bool targetActiveList)
    {
        if (!targetActiveList) return "+";
        return "-";
    }

    /// <summary>
    /// データの読み込み
    /// </summary>
    void Import()
    {
        if (scriptableObject == null)
        {
            scriptableObject = FileManager.LoadSaveData<ItemDataList>(SaveType.Normal, DataManager.ItemDataFileName);
        }

        ItemDataList sData = FileManager.LoadSaveData<ItemDataList>(SaveType.Normal, DataManager.ItemDataFileName);
        if (scriptableObject == null || scriptableObject == default) { return; }

        settingDataActiveList.Clear();
        for (int i = 0; i < scriptableObject.itemDataList.Count; i++)
        {
            settingDataActiveList.Add(false);
        }
    }
    /// <summary>
    /// データの書き出し
    /// </summary>
    void Export()
    {
        if (scriptableObject == null || scriptableObject == default)
        {
            Debug.Log("NewClass");
            scriptableObject = new ItemDataList();
        }
        scriptableObject.Save();//データ調整

        FileManager.DataSave<ItemDataList>(scriptableObject, SaveType.Normal, DataManager.ItemDataFileName, () =>
        {
            DebugUtilityMenu.UpdateItemData();
            // エディタを最新の状態にする
            AssetDatabase.Refresh();
        });
    }
    /// <summary>
    /// 並び替えエディタの作成
    /// </summary>
    void CreateReorderableEditor()
    {
        ItemReorderableListEditor.Create();
        ItemReorderableListEditor.thisEditor.Init(scriptableObject);
    }

    //エディタ消去時に呼び出される
    private void OnDestroy()
    {
        thisEditor = null;
    }
}

public class ItemReorderableListEditor : EditorWindow
{
    public static ItemReorderableListEditor thisEditor = null;
    private ItemDataList scriptableObject = null;

    private Vector2 eventScrolPos = Vector2.zero;
    public Color defaultColor { get; private set; }
    [SerializeField] private ReorderableList reorderableList;
    private bool isInitialized = false;

    public static void Create()
    {
        if (thisEditor == null)
        {
            thisEditor = ScriptableObject.CreateInstance<ItemReorderableListEditor>();
        }
        thisEditor.ShowUtility();

        thisEditor.defaultColor = new Color();
    }

    public void Init(ItemDataList _scriptableObject)
    {
        scriptableObject = _scriptableObject;
        defaultColor = GUI.backgroundColor;
        CreateReorderableList();
        isInitialized = true;
    }

    private void CreateReorderableList()
    {
        reorderableList = new ReorderableList(scriptableObject.itemDataList, typeof(ItemData))
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
                EditorGUI.LabelField(rect, "アイテムデータ");
            },

            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(rect, new GUIContent(scriptableObject.itemDataList[index].name));

            },
            //中身を表示する判定がtrueになっているかどうかで表示エリアの高さを変える
            elementHeightCallback = (index) =>
            {
                return EditorGUIUtility.singleLineHeight * 1.2f;
            },
        };
    }

    private void OnGUI()
    {
        if (!isInitialized) return;

        EditorGUILayout.LabelField("アイテム並び替えエディタ");
        defaultColor = GUI.backgroundColor;
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            eventScrolPos = EditorGUILayout.BeginScrollView(eventScrolPos, GUI.skin.box);
            {
                EditorGUILayout.BeginVertical();
                {
                    this.reorderableList.DoLayoutList();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    //エディタ消去時に呼び出される
    private void OnDestroy()
    {
        thisEditor = null;
    }
}