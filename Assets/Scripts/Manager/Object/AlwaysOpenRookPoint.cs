using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysOpenRookPoint : MonoBehaviour
{
    [field: SerializeField] public AlwaysOpenRookPointType pointType { get; private set; }
    //pointType = OnThe2Fの時は設定なしでよい
    [field: SerializeField] public RoomWanderingManager targetRoom { get; private set; }

}
public enum AlwaysOpenRookPointType
{
    OnThe2F,//2階にいるかのみ判断
    InRoom,//部屋に入っているかを判断
}
