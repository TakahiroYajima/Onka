using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの方へ振り返るステート。追いかけるステートへのつなぎ
/// </summary>
public class YukieStateTurnAroundToPlayer : StateBase
{
    private Enemy_Yukie yukie = null;
    private PlayerObject player = null;

    private float rotationSpeed = 3f;

    public override void StartAction()
    {
        yukie = StageManager.Instance.Yukie;
        player = StageManager.Instance.Player;
        yukie.wanderingActor.SetActive(false);
    }

    public override void UpdateAction()
    {
        Vector3 playerPos = new Vector3(player.transform.position.x, yukie.transform.position.y, player.transform.position.z);
        Vector3 targetDir = playerPos - yukie.transform.position;
        Vector3 normalized = targetDir.normalized;
        float dot = Vector3.Dot(yukie.transform.forward, normalized);

        float deg = Mathf.Acos(dot) * Mathf.Rad2Deg;
        if (deg <= rotationSpeed) {
            yukie.transform.LookAt(playerPos);
            yukie.ChangeState(EnemyState.RecognizedPlayer);
        }
        else
        {
            Vector3 newDir = Vector3.RotateTowards(yukie.transform.forward, targetDir, rotationSpeed * Time.deltaTime, 0f);
            yukie.transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    public override void EndAction()
    {
        yukie.wanderingActor.SetActive(true);
    }
}
