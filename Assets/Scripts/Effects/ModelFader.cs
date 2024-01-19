using System;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//https://gametukurikata.com/shader/maketheobjectinfrontofthecamerasemitransparent
public class ModelFader : MonoBehaviour
{
    [SerializeField]
    private Renderer[] renderers;
    
    private Material[] materials;

    private const float FadeTime = 1.5f;
    private const float WaitTime = 1.5f;

    public void SetStart(Action onComplete)
    {
        SetupMaterial();
        StartCoroutine(FadeModel(onComplete));
    }

    private IEnumerator FadeModel(Action onComplete)
    {
        yield return FadeInModel();
        yield return new WaitForSeconds(WaitTime);
        yield return FadeOutModel();
        onComplete?.Invoke();
    }

    private IEnumerator FadeInModel()
    {
        var time = 0f;
        while (time < FadeTime)
        {
            foreach (var material in materials)
            {
                material.SetFloat("_Alpha", Mathf.Lerp(0f, 1f, time / FadeTime));
            }
            time += Time.deltaTime;
            yield return null;
        }
        foreach (var material in materials)
        {
            material.SetFloat("_Alpha", 1f);
        }
    }

    private IEnumerator FadeOutModel()
    {
        var time = 0f;
        while (time < FadeTime)
        {
            foreach (var material in materials)
            {
                material.SetFloat("_Alpha", Mathf.Lerp(1f, 0f, time / FadeTime));
            }
            time += Time.deltaTime;
            yield return null;
        }
        foreach (var material in materials)
        {
            material.SetFloat("_Alpha", 0f);
        }
    }

    public void SetupMaterial()
    {
        //renderers = GetComponentsInChildren<Renderer>();
        //　Renderer分の配列要素を確保
        materials = new Material[renderers.Length];
        //　Rendererからマテリアルを取得
        var i = 0;
        foreach (var renderer in renderers)
        {
            materials[i] = renderer.material;
            i++;
        }
    }
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(ModelFader))]
//public class ModelFaderEditor : Editor
//{

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        var component = (ModelFader)target;
//        if (GUILayout.Button("SetupMaterial"))
//        {
//            component.SetupMaterial();
//            EditorUtility.SetDirty(component);
//        }
//    }
//}
//#endif