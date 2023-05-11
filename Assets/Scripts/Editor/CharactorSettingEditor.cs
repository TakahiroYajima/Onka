using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using Onka.Manager.Data;

public class CharactorSettingEditor : EditorWindow
{
    private static CharactorSettingEditor thisEditor = null;

    private Vector2 eventScrolPos = Vector2.zero;
    public Color defaultColor { get; private set; }

    private CharactorDataList dataList = null;
    private List<bool> settingDataActiveList = new List<bool>();

    private readonly SaveType saveType = SaveType.MasterData;

    [MenuItem("Editor/キャラクターデータ設定")]
    public static void Create()
    {
        if (thisEditor == null)
        {
            thisEditor = ScriptableObject.CreateInstance<CharactorSettingEditor>();
        }
        thisEditor.Init();
        thisEditor.ShowUtility();

        thisEditor.defaultColor = new Color();
    }

    private void Init()
    {
        dataList = FileManager.LoadSaveData<CharactorDataList>(saveType, DataManager.CharactorDataFileName);
        if (dataList == null)
        {
            dataList = new CharactorDataList();
        }
        else
        {
            Import();
        }
        defaultColor = GUI.backgroundColor;
    }

    private void OnGUI()
    {
        if (CharactorReorderableListEditor.thisEditor == null)
        {
            NormalLayout();
        }
    }

    private void NormalLayout()
    {
        if (dataList == null)
        {
            Import();
        }

        EditorGUILayout.LabelField("キャラクター設定エディタ");
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
                if (GUILayout.Button("キャラクターを追加", GUILayout.Width(120)))
                {
                    dataList.charactorDataList.Add(new CharactorData());
                    settingDataActiveList.Add(false);
                }
            }
            EditorGUILayout.EndHorizontal();

            eventScrolPos = EditorGUILayout.BeginScrollView(eventScrolPos, GUI.skin.box);
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    for (int findID = 0; findID < dataList.charactorDataList.Count; findID++)
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        {
                            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                            {
                                GUILayout.Label("アイテム" + (findID + 1) + " " + dataList.charactorDataList[findID].name);
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
                                    dataList.charactorDataList[findID].id = findID + 1;
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    {
                                        dataList.charactorDataList[findID].key = EditorGUILayout.TextField("Key", dataList.charactorDataList[findID].key);
                                        EditorGUILayout.BeginHorizontal();
                                        {
                                            dataList.charactorDataList[findID].name = EditorGUILayout.TextField("名前", dataList.charactorDataList[findID].name);
                                            dataList.charactorDataList[findID].name_en = EditorGUILayout.TextField("名前（英）", dataList.charactorDataList[findID].name_en);
                                            dataList.charactorDataList[findID].isCharactorName = EditorGUILayout.Toggle("名前表記？", dataList.charactorDataList[findID].isCharactorName);
                                        }
                                        EditorGUILayout.EndHorizontal();
                                        dataList.charactorDataList[findID].gender = (Gender)EditorGUILayout.EnumPopup("性別", dataList.charactorDataList[findID].gender);
                                        dataList.charactorDataList[findID].age = EditorGUILayout.IntField("年齢", dataList.charactorDataList[findID].age);
                                        EditorGUILayout.BeginHorizontal();
                                        {
                                            GUILayout.Label("画像ファイル  ：  " + dataList.charactorDataList[findID].imageName);
                                            Sprite sprite = null;
                                            sprite = EditorGUILayout.ObjectField(sprite, typeof(Sprite), true) as Sprite;
                                            if (sprite != null)
                                            {
                                                dataList.charactorDataList[findID].imageName = sprite.name;
                                            }
                                        }
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.BeginVertical(GUI.skin.box);
                                        {
                                            EditorGUILayout.LabelField("説明");
                                            EditorGUILayout.BeginHorizontal(GUI.skin.box);
                                            {
                                                dataList.charactorDataList[findID].description = EditorGUILayout.TextArea(dataList.charactorDataList[findID].description);
                                            }
                                            EditorGUILayout.EndHorizontal();
                                            EditorGUILayout.LabelField("説明（英）");
                                            EditorGUILayout.BeginHorizontal(GUI.skin.box);
                                            {
                                                dataList.charactorDataList[findID].description_en = EditorGUILayout.TextArea(dataList.charactorDataList[findID].description_en);
                                            }
                                            EditorGUILayout.EndHorizontal();
                                        }
                                        EditorGUILayout.EndVertical();
                                        
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
        if (dataList == null)
        {
            dataList = FileManager.LoadSaveData<CharactorDataList>(saveType, DataManager.CharactorDataFileName);
        }

        CharactorDataList sData = FileManager.LoadSaveData<CharactorDataList>(saveType, DataManager.CharactorDataFileName);
        if (dataList == null || dataList == default) { return; }

        settingDataActiveList.Clear();
        for (int i = 0; i < dataList.charactorDataList.Count; i++)
        {
            settingDataActiveList.Add(false);
        }
    }
    /// <summary>
    /// データの書き出し
    /// </summary>
    void Export()
    {
        if (dataList == null || dataList == default)
        {
            Debug.Log("NewClass");
            dataList = new CharactorDataList();
        }
        //dataList.Save();//データ調整

        FileManager.DataSave<CharactorDataList>(dataList, saveType, DataManager.CharactorDataFileName, () =>
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
        CharactorReorderableListEditor.Create();
        CharactorReorderableListEditor.thisEditor.Init(dataList);
    }

    //エディタ消去時に呼び出される
    private void OnDestroy()
    {
        thisEditor = null;
    }
}


public class CharactorReorderableListEditor : EditorWindow
{
    public static CharactorReorderableListEditor thisEditor = null;
    private CharactorDataList dataList = null;

    private Vector2 eventScrolPos = Vector2.zero;
    public Color defaultColor { get; private set; }
    [SerializeField] private ReorderableList reorderableList;
    private bool isInitialized = false;

    public static void Create()
    {
        if (thisEditor == null)
        {
            thisEditor = ScriptableObject.CreateInstance<CharactorReorderableListEditor>();
        }
        thisEditor.ShowUtility();

        thisEditor.defaultColor = new Color();
    }

    public void Init(CharactorDataList _dataList)
    {
        dataList = _dataList;
        defaultColor = GUI.backgroundColor;
        CreateReorderableList();
        isInitialized = true;
    }

    private void CreateReorderableList()
    {
        reorderableList = new ReorderableList(dataList.charactorDataList, typeof(CharactorData))
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
                EditorGUI.LabelField(rect, new GUIContent(dataList.charactorDataList[index].name));

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