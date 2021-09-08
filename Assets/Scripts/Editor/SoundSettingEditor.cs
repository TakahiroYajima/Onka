using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class SoundSettingEditor : EditorWindow
{
    private static SoundSettingEditor thisEditor = null;

    private Vector2 eventScrolPos = Vector2.zero;
    public Color defaultColor { get; private set; }

    private SoundDataSO scriptableObject = null;
    private List<bool> settingDataActiveList = new List<bool>();

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
        scriptableObject = FileManager.LoadSaveData<SoundDataSO>(DataManager.SoundDataFileName);
        if (scriptableObject == null)
        {
            scriptableObject = new SoundDataSO();
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
                    for(int findID = 0; findID < scriptableObject.soundDataList.Count; findID++)
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
                                        if(scriptableObject.soundDataList[findID].type == SoundType.SE || scriptableObject.soundDataList[findID].type == SoundType.Voice)
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
            scriptableObject = FileManager.LoadSaveData<SoundDataSO>(DataManager.SoundDataFileName);
        }

        SoundDataSO sData = FileManager.LoadSaveData<SoundDataSO>(DataManager.SoundDataFileName);
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

        FileManager.DataSave<SoundDataSO>(scriptableObject, DataManager.SoundDataFileName, () =>
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
