using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoundDistance
{
    /// <summary>
    /// 音が聞こえる方向の情報を判定するための各地点
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class SoundDistancePoint : MonoBehaviour
    {
        [field: SerializeField, ReadOnly] public string pointKey { get; private set; }
        [SerializeField] private bool isOuter = false;//外周に位置しているか
        public bool IsOuter { get { return isOuter; } }
        [SerializeField] private bool isCorner = false;//曲がり角に配置されているか
        public bool IsCorner { get { return isCorner; } }

        private BoxCollider boxCollider = null;
        //音が聞こえる部分の中心点（transform.positionでは曲がり角の時に中央から聞こえてしまうので、インコース寄りにさせる。直線はそのままtransform.positionになる）
        public Vector3 soundAncherPosition { get; private set; } = Vector3.zero;
        /// <summary>
        /// 当たり判定の端にEmitterDistanceの位置を置きたいため、当たり判定のサイズが大きい方を返す（中心からの距離を測るので半分のサイズにする）
        /// </summary>
        private float AddEmitterPointDistance { get { return boxCollider.size.x > boxCollider.size.z ? boxCollider.size.x : boxCollider.size.z; } }
        public Vector3 GetDirectionPoint(Direction direction)
        {
            switch (direction)
            {
                case Direction.Foward:  return transform.position + new Vector3(0, 0, boxCollider.size.z * 0.5f);
                case Direction.Back:    return transform.position - new Vector3(0, 0, boxCollider.size.z * 0.5f);
                case Direction.Right:   return transform.position + new Vector3(boxCollider.size.x * 0.5f, 0, 0);
                case Direction.Left:    return transform.position - new Vector3(boxCollider.size.x * 0.5f, 0, 0);
            }
            return Vector3.zero;
        }
        public Vector3 GetDirectionPointAddSizeOnly(Direction direction)
        {
            switch (direction)
            {
                case Direction.Foward: return new Vector3(0, 0, boxCollider.size.z * 0.5f);
                case Direction.Back: return new Vector3(0, 0, -boxCollider.size.z * 0.5f);
                case Direction.Right: return new Vector3(boxCollider.size.x * 0.5f, 0, 0);
                case Direction.Left: return new Vector3(-boxCollider.size.x * 0.5f, 0, 0);
            }
            return Vector3.zero;
        }

        //前後左右で作る（RPGマップの経路探索みたいに）
        [SerializeField] private SoundDistancePointNodeTable soundDistancePointNodes = null;
        public SoundDistancePointNodeTable PointNodes { get { return soundDistancePointNodes; } }
        /// <summary>
        /// 渡された方向にある隣のノードを返す
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public SoundDistancePointNode GetPointNode(Direction direction)
        {
            //無ければ生成（IsExist参照ができない事を防ぐ）
            if (!soundDistancePointNodes.GetTable().ContainsKey(direction))
            {
                soundDistancePointNodes.GetTable().Add(direction, new SoundDistancePointNode());
            }
            return soundDistancePointNodes.GetTable()[direction];
        }
        public SoundDistancePointNode GetPointNodeWithID(int _id)
        {
            return soundDistancePointNodes.GetTable().Where(t => t.Value.IsExist).FirstOrDefault(x => x.Value.node.ID == _id).Value;
        }

        public Vector3 GetDirectionNormalize(Direction direction)
        {
            if (!soundDistancePointNodes.GetTable().ContainsKey(direction)) return Vector3.zero;
            if (!soundDistancePointNodes.GetTable()[direction].IsExist) return Vector3.zero;
            return (transform.position - soundDistancePointNodes.GetTable()[direction].node.transform.position).normalized;
        }

        //現在、音が聞こえる方向にある隣のノード（前後左右の情報のどれかが入る）
        public SoundDistancePoint currentEmitterDirectionPoint { get; private set; } = null;
        public void SetEnitterDirectionPoint(SoundDistancePoint _point)
        {
            currentEmitterDirectionPoint = _point;
        }
        //currentEmitterDirectionPointの方向に単位ベクトルを追加した自身のpositionを返す
        public Vector3 EmitterDirectionPoint { get { return (soundAncherPosition + (currentEmitterDirectionPoint.soundAncherPosition - soundAncherPosition).normalized * AddEmitterPointDistance * 1.1f); } }

        //上記の複数版（ゴール地点では両方の方向からprev設定されるので保持する
        public List<SoundDistancePoint> prevPointList { get; private set; } = new List<SoundDistancePoint>();
        public void AddPrev(SoundDistancePoint _prev)
        {
            prevPointList.Add(_prev);
        }
        public void ClearPrev()
        {
            prevPointList.Clear();
        }

        /// <summary>
        /// インスタンスID
        /// </summary>
        public int ID { get; private set; } = -1;

        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        /// <summary>
        /// ノード情報の初期設定
        /// </summary>
        public void Initialize()
        {
            Dictionary<Direction, bool> routeExistsDic = new Dictionary<Direction, bool>();
            foreach (var n in soundDistancePointNodes.GetTable())
            {
                if (n.Value.IsExist)
                {
                    n.Value.distanceMagnitude = (transform.position - n.Value.node.transform.position).sqrMagnitude;
                }
                routeExistsDic.Add(n.Key, n.Value.IsExist);
            }
            //曲がり角の場合、パターンに応じてsoundAncherPositionをtransform.positionからColliderの範囲 / 4分ずらす
            soundAncherPosition = transform.position;
            if (isCorner)
            {
                float shiftX = boxCollider.size.x * 0.25f;
                float shiftZ = boxCollider.size.z * 0.25f;
                if (routeExistsDic[Direction.Foward])
                {
                    //L型の曲がり角
                    if (routeExistsDic[Direction.Right])
                    {
                        soundAncherPosition = transform.position + new Vector3(shiftX, 0, shiftZ);
                    }
                    //逆L型の曲がり角
                    if (routeExistsDic[Direction.Left])
                    {
                        soundAncherPosition = transform.position + new Vector3(-shiftX, 0, shiftZ);
                    }
                }else if (routeExistsDic[Direction.Back])
                {
                    //上下逆L型の曲がり角
                    if (routeExistsDic[Direction.Right])
                    {
                        soundAncherPosition = transform.position + new Vector3(shiftX, 0, -shiftZ);
                    }
                    //上下逆の逆L型の曲がり角
                    if (routeExistsDic[Direction.Left])
                    {
                        soundAncherPosition = transform.position + new Vector3(-shiftX, 0, -shiftZ);
                    }
                }
            }
        }
        //void OnDrawGizmos()
        //{
        //    Gizmos.DrawCube(soundAncherPosition, new Vector3(0.3f, 0.3f, 0.3f));
        //}
        /// <summary>
        /// インスタンスIDを設定
        /// </summary>
        /// <param name="_id">子要素のID = 配列の要素番号</param>
        public void SetID(int _id)
        {
            ID = _id;
        }

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log("Hit id " + ID + " : " + other.name);
            switch (other.tag)
            {
                case "Player":
                    SoundDistanceManager.Instance.SetPointToListener(this);
                    break;
                case "Enemy":
                    SoundDistanceManager.Instance.SetPointToEmitter(this);
                    break;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            //Debug.Log("Exit id " + ID + " : " + other.name);
            SoundDistancePointNode nextNode;
            int nextID = -1;
            switch (other.tag)
            {
                case "Player":
                    nextNode = GetPointNode(GetMoveObjectExitDirection(SoundDistanceManager.Instance.Listener.transform.position));
                    NextNodeIDSetting(nextNode, out nextID);
                    SoundDistanceManager.Instance.ExitPointToListener(ID, nextID);
                    break;
                case "Enemy":
                    nextNode = GetPointNode(GetMoveObjectExitDirection(SoundDistanceManager.Instance.Emitter.transform.position));
                    NextNodeIDSetting(nextNode, out nextID);
                    SoundDistanceManager.Instance.ExitPointToEmitter(ID,nextID);
                    break;
            }
        }
        /// <summary>
        /// Listenerが出ていった方向を返す
        /// </summary>
        /// <returns></returns>
        private Direction GetMoveObjectExitDirection(Vector3 moveObjPosition)
        {
            Vector3 dir = (moveObjPosition - transform.position).normalized;
            float ax = Mathf.Abs(dir.x);
            float az = Mathf.Abs(dir.z);
            bool isVertical = az > ax;
            if (dir.z > 0   &&  isVertical) return Direction.Foward;
            if (dir.z <= 0  &&  isVertical) return Direction.Back;
            if (dir.x > 0   && !isVertical) return Direction.Right;
            if (dir.x <= 0  && !isVertical) return Direction.Left;

            Debug.LogError("どれにもあてはまりません");
            return Direction.Foward;//とりあえずFowardを返す
        }

        private void NextNodeIDSetting(SoundDistancePointNode nextNode, out int nextID)
        {
            if (nextNode.IsExist)
            {
                nextID = nextNode.node.ID;
            }
            else
            {
                nextID = ID;//ノードが設定されていないところに出た場合は自分のIDを設定
            }
        }
    }

    [System.Serializable]
    public class SoundDistancePointNode
    {
        public SoundDistancePoint node = null;
        [HideInInspector] public float distanceMagnitude = 0f;

        public bool IsExist { get { return node != null; } }
        public bool IsOuter { get { if (IsExist) { return node.IsOuter; } else { return false; } } }
    }
}

public enum Direction
{
    Foward,
    Back,
    Left,
    Right,
}

/// <summary>
/// ジェネリックを隠すために継承してしまう
/// [System.Serializable]を書くのを忘れない
/// </summary>
[System.Serializable]
public class SoundDistancePointNodeTable : Serialize.TableBase<Direction, SoundDistance.SoundDistancePointNode, SoundDistancePointNodePair>
{


}

/// <summary>
/// ジェネリックを隠すために継承してしまう
/// [System.Serializable]を書くのを忘れない
/// </summary>
[System.Serializable]
public class SoundDistancePointNodePair : Serialize.KeyAndValue<Direction, SoundDistance.SoundDistancePointNode>
{

    public SoundDistancePointNodePair(Direction key, SoundDistance.SoundDistancePointNode value) : base(key, value)
    {

    }
}