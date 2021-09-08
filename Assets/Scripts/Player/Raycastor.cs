using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Rayを飛ばしてオブジェクトを取得するクラス
/// </summary>
public class Raycastor : MonoBehaviour
{
    [SerializeField] private Camera camera = null;
    private const float RayDirection = 3f;

    public void ScreenToRayAction(UnityAction<RaycastHit> hitCallback)
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * RayDirection, Color.red, 0.5f, false);

        if (Physics.Raycast(ray, out hit, RayDirection))
        {
            hitCallback(hit);
        }
    }

    public void ObjectToRayAction(Vector3 startPosition, Vector3 taragetPosition, UnityAction<RaycastHit> hitCallback, float rayDirection = RayDirection)
    {
        RaycastHit hit;
        Vector3 normal = (taragetPosition - startPosition).normalized;
        if (Physics.Raycast(startPosition, normal, out hit, rayDirection))
        {
            hitCallback(hit);
        }
    }
    public RaycastHit ObjectToRayAction(Vector3 startPosition, Vector3 taragetPosition, float rayDirection = RayDirection)
    {
        RaycastHit hit;
        Vector3 normal = (taragetPosition - startPosition).normalized;
        if (Physics.Raycast(startPosition, normal, out hit, rayDirection))
        {
            return hit;
        }
        return default;
    }
    /// <summary>
    /// Rayが当たった対象のタグが指定したタグと一致しているか
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="taragetPosition"></param>
    /// <param name="targetTag"></param>
    /// <param name="rayDirection"></param>
    /// <returns></returns>
    public bool IsRaycastHitObjectMatch(Vector3 startPosition, Vector3 taragetPosition, string targetTag, float rayDirection = RayDirection)
    {
        RaycastHit hit;
        Vector3 normal = (taragetPosition - startPosition).normalized;
        if (Physics.Raycast(startPosition, normal, out hit, rayDirection))
        {
            if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, targetTag))
            {
                return true;
            }
        }
        return false;
    }
}
