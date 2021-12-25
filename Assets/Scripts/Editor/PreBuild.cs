using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class PreBuild : IPreprocessBuildWithReport
{

    public int callbackOrder { get { return 0; } } // ビルド前処理の中での処理優先順位 (0で最高)
    public void OnPreprocessBuild(BuildReport report)
    {
        Debug.Log("ビルド前処理：セーブデータの初期化をしてください");
        AssetDatabase.Refresh(); // アセットDBの更新
    }
}
