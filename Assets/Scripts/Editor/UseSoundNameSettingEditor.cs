using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using Onka.Manager.Data;

public class UseSoundNameSettingEditor : EditorWindow
{
    private static UseSoundNameSettingEditor thisEditor = null;

    private Vector2 eventScrolPos = Vector2.zero;
    public Color defaultColor { get; private set; }

    private UseSoundNameSO scriptableObject = null;
    private List<bool> settingDataActiveList = new List<bool>();

    private readonly SaveType saveType = SaveType.MasterData;

    [MenuItem("Editor/シーン別使用サウンド設定")]
    public static void Create()
    {
        if (thisEditor == null)
        {
            thisEditor = ScriptableObject.CreateInstance<UseSoundNameSettingEditor>();
        }
        thisEditor.Init();
        thisEditor.ShowUtility();

        thisEditor.defaultColor = new Color();
    }

    private void Init()
    {
        scriptableObject = FileManager.LoadSaveData<UseSoundNameSO>(saveType, DataManager.UseSoundNameFileName);
        if (scriptableObject == null || scriptableObject == default)
        {
            scriptableObject = new UseSoundNameSO();
        }
        else
        {
            Import();
        }
        defaultColor = GUI.backgroundColor;
    }

    private void OnGUI()
    {
        if (scriptableObject == null || scriptableObject == default)
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
            }
            GUI.backgroundColor = Color.gray;
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label("設定");
            }
            GUI.backgroundColor = defaultColor;

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("サウンド情報を追加", GUILayout.Width(120)))
                {
                    scriptableObject.useSoundNameDataList.Add(new UseSoundNameData());
                    settingDataActiveList.Add(false);
                }
            }
            EditorGUILayout.EndHorizontal();

            eventScrolPos = EditorGUILayout.BeginScrollView(eventScrolPos, GUI.skin.box);
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    for (int findID = 0; findID < scriptableObject.useSoundNameDataList.Count; findID++)
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        {
                            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
                            {
                                GUILayout.Label("サウンド" + (findID + 1) + " " + scriptableObject.useSoundNameDataList[findID].key);
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
                                        scriptableObject.useSoundNameDataList[findID].scene = (SceneType)EditorGUILayout.EnumPopup("使用するシーン", scriptableObject.useSoundNameDataList[findID].scene);
                                        EditorGUILayout.BeginHorizontal();
                                        {
                                            scriptableObject.useSoundNameDataList[findID].key = EditorGUILayout.TextField("サウンドのキー  ：  ", scriptableObject.useSoundNameDataList[findID].key);
                                        }
                                        EditorGUILayout.EndHorizontal();
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
        if (scriptableObject == null || scriptableObject == default)
        {
            scriptableObject = new UseSoundNameSO();
        }

        UseSoundNameSO sData = FileManager.LoadSaveData<UseSoundNameSO>(saveType, DataManager.UseSoundNameFileName);
        Debug.Log(sData.useSoundNameDataList.Count);
        if (sData == null || sData == default) { return; }

        settingDataActiveList.Clear();
        for (int i = 0; i < scriptableObject.useSoundNameDataList.Count; i++)
        {
            settingDataActiveList.Add(false);
        }
    }
    /// <summary>
    /// データの書き出し
    /// </summary>
    void Export()
    {
        // 読み込み
        if (scriptableObject == null || scriptableObject == default)
        {
            Debug.Log("NewClass");
            scriptableObject = new UseSoundNameSO();//ScriptableObject.CreateInstance<UseSoundNameSO>();
        }

        FileManager.DataSave<UseSoundNameSO>(scriptableObject, saveType, DataManager.UseSoundNameFileName, () =>
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
