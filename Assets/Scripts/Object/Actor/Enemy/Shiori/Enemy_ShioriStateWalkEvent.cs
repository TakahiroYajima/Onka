using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;

public class Enemy_ShioriStateWalkEvent : StateBase
{
    private float moveSpeed = 0.3f;
    private float endTime = 10f;
    private float currentTime = 0f;

    public override void StartAction()
    {
        currentTime = 0f;
        StageManager.Instance.Player.AddChasedCount(StageManager.Instance.Shiori);
        SoundManager.Instance.PlayEnvironmentWithKey("ambient_shiori", false);
        StageManager.Instance.Shiori.SoundPlayerObject.PlaySoundLoop(0);
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
        StageManager.Instance.Player.RemoveChasedCount(StageManager.Instance.Shiori);
        if (StageManager.Instance.Shiori.onWalkEventEnded != null)
        {
            StageManager.Instance.Shiori.onWalkEventEnded();
        }
        
        SoundManager.Instance.PlayEnvironmentWithKey("ambient_in_house", false);
        //StageManager.Instance.Shiori.SoundPlayerObject.StopSound();
    }
}
