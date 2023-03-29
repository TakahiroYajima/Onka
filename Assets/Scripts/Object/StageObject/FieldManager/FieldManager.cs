using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;
using SoundDistance;
using Onka.Manager.Event;
using Onka.Manager.Menu;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// フィールド（家）のルートにアタッチする
/// </summary>
public class FieldManager : MonoBehaviour
{
    private List<OpenableObjectEventSetterController> openableObjectEventSetterControllerList = new List<OpenableObjectEventSetterController>();

    [field: SerializeField] public GameObject restartPosition { get; private set; } = null;//セーブ地点から再開する時の場所
    [SerializeField] private SoundDistancePoint savePointSDP = null;
    public SoundDistancePoint SavePointSDP { get { return savePointSDP; } }
    [SerializeField] private List<WanderingPoint> yukieInitWanderingPoints = new List<WanderingPoint>();
    public IReadOnlyList<WanderingPoint> YukieInitWanderingPoints { get { return yukieInitWanderingPoints; } }
    [SerializeField] private List<SoundDistancePoint> yukieInitInstancePoints = new List<SoundDistancePoint>();
    public IReadOnlyList<SoundDistancePoint> YukieInitInstancePoints { get { return yukieInitInstancePoints; } }

    [SerializeField] private KeyLockHoleSetupController keyLockHoleSetupController = null;

    [SerializeField, ReadOnly] private AreaParent[] areaParents = null;

    public void SetUp()
    {
        keyLockHoleSetupController.SetUp();
        foreach (var v in areaParents)
        {
            v.SetUp();
        }
    }

    public void AddOpenableObjectEventSetterController(OpenableObjectEventSetterController script)
    {
        if (!openableObjectEventSetterControllerList.Contains(script))
        {
            //script.SetUp();
            openableObjectEventSetterControllerList.Add(script);
        }
    }

#if UNITY_EDITOR
    public void SetAreaParents()
    {
        areaParents = GetComponentsInChildren<AreaParent>();
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(FieldManager))]
public class FieldManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var component = (FieldManager)target;
        //AreaParentを一斉にセットするボタンを表示
        if (GUILayout.Button("SetAreaParent"))
        {
            //https://light11.hatenadiary.com/entry/2019/10/12/170109
            //https://bluebirdofoz.hatenablog.com/entry/2021/08/17/224314
            component.SetAreaParents();
            EditorUtility.SetDirty(component);
        }
    }
}
#endif