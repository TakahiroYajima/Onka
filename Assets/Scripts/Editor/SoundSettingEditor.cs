using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class SoundSettingEditor : EditorWindow
{
    private static SoundSettingEditor thisEditor = null;

    private Vector2 eventScrolPos = Vector2.zero;
    public Color defaultColor { get; private set; }

    private SoundDataSO scriptableObject = null;
    private List<bool> settingDataActiveList = new List<bool>();

    //[SerializeField] private ReorderableList reorderableList;

    [MenuItem("Editor/サウンドデータ設定")]
    public static void Create()
    {
        if (thisEditor == null)
        {
            thisEditor = ScriptableObject.CreateInstance<SoundSettingEditor>();
        }
        thisEditor.Init();
        thisEditor.ShowUtility();

        thisEditor.defaultColor = new Color();
    }

    private void Init()
    {
        scriptableObject = FileManager.LoadSaveData<SoundDataSO>(SaveType.Normal, DataManager.SoundDataFileName);
        if (scriptableObject == null)
        {
            scriptableObject = new SoundDataSO();
        }
        else
        {
            Import();
        }
        defaultColor = GUI.backgroundColor;
        //CreateReorderableList();
    }

    //private void CreateReorderableList()
    //{
    //    reorderableList = new ReorderableList(scriptableObject.soundDataList, typeof(SoundData))
    //    {

    //        //要素の追加・削除ができないようにさせる
    //        onCanAddCallback = (ReorderableList list) =>
    //        {
    //            return false;
    //        },
    //        onCanRemoveCallback = (ReorderableList list) =>
    //        {
    //            return false;
    //        },

    //        drawHeaderCallback = (rect) =>
    //        {
    //            EditorGUI.LabelField(rect, "サウンドデータ");
    //        },

    //        drawElementCallback = (rect, index, isActive, isFocused) =>
    //        {
    //            rect.height = EditorGUIUtility.singleLineHeight;
    //            rect = EditorGUI.PrefixLabel(rect, new GUIContent("サウンド  " + (index + 1) + " " + scriptableObject.soundDataList[index].title));
    //            //EditorGUI.TextField(rect, scriptableObject.list[index].eventKey);
    //            //+ - ボタン
    //            string s = "+";
    //            bool act = !settingDataActiveList[index];
    //            s = GetPlusOrMinus(settingDataActiveList[index]);
    //            if (GUI.Button(rect, s))
    //            {
    //                settingDataActiveList[index] = act;
    //            }

    //            rect.y += rect.height;
    //            if (settingDataActiveList[index])
    //            {
    //                Rect initRect = new Rect(rect);
    //                //プロパティを描画するごとに次のプロパティが表示できる位置までYを下げる。Xは、ラベルを描くとなぜか右にずれるので初期位置に修正させる
    //                UnityEngine.Events.UnityAction onUpdateRectTemp = () => { rect.y += rect.height + 2; rect.x = initRect.x; };

    //                rect = EditorGUI.PrefixLabel(rect, new GUIContent("Key"));
    //                scriptableObject.soundDataList[index].key = EditorGUI.TextField(rect, scriptableObject.soundDataList[index].key);

    //                onUpdateRectTemp();
    //                rect = EditorGUI.PrefixLabel(rect, new GUIContent("Type"));
    //                scriptableObject.soundDataList[index].type = (SoundType)EditorGUI.EnumPopup(rect, scriptableObject.soundDataList[index].type);

    //                onUpdateRectTemp();
    //                rect = EditorGUI.PrefixLabel(rect, new GUIContent("ファイル （" + scriptableObject.soundDataList[index].soundName + "）"));
    //                AudioClip clip = null;
    //                clip = EditorGUI.ObjectField(rect, clip, typeof(AudioClip), true) as AudioClip;
    //                if (clip != null)
    //                {
    //                    scriptableObject.soundDataList[index].soundName = clip.name;
    //                }

    //                onUpdateRectTemp();
    //                rect = EditorGUI.PrefixLabel(rect, new GUIContent("タイトル"));
    //                scriptableObject.soundDataList[index].title = EditorGUI.TextField(rect, scriptableObject.soundDataList[index].title);

    //                onUpdateRectTemp();
    //                rect = EditorGUI.PrefixLabel(rect, new GUIContent("ボリューム "));
    //                scriptableObject.soundDataList[index].volume = (float)EditorGUI.Slider(rect, scriptableObject.soundDataList[index].volume, 0f, 1f);

    //                if (scriptableObject.soundDataList[index].type == SoundType.SE || scriptableObject.soundDataList[index].type == SoundType.Voice)
    //                {
    //                    onUpdateRectTemp();
    //                    rect = EditorGUI.PrefixLabel(rect, new GUIContent("2D - 3D "));
    //                    scriptableObject.soundDataList[index].spatialBlend = (float)EditorGUI.Slider(rect, scriptableObject.soundDataList[index].spatialBlend, 0f, 1f);
    //                }
    //                else
    //                {
    //                    scriptableObject.soundDataList[index].spatialBlend = 0f;
    //                }

    //            }
    //        },
    //        //中身を表示する判定がtrueになっているかどうかで表示エリアの高さを変える
    //        elementHeightCallback = (index) => {
    //            if (!settingDataActiveList[index]) { return EditorGUIUtility.singleLineHeight * 2; }
    //            else { return EditorGUIUtility.singleLineHeight * 10; }
    //            },
    //    };
    //}

    private void OnGUI()
    {
        if (SoundReorderableEditor.thisEditor != null) return;

        if (scriptableObject == null)
        {
            Import();
        }

        EditorGUILayout.LabelField("サウンド設定エディタ");
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
                if (GUILayout.Button("サウンドを追加", GUILayout.Width(120)))
                {
                    scriptableObject.soundDataList.Add(new SoundData());
                    settingDataActiveList.Add(false);
                }
            }
            EditorGUILayout.EndHorizontal();

            eventScrolPos = EditorGUILayout.BeginScrollView(eventScrolPos, GUI.skin.box);
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    //this.reorderableList.DoLayoutList();
                    for (int findID = 0; findID < scriptableObject.soundDataList.Count; findID++)
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        {
                            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                            {
                                GUILayout.Label("サウンド" + (findID + 1) + " " + scriptableObject.soundDataList[findID].title);
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
                                        scriptableObject.soundDataList[findID].key = EditorGUILayout.TextField("Key", scriptableObject.soundDataList[findID].key);
                                        scriptableObject.soundDataList[findID].type = (SoundType)EditorGUILayout.EnumPopup("種類", scriptableObject.soundDataList[findID].type);
                                        EditorGUILayout.BeginHorizontal();
                                        {
                                            GUILayout.Label("サウンドファイル  ：  " + scriptableObject.soundDataList[findID].soundName);
                                            AudioClip clip = null;
                                            clip = EditorGUILayout.ObjectField(clip, typeof(AudioClip), true) as AudioClip;
                                            if (clip != null)
                                            {
                                                scriptableObject.soundDataList[findID].soundName = clip.name;
                                            }
                                        }
                                        EditorGUILayout.EndHorizontal();
                                        scriptableObject.soundDataList[findID].title = EditorGUILayout.TextField("タイトル", scriptableObject.soundDataList[findID].title);
                                        scriptableObject.soundDataList[findID].volume = (float)EditorGUILayout.Slider("ボリューム", scriptableObject.soundDataList[findID].volume, 0f, 1f);
                                        if (scriptableObject.soundDataList[findID].type == SoundType.SE || scriptableObject.soundDataList[findID].type == SoundType.Voice)
                                        {
                                            scriptableObject.soundDataList[findID].spatialBlend = (float)EditorGUILayout.Slider("2D - 3D", scriptableObject.soundDataList[findID].spatialBlend, 0f, 1f);
                                        }
                                        else
                                        {
                                            scriptableObject.soundDataList[findID].spatialBlend = 0f;
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
            scriptableObject = FileManager.LoadSaveData<SoundDataSO>(SaveType.Normal, DataManager.SoundDataFileName);
        }

        SoundDataSO sData = FileManager.LoadSaveData<SoundDataSO>(SaveType.Normal, DataManager.SoundDataFileName);
        if (scriptableObject == null || scriptableObject == default) { return; }

        settingDataActiveList.Clear();
        for(int i = 0; i < scriptableObject.soundDataList.Count; i++)
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
            scriptableObject = new SoundDataSO();
        }
        scriptableObject.SplitSoundDatas();

        FileManager.DataSave<SoundDataSO>(scriptableObject, SaveType.Normal, DataManager.SoundDataFileName, () =>
        {
            // エディタを最新の状態にする
            AssetDatabase.Refresh();
        });
    }
    /// <summary>
    /// 並び替えエディタの作成
    /// </summary>
    void CreateReorderableEditor()
    {
        SoundReorderableEditor.Create();
        SoundReorderableEditor.thisEditor.Init(scriptableObject);
    }

    //エディタ消去時に呼び出される
    private void OnDestroy()
    {
        thisEditor = null;
    }
}

public class SoundReorderableEditor : EditorWindow
{
    public static SoundReorderableEditor thisEditor = null;
    private SoundDataSO scriptableObject = null;

    private Vector2 eventScrolPos = Vector2.zero;
    public Color defaultColor { get; private set; }
    [SerializeField] private ReorderableList reorderableList;
    private bool isInitialized = false;

    public static void Create()
    {
        if (thisEditor == null)
        {
            thisEditor = ScriptableObject.CreateInstance<SoundReorderableEditor>();
        }
        thisEditor.ShowUtility();

        thisEditor.defaultColor = new Color();
    }

    public void Init(SoundDataSO _scriptableObject)
    {
        scriptableObject = _scriptableObject;
        defaultColor = GUI.backgroundColor;
        CreateReorderableList();
        isInitialized = true;
    }

    private void CreateReorderableList()
    {
        reorderableList = new ReorderableList(scriptableObject.soundDataList, typeof(SoundData))
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
                EditorGUI.LabelField(rect, "サウンドデータ");
            },

            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(rect, new GUIContent(scriptableObject.soundDataList[index].title));

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

        EditorGUILayout.LabelField("サウンド並び替えエディタ");
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