using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Rayを飛ばしてオブジェクトを取得するクラス
/// </summary>
public class Raycastor : MonoBehaviour
{
    [SerializeField] private Camera camera = null;
    private const float RayDirection = 2f;

    public void ScreenToRayAction(UnityAction<RaycastHit> hitCallback, UnityAction noHitCallback = null)
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction * RayDirection, Color.red, 0.5f, false);

        if (Physics.Raycast(ray, out hit, RayDirection))
        {
            hitCallback(hit);
        }
        else
        {
            if(noHitCallback != null) { noHitCallback(); }
        }
    }
    public void ScreenToRayActionWithLayerMask(int layerMask, UnityAction<RaycastHit> hitCallback, UnityAction noHitCallback = null)
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * RayDirection, Color.red, 0.5f, false);

        if (Physics.Raycast(ray, out hit, RayDirection, layerMask))
        {
            hitCallback(hit);
        }
        else
        {
            if (noHitCallback != null) { noHitCallback(); }
        }
    }

    public void ScreenToRayActionIgnorePlayer(UnityAction<RaycastHit> hitCallback, UnityAction noHitCallback = null)
    {
        var hits = Physics.RaycastAll(camera.transform.position + (camera.transform.forward * 1.3f), camera.transform.forward, RayDirection).ToList();
        if (hits == null || hits == default) { noHitCallback(); return; }
        if (hits.Count == 0) { noHitCallback(); return; }
        if (hits[0].transform.tag == Tags.Player)
        {
            if (hits.Count > 1)
            {
                hitCallback(hits[1]);
            }
            else { noHitCallback(); }
        }
        else
        {
            hitCallback(hits[0]);
        }
    }

    public void ObjectToRayAction(Vector3 startPosition, Vector3 taragetPosition, UnityAction<RaycastHit> hitCallback, float rayDirection = RayDirection)
    {
        RaycastHit hit;
        Vector3 normal = (taragetPosition - startPosition).normalized;
        Debug.DrawRay(startPosition, (taragetPosition - startPosition), Color.red, 1f);
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
        //Debug.DrawRay(startPosition, (taragetPosition - startPosition), Color.red, 1f);
        
        if (Physics.Raycast(startPosition, normal, out hit, rayDirection))
        {
            //Debug.Log(hit.transform.gameObject);
            if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, targetTag))
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Rayが当たった対象のタグが指定したタグと一致しているか（レイヤーマスク付き）
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="taragetPosition"></param>
    /// <param name="targetTag"></param>
    /// <param name="layerMask"></param>
    /// <param name="rayDirection"></param>
    /// <returns></returns>
    public bool IsRaycastHitObjectMatchWithLayerMask(Vector3 startPosition, Vector3 taragetPosition, string targetTag, int layerMask, float rayDirection = RayDirection)
    {
        RaycastHit hit;
        Vector3 normal = (taragetPosition - startPosition).normalized;
        //Debug.DrawRay(startPosition, (taragetPosition - startPosition), Color.red, 1f);

        if (Physics.Raycast(startPosition, normal, out hit, rayDirection, layerMask))
        {
            //Debug.Log(hit.transform.gameObject);
            if (Utility.Instance.IsTagNameMatch(hit.transform.gameObject, targetTag))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// カメラからRayを飛ばして当たった対象のタグが指定したタグと一致しているか
    /// </summary>
    /// <param name="targetTag"></param>
    /// <param name="rayDirection"></param>
    /// <returns></returns>
    public bool IsRaycastHitObjectMatchFromScreen(string targetTag, float rayDirection = RayDirection)
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, rayDirection))
        {
            return Utility.Instance.IsTagNameMatch(hit.transform.gameObject, targetTag);
        }
        return false;
    }
}
