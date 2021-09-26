using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_ShioriStateWalkEvent : StateBase
{
    private float moveSpeed = 0.6f;
    private float endTime = 6f;
    private float currentTime = 0f;

    public override void StartAction()
    {
        currentTime = 0f;
    }
    public override void UpdateAction()
    {
        Vector3 moveDir = StageManager.Instance.Player.transform.position - StageManager.Instance.Shiori.transform.position;
        moveDir.y = 0f;
        StageManager.Instance.Shiori.transform.rotation = Quaternion.LookRotation(moveDir);
        StageManager.Instance.Shiori.transform.position += moveDir.normalized * Time.deltaTime * moveSpeed;
        currentTime += Time.deltaTime;
        if(currentTime >= endTime)
        {
            StageManager.Instance.Shiori.ChangeState(Enemy_ShioriState.Init);
        }
    }
    public override void EndAction()
    {
        if(StageManager.Instance.Shiori.onWalkEventEnded != null)
        {
            StageManager.Instance.Shiori.onWalkEventEnded();
        }
    }
}
