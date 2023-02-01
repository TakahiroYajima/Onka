using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Onka.Manager.Event;

/// <summary>
/// キーを渡せば参照できるようにするオブジェクト（イベント用）
/// </summary>
public class UseEventObject : MonoBehaviour
{
    [field: SerializeField] public string objectKey { get; private set; }

}
