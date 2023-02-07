﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 部屋内のオブジェクトを管理する。各エリアに必ず付与され、動的生成するオブジェクトはここで生成管理
/// </summary>
public class AreaParent : MonoBehaviour
{
    [SerializeField] private GameObject staticObjectPrefab = null;
    [SerializeField] private OpenableObjectEventSetterController haveItemObjectControllerPrefab = null;
    [SerializeField] private UseEventObjectParent useEventObjectParentPrefab = null;

    private GameObject staticObject = null;
    private OpenableObjectEventSetterController haveItemObject = null;
    private UseEventObjectParent useEventObject = null;

    [SerializeField] private bool isInitInstantiate = false;//trueにするとゲーム開始時に生成される
    //注意：プレイヤーにしか当たらないようにコライダーを設定する。生成したらコライダーを破棄する
    [SerializeField] private CollisionEnterEvent objectInstantiateJudgeCollision = null;

    public void SetUp()
    {
        if(objectInstantiateJudgeCollision != null)
        {
            objectInstantiateJudgeCollision.InitSetTriggerEnter(OnInstantiateJudgeColliderEnter);
        }
        if (isInitInstantiate)
        {
            AllInstantiate();
        }
    }

    public void OnInstantiateJudgeColliderEnter()
    {
        if (Utility.Instance.IsTagNameMatch(objectInstantiateJudgeCollision.HitCollider.gameObject, Tags.Player))
        {
            AllInstantiate();
        }
    }

    public void AllInstantiate()
    {
        InstantiateStaticObject();
        InstantiateOpenableObject();
        InstantiateUseEventObject();
        DestroyCollider();
    }

    public void InstantiateStaticObject()
    {
        if(staticObject == null && staticObjectPrefab != null)
        {
            staticObject = Instantiate(staticObjectPrefab, this.transform);
        }
    }
    public void InstantiateOpenableObject()
    {
        if(haveItemObject == null && haveItemObjectControllerPrefab != null)
        {
            haveItemObject = Instantiate(haveItemObjectControllerPrefab, this.transform);
        }
    }
    public void InstantiateUseEventObject()
    {
        if(useEventObject == null && useEventObjectParentPrefab != null)
        {
            useEventObject = Instantiate(useEventObjectParentPrefab, this.transform);
        }
    }

    private void DestroyCollider()
    {
        if(objectInstantiateJudgeCollision != null)
        {
            Destroy(objectInstantiateJudgeCollision);
        }
    }
}