using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;

public class KozoStateChasePlayer : StateBase
{
    private Enemy_Kozo kozo = null;

    public KozoStateChasePlayer(Enemy_Kozo _kozo)
    {
        kozo = _kozo;
    }

    public override void StartAction()
    {
        kozo.navMeshAgent.enabled = true;
        kozo.navMeshAgent.speed = kozo.walkSpeed / 2f;
        kozo.walkAnimObj.enabled = true;
        kozo.walkAnimObj.isLookTarget = false;
        kozo.walkAnimObj.isAutoRotation = false;
        kozo.walkAnimObj.SetAnimSpeed(kozo.walkSpeed);
        kozo.walkAnimObj.AnimOn();
        kozo.soundPlayerObject.PlaySoundLoop(0);
        kozo.onColliderEnterEvent = OnColliderEnterEvent;
        StageManager.Instance.Player.AddChasedCount(kozo);
    }

    public override void UpdateAction()
    {
        kozo.navMeshAgent.SetDestination(StageManager.Instance.Player.transform.position);
    }

    public override void EndAction()
    {
        StageManager.Instance.Player.RemoveChasedCount(kozo);
        kozo.walkAnimObj.enabled = false;
    }

    private void OnColliderEnterEvent(Collider c)
    {
        if (Utility.Instance.IsTagNameMatch(c.gameObject, Tags.Player))
        {
            kozo.soundPlayerObject.StopSound();
            GameOverManager.Instance.StartGameOver(GameOverType.KozoArrested);
        }
    }
}
