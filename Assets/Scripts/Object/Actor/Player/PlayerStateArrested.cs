using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateArrested : StateBase
{
    private PlayerObject player = null;
    public PlayerStateArrested(PlayerObject _player)
    {
        player = _player;
    }
    public override void StartAction()
    {
        player.ForcedStopFPS();
        player.capsuleCollider.enabled = false;
        player.rigidbody.velocity = Vector3.zero;
        player.rigidbody.angularVelocity = Vector3.zero;
        player.rigidbody.isKinematic = true;
        CrosshairManager.Instance.SetCrosshairActive(false);
    }

    public override void UpdateAction()
    {

    }

    public override void EndAction()
    {
        player.StartActiveFPS();
    }
}
