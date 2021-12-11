using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Utility
{
    private static Utility instance = null;
    public static Utility Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new Utility();
            }
            return instance;
        }
    }

    /// <summary>
    /// レイヤー名の一致判定を返す
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="layerName"></param>
    /// <returns></returns>
    public bool IsLayerNameMatch(GameObject obj, string layerName)
    {
        if (obj != null && layerName != string.Empty && layerName != "")
        {
            return LayerMask.LayerToName(obj.layer) == layerName;
        }
        return false;
    }

    public bool IsAnyLayerNameMatch(GameObject obj, string[] layerNames)
    {
        int hitLayerCount = layerNames.Where(x => x == LayerMask.LayerToName(obj.layer)).Count();
        if (hitLayerCount >= 1)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// タグ名の一致判定を返す
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="tagName"></param>
    /// <returns></returns>
    public bool IsTagNameMatch(GameObject obj, string tagName)
    {
        if (obj != null && tagName != string.Empty && tagName != "")
        {
            return obj.transform.tag == tagName;
        }
        return false;
    }
    public bool IsTagNameMatch(Transform tr, string tagName)
    {
        if (tr != null && tagName != string.Empty && tagName != "")
        {
            return tr.tag == tagName;
        }
        return false;
    }

    public bool IsAnyTagNameMatch(GameObject obj, string[] tagNames)
    {
        int hitTagCount = tagNames.Where(x => x == obj.transform.tag).Count();
        if (hitTagCount >= 1)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 視界に入っていたらtrueを返す
    /// </summary>
    /// <param name="myObject"></param>
    /// <param name="targetObject"></param>
    /// <param name="searchMaxAngle"></param>
    /// <returns></returns>
    public bool IsInSightAngle(GameObject myObject, GameObject targetObject, float searchMaxAngle)
    {
        return Vector3.Angle(myObject.transform.forward, (targetObject.transform.position - myObject.transform.position)) <= searchMaxAngle;
    }
}
