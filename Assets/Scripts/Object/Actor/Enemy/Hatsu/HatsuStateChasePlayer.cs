using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatsuStateChasePlayer : StateBase
{
    private Enemy_Hatsu hatsu = null;

    public HatsuStateChasePlayer(Enemy_Hatsu _hatsu)
    {
        hatsu = _hatsu;
    }

    public override void StartAction()
    {
        hatsu.navMeshAgent.enabled = true;
        hatsu.navMeshAgent.speed = hatsu.walkSpeed / 2f;
        hatsu.walkAnimObj.enabled = true;
        hatsu.walkAnimObj.isLookTarget = false;
        hatsu.walkAnimObj.isAutoRotation = false;
        hatsu.walkAnimObj.SetAnimSpeed(hatsu.walkSpeed);
        hatsu.walkAnimObj.AnimOn();
        hatsu.soundPlayerObject.PlaySoundLoop(0);
        hatsu.onColliderEnterEvent = OnColliderEnterEvent;
        StageManager.Instance.Player.AddChasedCount(hatsu);
    }

    public override void UpdateAction()
    {
        hatsu.navMeshAgent.SetDestination(StageManager.Instance.Player.Position);
    }

    public override void EndAction()
    {
        StageManager.Instance.Player.RemoveChasedCount(hatsu);
        hatsu.walkAnimObj.enabled = false;
    }

    private void OnColliderEnterEvent(Collider c)
    {
        if (Utility.Instance.IsTagNameMatch(c.gameObject, Tags.Player))
        {
            hatsu.soundPlayerObject.StopSound();
            GameOverManager.Instance.StartGameOver(GameOverType.HatsuArrested);
        }
    }
}
