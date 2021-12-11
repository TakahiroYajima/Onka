using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolveKeylockManager : SingletonMonoBehaviour<SolveKeylockManager>
{
    private KeyLockObject currentSolvingKeyLockObject = null;
    [SerializeField] private GameObject canvasObj = null;

    /// <summary>
    /// 解錠イベント開始
    /// </summary>
    public void StartSolveEvent(KeyLockObject _keyLockObject)
    {
        currentSolvingKeyLockObject = _keyLockObject;
        StageManager.Instance.Player.ChangeState(PlayerState.SolveKeylock);
        canvasObj.SetActive(true);
        //currentSolvingKeyLockObjectをカメラの前まで移動させる
        Vector3 pos = StageManager.Instance.Player.CameraObj.transform.position + StageManager.Instance.Player.CameraObj.transform.forward * currentSolvingKeyLockObject.distanceFromCamera;
        //カメラの向きの先に置く(transform.fowardを取れば楽？)
        currentSolvingKeyLockObject.transform.position = pos;
        //currentSolvingKeyLockObjectをカメラの方に向かせる(transform.lookat?)
        currentSolvingKeyLockObject.transform.LookAt(currentSolvingKeyLockObject.transform.position + StageManager.Instance.Player.CameraObj.transform.forward);
        //鍵が揺れる的な効果音
    }
    /// <summary>
    /// 解かずに終了する場合
    /// </summary>
    public void EndSolveEvent()
    {
        ForceFinish();
        //プレイヤーのステートを戻す
        StageManager.Instance.Player.ChangeState(PlayerState.Free);
        canvasObj.SetActive(false);
    }

    /// <summary>
    /// 解錠できた際のイベント
    /// </summary>
    public void StartUnlockEvent()
    {
        //敵すべてのステートをCanNotActionにする（イベント中にプレイヤーを発見できなくさせる）
        StageManager.Instance.AllEnemyForceChangeStateCanNotAction();
        canvasObj.SetActive(false);
    }
    /// <summary>
    /// 解錠イベント終了
    /// </summary>
    public void FinishUnlockEvent()
    {
        //currentSolvingKeyLockObjectを元の位置に戻す（カメラの前にあるので）
        currentSolvingKeyLockObject.RemoveInitPos();
        currentSolvingKeyLockObject.DoEnactive();

        currentSolvingKeyLockObject = null;
        
        //プレイヤーと敵のステートを戻す
        StageManager.Instance.Player.ChangeState(PlayerState.Free);
        StageManager.Instance.AllEnemyRestoreState();
    }

    /// <summary>
    /// 解錠中に敵に見つかった時、強制終了させる
    /// </summary>
    public void ForceFinish()
    {
        //currentSolvingKeyLockObjectを元の位置に戻す
        currentSolvingKeyLockObject.RemoveInitState();
        currentSolvingKeyLockObject = null;
        canvasObj.SetActive(false);
    }
}
