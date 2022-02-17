using UnityEngine;

public class Tags
{
    public const string Player = "Player";
    public const string Enemy = "Enemy";
    public const string Door = "Door";
    public const string Item = "Item";
    public const string KeyHole = "KeyHole";
    public const string KeyLock = "KeyLock";
    public const string KeyLockTouchable = "KeyLockTouchable";
    public const string StageObject = "StageObject";
    public const string SaveObject = "SaveObject";
}

public static class LayerMaskData
{
    public static int SerchToPlayerMask = 0;//RayCastでプレイヤーを探す時のLayerMask（EnemyとIgnoreにHit判定を作りたくない）
    public static int FromPlayerRayMask = 0;//プレイヤーから飛ばされるRay。PlayerとIgnoreを無視する

    /// <summary>
    /// このゲーム限定の初期化を書かなければならない。めんどくせえ！！
    /// </summary>
    public static void Initialize()
    {
        //Debug.Log("Init");
        //~を付けるので無視するレイヤーを列挙。これ以外のレイヤーと当たるようにする
        SerchToPlayerMask = ~(
            LayerMask.NameToLayer(Tags.Enemy) |
            LayerMask.NameToLayer("Ignore Raycast") |
            LayerMask.NameToLayer("WanderingSystem") |
            LayerMask.NameToLayer("RoomCollider") |
            LayerMask.NameToLayer("ToPlayerOnlyCollision"));

        FromPlayerRayMask = ~(
            LayerMask.NameToLayer(Tags.Player) |
            LayerMask.NameToLayer("Ignore Raycast") |
            LayerMask.NameToLayer("WanderingSystem") |
            LayerMask.NameToLayer("RoomCollider")
            );
    }
}