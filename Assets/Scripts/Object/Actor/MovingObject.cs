using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 動作関連で共通するものを定義
/// </summary>
public class MovingObject : MonoBehaviour
{
    private float rotationSpeed = 3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// 指定された方向へ向く（Y軸は自分と同じになる）
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="onRotationEnded"></param>
    public void TurnAroundToTargetAngle_Update(Vector3 targetPosition, UnityAction onRotationEnded)
    {
        Vector3 playerPos = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        Vector3 targetDir = playerPos - transform.position;
        Vector3 normalized = targetDir.normalized;
        float dot = Vector3.Dot(transform.forward, normalized);

        float deg = Mathf.Acos(dot) * Mathf.Rad2Deg;
        if (deg <= rotationSpeed)
        {
            transform.LookAt(playerPos);
            onRotationEnded();
        }
        else
        {
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotationSpeed * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    /// <summary>
    /// 指定された方向へ向く（Y軸は自分と同じになる）
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="rotationTime"></param>
    /// <param name="onRotationEnded"></param>
    public IEnumerator TurnAroundToTargetAngle_Coroutine(Vector3 targetPosition, float rotationSpeed, UnityAction onRotationEnded = null)
    {
        Vector3 targetPos = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        Vector3 targetDir = targetPos - transform.position;
        Vector3 normalized = targetDir.normalized;
        float dot = Vector3.Dot(transform.forward, normalized);
        //float degFirst = Mathf.Acos(dot) * Mathf.Rad2Deg;
        float deg = rotationSpeed * 2f;//初期値はrotationSpeedより大きく
        while (deg > rotationSpeed)
        {
            //targetDir = targetPos - transform.position;
            //normalized = targetDir.normalized;
            dot = Vector3.Dot(transform.forward, normalized);
            deg = Mathf.Acos(dot) * Mathf.Rad2Deg;

            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotationSpeed * Time.deltaTime, 0f); //degFirst * (Time.deltaTime / rotationTime), 0f);
            transform.rotation = Quaternion.LookRotation(newDir);
            yield return null;
        }
        transform.LookAt(targetPos);
        if (onRotationEnded != null)
        {
            onRotationEnded();
        }
    }

    /// <summary>
    /// ターゲットの方へ向きながら進む
    /// </summary>
    public void MoveToTargetDir_Update(Vector3 moveDir, float moveSpeed)
    {
        moveDir.y = 0f;
        transform.rotation = Quaternion.LookRotation(moveDir);
        transform.position += moveDir.normalized * Time.deltaTime * moveSpeed;
    }

    public IEnumerator MoveWithTime(Vector3 targetPosition, float moveTime, UnityAction onComplete = null)
    {
        Vector3 direction = targetPosition - transform.position;
        float currentTime = 0f;
        while(currentTime < moveTime)
        {
            transform.position += direction * Time.deltaTime / moveTime;
            currentTime += Time.deltaTime;
            yield return null;
        }
        if(onComplete != null)
        {
            onComplete();
        }
    }

    public bool TurnAroundSmooth_Update(Vector3 targetPosition, float rotateSpeed)
    {
        bool isArrival = false;
        Vector3 targetDir = targetPosition - transform.position;
        Vector3 normalized = targetDir.normalized;
        float dot = dot = Vector3.Dot(transform.forward, normalized);
        float deg = deg = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (deg < 0.1f)//0.1は感覚値
        {
            transform.LookAt(targetPosition);
            isArrival = true;
        }
        else
        {
            Quaternion quaternion = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, Time.deltaTime * rotateSpeed);
        }
        return isArrival;
    }
    /// <summary>
    /// 徐々にターゲットの方を向かせる
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="rotateSpeed"></param>
    /// <returns></returns>
    public IEnumerator TurnAroundSmooth_Coroutine(Vector3 targetPosition, float rotateSpeed)
    {
        Vector3 targetDir = targetPosition - transform.position;
        Vector3 normalized = targetDir.normalized;
        float dot = Vector3.Dot(transform.forward, normalized);
        float deg = 1000f;//初期値はありえない値

        Quaternion quaternion;

        bool isMove = true;
        while (isMove)
        {
            dot = Vector3.Dot(transform.forward, normalized);
            deg = Mathf.Acos(dot) * Mathf.Rad2Deg;
            if(deg < 0.1f)//0.1は感覚値
            {
                transform.LookAt(targetPosition);
                isMove = false;
            }
            else
            {
                quaternion = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, Time.deltaTime * rotateSpeed);
            }
            yield return null;
        }
    }
}
