using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;
using SoundDistance;
using Onka.Manager.Event;
using Onka.Manager.Menu;

/// <summary>
/// フィールド（家）のルートにアタッチする
/// </summary>
public class FieldManager : MonoBehaviour
{
    [SerializeField] private OpenableObjectEventSetterManager openableObjectEventSetterManager;

    [SerializeField] private GameObject restartPosition = null;//セーブ地点から再開する時の場所
    [SerializeField] private SoundDistancePoint savePointSDP = null;
    public SoundDistancePoint SavePointSDP { get { return savePointSDP; } }
    [SerializeField] private List<WanderingPoint> yukieInitWanderingPoints = new List<WanderingPoint>();
    public IReadOnlyList<WanderingPoint> YukieInitWanderingPoints { get { return yukieInitWanderingPoints; } }
    [SerializeField] private List<SoundDistancePoint> yukieInitInstancePoints = new List<SoundDistancePoint>();
    public IReadOnlyList<SoundDistancePoint> YukieInitInstancePoints { get { return yukieInitInstancePoints; } }

    public void SetUp()
    {
        openableObjectEventSetterManager.SetUp();
    }
}
