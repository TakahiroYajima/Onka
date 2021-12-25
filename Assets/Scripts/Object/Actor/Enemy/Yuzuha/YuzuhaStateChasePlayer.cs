using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YuzuhaStateChasePlayer : StateBase
{
    private Enemy_Yuzuha yuzuha = null;
    private bool isHitPlayer = false;//最初からプレイヤーに衝突している場合、OnColliderEnterが反応しないので、OnColliderStayを1度だけ発生させるようにするフラグ

    public override void StartAction()
    {
        yuzuha = StageManager.Instance.Yuzuha;
        yuzuha.navMeshAgent.enabled = true;
        yuzuha.navMeshAgent.speed = yuzuha.runSpeed;
        yuzuha.walkAnimObj.enabled = true;
        yuzuha.walkAnimObj.isLookTarget = true;
        yuzuha.walkAnimObj.isAutoRotation = false;
        yuzuha.walkAnimObj.SetAnimSpeed(1f);
        yuzuha.walkAnimObj.AnimOn();
        //yuzuha.onColliderEnterCallback = OnColliderEnterEvent;
        yuzuha.onCollsionEnterCallback = OnCollisionEnterEvent;
        StageManager.Instance.Player.AddChasedCount(yuzuha);
    }

    public override void UpdateAction()
    {
        yuzuha.walkAnimObj.lookTargetPosition = StageManager.Instance.Player.eyePosition;
        yuzuha.navMeshAgent.SetDestination(StageManager.Instance.Player.transform.position);
    }

    public override void EndAction()
    {
        StageManager.Instance.Player.RemoveChasedCount(yuzuha);
        yuzuha.walkAnimObj.enabled = false;
    }

    //public void OnColliderEnterEvent(Collider collider)
    //{
    //    if (isHitPlayer) return;

    //    switch (collider.transform.tag)
    //    {
    //        case Tags.Player:
    //            isHitPlayer = true;
    //            yuzuha.ChangeState(EnemyState.CanNotAction);
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
                yuzuha.ChangeState(EnemyState.CanNotAction);
                GameOverManager.Instance.StartGameOver(GameOverType.AzuYuzuArrested);
                break;
        }
    }
}
