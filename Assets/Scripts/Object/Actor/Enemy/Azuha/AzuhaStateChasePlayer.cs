using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AzuhaStateChasePlayer : StateBase
{
    private Enemy_Azuha azuha = null;
    private bool isHitPlayer = false;//最初からプレイヤーに衝突している場合、OnColliderEnterが反応しないので、OnColliderStayを1度だけ発生させるようにするフラグ

    public override void StartAction()
    {
        azuha = StageManager.Instance.Azuha;
        azuha.navMeshAgent.enabled = true;
        azuha.navMeshAgent.speed = azuha.runSpeed;
        azuha.walkAnimObj.enabled = true;
        azuha.walkAnimObj.isLookTarget = true;
        azuha.walkAnimObj.isAutoRotation = false;
        azuha.walkAnimObj.SetAnimSpeed(1f);
        azuha.walkAnimObj.AnimOn();
        //azuha.onColliderEnterCallback = OnColliderEnterEvent;
        azuha.onCollsionEnterCallback = OnCollisionEnterEvent;
        azuha.soundPlayerObject.PlaySE(0);
        StageManager.Instance.Player.AddChasedCount(azuha);
    }

    public override void UpdateAction()
    {
        azuha.walkAnimObj.lookTargetPosition = StageManager.Instance.Player.eyePosition;
        azuha.navMeshAgent.SetDestination(StageManager.Instance.Player.transform.position);
    }

    public override void EndAction()
    {
        azuha.soundPlayerObject.StopSound();
        StageManager.Instance.Player.RemoveChasedCount(azuha);
        azuha.walkAnimObj.enabled = false;
    }

    //public void OnColliderEnterEvent(Collider collider)
    //{
    //    if (isHitPlayer) return;

    //    switch (collider.transform.tag)
    //    {
    //        case Tags.Player:
    //            isHitPlayer = true;
    //            azuha.ChangeState(EnemyState.CanNotAction);
    //            GameOverManager.Instance.StartGameOver(GameOverType.AzuYuzuArrested);
    //            break;
    //    }
    //}
    public void OnCollisionEnterEvent(Collision collision)
    {
        if (isHitPlayer) return;

        switch (collision.transform.tag)
        {
            case Tags.Player:
                isHitPlayer = true;
                azuha.ChangeState(EnemyState.CanNotAction);
                GameOverManager.Instance.StartGameOver(GameOverType.AzuYuzuArrested);
                break;
        }
    }
}
