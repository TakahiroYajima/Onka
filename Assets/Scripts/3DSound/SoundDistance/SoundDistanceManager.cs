using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoundDistance
{
    /// <summary>
    /// 建物内で曲がり角から曲がった先で発する音が聞こえるようにする機能の管理
    /// </summary>
    public class SoundDistanceManager : SingletonMonoBehaviour<SoundDistanceManager>
    {
        [HideInInspector] public bool isActive = false;
        [SerializeField] private SoundDistanceListener listenerObj = null;//音を聞く側
        public SoundDistanceListener Listener { get { return listenerObj; } }
        [SerializeField] private SoundDistanceEmitter emitterObj = null;//音を発する側
        public SoundDistanceEmitter Emitter { get { return emitterObj; } }
        [SerializeField] private SoundDistanceMakerObj soundMaker = null;//listenerObjから音の方向に位置取り、サウンドを鳴らすオブジェクト
        public SoundDistanceMakerObj Maker { get { return soundMaker; } }

        [SerializeField, Range(0, 1f)] private float canNotHearRatio = 0.6f;//音が聞こえなくなる距離の割合（外周に対してどれだけEmitterとListenerが離れていれば音が聞こえなくなるかの割合）
        public float CanNotHearRatio { get { return canNotHearRatio; } }

        public List<SoundDistancePoint> soundDistancePoints { get; private set; } = new List<SoundDistancePoint>();
        public float OuterCircumference { get; private set; } = 0f;//1周の距離（袋小路は考慮しない）

        //ダイクストラで計算された最短距離（goalからstartまで）
        private List<List<SoundDistancePoint>> serchEachRouteList = new List<List<SoundDistancePoint>>();//全ルート
        public List<SoundDistancePoint> calcMinDistancePoints { get; private set; } = new List<SoundDistancePoint>();//最短ルート
        private List<float> costList = new List<float>();
        private float subCost = 0f;
        private int currentMinID = -1;
        //現在のListenerからEmitterまでの距離（ダイクストラで計算された最短距離）
        public float currentDistanceListenerToEmitter { get; private set; } = 0f;

        #region フレームカウント
        //フレームカウント
        private const int ActionFrameNum = 60;//ActionFrameNumフレームに1回処理させる
        private int currentFrame = 0;
        private bool isActionFrame { get { return currentFrame >= ActionFrameNum; } }
        #endregion

        public void Initialize()
        {
            soundDistancePoints.Clear();
            soundDistancePoints = transform.GetComponentsInChildren<SoundDistancePoint>().ToList();
            
            //全て初期化してからルート探索のデータを設定（初期化前のPointを参照している恐れがあるため）
            for (int i = 0; i < soundDistancePoints.Count; i++)
            {
                soundDistancePoints[i].SetID(i);
                soundDistancePoints[i].Initialize();
            }
            for (int i = 0; i < soundDistancePoints.Count; i++)
            {
                //全ての方向のデータ分を回す
                foreach (Direction d in Enum.GetValues(typeof(Direction)))
                {
                    SoundDistancePointNode node = soundDistancePoints[i].GetPointNode(d);
                    //外周のものなら距離をカウント
                    if (soundDistancePoints[i].IsOuter && node.IsOuter)
                    {
                        OuterCircumference += node.distanceMagnitude;
                    }
                }
            }
            OuterCircumference *= 0.5f;//双方追加している関係で /2 する
            //Debug.Log("初期化　外周の距離：" + OuterCircumference);
        }
        
        // Update is called once per frame
        void Update()
        {
            if (!IsAllInitSeted() || !isActive) return;
            if (DoUpdateFrameAndJudge())
            {
                CalcRouteSeachUpdate();
            }
            if(currentMinID > -1)
            {
                SetCurrentDistanceListenerToEmitter(costList, currentMinID);
            }

        }
        
        private bool IsAllInitSeted()
        {
            return listenerObj.currentPointID >= 0 && emitterObj.currentPointID >= 0;
        }

        private bool DoUpdateFrameAndJudge()
        {
            if (!isActionFrame) { currentFrame++; return false; }
            ResetFrameCount();
            return true;
        }
        private void ResetFrameCount()
        {
            currentFrame = 0;
        }

        #region Optional Setter
        /// <summary>
        /// 機能をオフにする
        /// </summary>
        /// <param name="listenerID"></param>
        /// <param name="emitterID"></param>
        public void SetInactive(int listenerID = 0, int emitterID = 0)
        {
            isActive = false;
            Maker.StopAction();Maker.SetVolume(0f);
            Maker.SetVolume(0f);
            Listener.SetCurrentPointID(listenerID);
            Emitter.SetPointID(emitterID);
        }
        public void SetEachIDs(int listenerID, int emitterID)
        {
            Listener.SetCurrentPointID(listenerID);
            Emitter.SetPointID(emitterID);
        }
        #endregion

        #region RouteSerch
        /// <summary>
        /// ゴールから全ての方向のルートを辿り、スタートまでの道のりをserchEachRouteListに設定する
        /// </summary>
        /// <param name="emitterCurrentOuterID"></param>
        /// <param name="listenerCurrentOuterID"></param>
        private void SetAllRouteToDikstraEachOuterRouteList(int emitterCurrentOuterID, int listenerCurrentOuterID)
        {
            ClearAllPointPrevData();//Prev情報を初期化
            this.serchEachRouteList.Clear();
            this.costList.Clear();

            SoundDistancePoint goal = soundDistancePoints.FirstOrDefault(x => x.ID == emitterCurrentOuterID);
            int routeArrayNum = 0;
            //emitterCurrentIDとlistenerCurrentIDが一致していたら同じ場所（or同じ部屋の入口）にいるのでreturn
            if (emitterCurrentOuterID == listenerCurrentOuterID) {
                //goalだけは追加する
                this.serchEachRouteList.Add(new List<SoundDistancePoint>());
                this.serchEachRouteList[0].Add(goal);
                return;
            }

            foreach (Direction fromGoalDirection in Enum.GetValues(typeof(Direction)))
            {
                SoundDistancePointNode n = goal.GetPointNode(fromGoalDirection);
                if(!n.IsExist || !n.IsOuter) { continue; }

                this.serchEachRouteList.Add(new List<SoundDistancePoint>());
                this.serchEachRouteList[routeArrayNum].Add(goal);
                n.node.AddPrev(goal);
                this.costList.Add(0f);
                
                //goalから1方向のルートを順番に探索するため、最初にgoalの隣のノードを追加しておく
                this.serchEachRouteList[routeArrayNum].Add(n.node);
                this.costList[routeArrayNum] += n.distanceMagnitude;

                for (int i = 1; true; i++)
                {
                    SoundDistancePoint currentSeachPoint = this.serchEachRouteList[routeArrayNum][i];
                    //スタート地点に来ていたら終了
                    if (currentSeachPoint.ID == listenerCurrentOuterID) { break; }

                    //全ての方向のデータ分を回す
                    foreach (Direction d in Enum.GetValues(typeof(Direction)))
                    {
                        SoundDistancePointNode node = currentSeachPoint.GetPointNode(d);
                        //外周のものをルートに追加
                        if (node.IsExist && node.IsOuter)
                        {
                            //逆走できないようにする(Prevに設定されていない場所だけ追加できる)
                            bool isAddable = currentSeachPoint.prevPointList.FirstOrDefault(x => x.ID == node.node.ID) == null;
                            if (isAddable)
                            {
                                //次のノードのPrevに現在のノードを設定、距離も追加
                                node.node.AddPrev(currentSeachPoint);
                                this.serchEachRouteList[routeArrayNum].Add(node.node);
                                this.costList[routeArrayNum] += node.distanceMagnitude;
                            }
                        }

                    }
                }
                //costにListenerとEmitterの移動済み距離も加味する
                if (serchEachRouteList[routeArrayNum].Count >= 2)
                {
                    float toListenerDistance = (serchEachRouteList[routeArrayNum][serchEachRouteList[routeArrayNum].Count - 2].transform.position - listenerObj.transform.position).sqrMagnitude;
                    //スタートの隣にListenerのnextがある = そこへ向かって歩いている => startからListenerまでの距離をcostから減らす
                    if (serchEachRouteList[routeArrayNum][serchEachRouteList[routeArrayNum].Count - 2].ID == listenerObj.nextTargetPointID)
                    {
                        costList[routeArrayNum] -= toListenerDistance;
                    }
                    else
                    {
                        costList[routeArrayNum] += toListenerDistance;
                    }

                    float toEmitterDistance = (serchEachRouteList[routeArrayNum][1].transform.position - emitterObj.transform.position).sqrMagnitude;
                    //ゴールの隣にEmitterのnextがある = そこへ向かって歩いている => goalからEmitterまでの距離をcostから減らす
                    if (serchEachRouteList[routeArrayNum][1].ID == emitterObj.nextTargetPointID)
                    {
                        costList[routeArrayNum] -= toEmitterDistance;
                    }
                    else
                    {
                        costList[routeArrayNum] += toEmitterDistance;
                    }
                }
                routeArrayNum++;
            }
        }
        /// <summary>
        /// 外周から外れた部分（部屋など）にListenerがいる時、そこから外周へ戻るまでのルートを取得
        /// </summary>
        private List<SoundDistancePoint> CalcSerachSideRoute(int currentListenerOuterID, int currentListenerPointID)
        {
            List<List<SoundDistancePoint>> points = new List<List<SoundDistancePoint>>();
            List<float> subCostList = new List<float>();
            SoundDistancePoint outer = soundDistancePoints.FirstOrDefault(x => x.ID == currentListenerOuterID);
            int routeCount = 0;
            bool isEnd = false;
            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                SoundDistancePointNode n = outer.GetPointNode(d);
                if (!n.IsExist || n.IsOuter) { continue; }//ノードが無い or 外周ならcontinue
                points.Add(new List<SoundDistancePoint>());
                points[routeCount].Add(outer);
                subCostList.Add(0f);
                n.node.AddPrev(outer);

                points[routeCount].Add(n.node);

                for (int i = 1; true; i++)
                {
                    SoundDistancePoint currentSeachPoint = points[routeCount][i];
                    //スタート地点に来ていたら終了
                    if (currentSeachPoint.ID == currentListenerPointID) { isEnd = true; break; }

                    int missingCount = 0;
                    //全ての方向のデータ分を回す
                    foreach (Direction d2 in Enum.GetValues(typeof(Direction)))
                    {
                        SoundDistancePointNode node = currentSeachPoint.GetPointNode(d2);
                        //外周でないのものをルートに追加
                        if (node.IsExist && !node.IsOuter)
                        {
                            //逆走できないようにする(Prevに設定されていない場所だけ追加できる)
                            bool isAddable = currentSeachPoint.prevPointList.FirstOrDefault(x => x.ID == node.node.ID) == null;
                            if (isAddable)
                            {
                                //次のノードのPrevに現在のノードを設定、距離も追加
                                node.node.AddPrev(currentSeachPoint);
                                points[routeCount].Add(node.node);
                                subCostList[routeCount] += node.distanceMagnitude;
                            }
                            else
                            {
                                missingCount++;
                            }
                        }
                        else
                        {
                            missingCount++;
                        }
                    }
                    //全て探索してもListenerがいなかったら次のルートへ
                    if(missingCount >= Enum.GetValues(typeof(Direction)).Length)
                    {
                        break;
                    }
                }
                if (isEnd) { break; }
                routeCount++;
            }
            points[routeCount].RemoveAt(0);//outerの情報は不要なので削除

            subCost = subCostList[routeCount];
            return points[routeCount];//ルートが見つかると強制終了なので、そのままカウントを参照すればそのルートになる
        }
        /// <summary>
        /// 全ルート探索でEmitterまでの経路を計算。Listenerに音を設定させる
        /// </summary>
        private void CalcRouteSeachUpdate()
        {
            SetAllRouteToDikstraEachOuterRouteList(emitterObj.currentOuterPointID, listenerObj.currentOuterPointID);
            //ゴールからスタートまでのルート（複数）の中から最短ルートの配列番号を取得
            currentMinID = GetMinID(costList);
            
            //部屋の中に入るなど、外周から逸れている場合はその分のルートを計算
            if(listenerObj.currentPointID != listenerObj.currentOuterPointID)
            {
                serchEachRouteList[currentMinID].AddRange(CalcSerachSideRoute(listenerObj.currentOuterPointID, listenerObj.currentPointID));
                if(costList.Count == 0)
                {
                    for(int i = 0; i < currentMinID + 1; i++)
                    {
                        costList.Add(0f);
                    }
                }
                costList[currentMinID] += subCost;//逸れている分のコストを追加
                //if (costList.Count > currentMinID)
                //{
                //    costList[currentMinID] += subCost;//逸れている分のコストを追加
                //}
            }
            //if (costList.Count == 0)
            //{
            //    for (int i = 0; i < currentMinID + 1; i++)
            //    {
            //        costList.Add(0f);
            //    }
            //}
            //Debug.Log("現在のコスト : " + costList[currentMinID]);
            //count == 0　：　Pointが同じ
            if (listenerObj.currentPointID == emitterObj.currentPointID)//if (dikstraEachRouteList.Count == 0)
            {
                SetSoundMakerTargetPosition(emitterObj.transform.position);
                calcMinDistancePoints.Clear();
            }
            else
            {
                calcMinDistancePoints = new List<SoundDistancePoint>(serchEachRouteList[currentMinID]);
                SetTargetEachPoint(serchEachRouteList[currentMinID]);
                SetSoundMakerTarget(serchEachRouteList, currentMinID);
            }
        }
        
        /// <summary>
        /// ゴールからスタートまでのルート（複数）の中から最短ルートの配列番号を取得
        /// </summary>
        /// <param name="_costList"></param>
        /// <returns></returns>
        public int GetMinID(IReadOnlyList<float> _costList)
        {
            int minID = 0;
            for (int i = 0; i < _costList.Count; i++)
            {
                if (i > 0)
                {
                    if (_costList[i] < _costList[i - 1]) { minID = i; }
                }
            }
            return minID;
        }
        /// <summary>
        /// ListenerからEmitterまでの距離を設定（Pointからの距離も含めての計算）
        /// </summary>
        /// <param name="costList"></param>
        private void SetCurrentDistanceListenerToEmitter(List<float> costList, int minID)
        {
            //接近していたら直接の距離を設定
            bool isCloseBy = listenerObj.currentPointID == emitterObj.nextTargetPointID && listenerObj.nextTargetPointID == emitterObj.currentPointID;//すれ違うパターン
            bool isCreep = listenerObj.currentPointID != emitterObj.currentPointID;//Emitterの真後ろにListenerがいる場合
            if (isCloseBy || isCreep || calcMinDistancePoints.Count == 0 || minID >= costList.Count)
            {
                currentDistanceListenerToEmitter = (listenerObj.transform.position - emitterObj.transform.position).sqrMagnitude;
            }
            else
            {
                currentDistanceListenerToEmitter = costList[minID];
                float distanceToCurrentPoint = (emitterObj.transform.position - soundDistancePoints[emitterObj.currentPointID].transform.position).sqrMagnitude;
                //EmitterがListener側に向かってきていたらその分距離を近づける。逆方向なら離れている分距離を追加する
                bool isEmitterComeOn = calcMinDistancePoints.FirstOrDefault(x => x.ID == emitterObj.nextTargetPointID) != null;
                if (isEmitterComeOn)    currentDistanceListenerToEmitter -= distanceToCurrentPoint;
                else                    currentDistanceListenerToEmitter += distanceToCurrentPoint;

                //Listenerの方も同様に計算
                //ListenerがEmitterへ向かうように動いているか
                float distanceToCurrentPointListener = (listenerObj.transform.position - soundDistancePoints[listenerObj.currentPointID].transform.position).sqrMagnitude;
                bool isListenerHeadToEmitter = calcMinDistancePoints.FirstOrDefault(x => x.ID == listenerObj.nextTargetPointID) != null;
                if (isListenerHeadToEmitter) currentDistanceListenerToEmitter -= distanceToCurrentPointListener;
                else currentDistanceListenerToEmitter += distanceToCurrentPointListener;
            }
        }

        /// <summary>
        /// 最短距離にある各Pointに、聞こえる方向にある隣のPointを設定
        /// </summary>
        /// <param name="_routeList"></param>
        private void SetTargetEachPoint(IReadOnlyList<SoundDistancePoint> _routeList)
        {
            int startNum = _routeList.Count - 1;
            //goal->startの順になっているので、逆からループさせる
            for (int i = startNum; i > 0; i--)
            {
                if(i > 0)
                {
                    _routeList[i].SetEnitterDirectionPoint(_routeList[i - 1]);
                }
            }
        }

        /// <summary>
        /// 音が聞こえる方向（ターゲット）を設定
        /// </summary>
        /// <param name="_eachRouteList"></param>
        /// <param name="_minID"></param>
        private void SetSoundMakerTarget(IReadOnlyList<IReadOnlyList<SoundDistancePoint>> _eachRouteList, int _minID)
        {
            if (_eachRouteList.Count == 0) {
                listenerObj.SetEmitDirectionPointID(soundDistancePoints[listenerObj.currentPointID].ID);
                return;
            }

            SoundDistancePoint target = null;
            if (_eachRouteList[_minID].Count >= 2)
            {
                listenerObj.SetEmitDirectionPointID(_eachRouteList[_minID][_eachRouteList[_minID].Count - 2].ID);
                target = _eachRouteList[_minID][_eachRouteList[_minID].Count - 2];//最後の要素の1つ前 = スタートの隣のPoint情報
            }
            else
            {
                //カウントが2未満＝同じ場所にいるのでそのままStartを設定
                target = _eachRouteList[_minID][_eachRouteList[_minID].Count - 1];
            }

            SetTargetPositionToMaker(target);
        }
        private void SetTargetPositionToMaker(SoundDistancePoint target)
        {
            if (target == null) return;
            Vector3 targetPosition = Vector3.zero;
            if (target.currentEmitterDirectionPoint != null)
            {
                //targetとcurrentPointの距離が近すぎるとtarget.EmitterDirectionPointがListenerのcurrentPoint寄りになってしまい、反対方向にMakerが配置されるので判定を追加
                float distance = (soundDistancePoints[listenerObj.currentPointID].transform.position - soundDistancePoints[listenerObj.currentPointID].EmitterDirectionPoint).sqrMagnitude;
                float near = 3 * 3 + 1;
                if (distance <= near)
                {
                    targetPosition = target.soundAncherPosition;
                }
                else
                {
                    targetPosition = target.EmitterDirectionPoint;
                }
            }
            else
            {
                targetPosition = target.soundAncherPosition;
            }
            //Listenerが音が聞こえる方向と逆方向に進んでいたら、最後に出たPointの位置をセット
            if (listenerObj.nextTargetPointID != -1 && target.ID != listenerObj.nextTargetPointID)
            {
                if (soundDistancePoints[listenerObj.currentPointID].currentEmitterDirectionPoint != null)
                {
                    targetPosition = soundDistancePoints[listenerObj.currentPointID].EmitterDirectionPoint;
                }
                else
                {
                    targetPosition = soundDistancePoints[listenerObj.currentPointID].soundAncherPosition;
                }
            }
            //EmitterとListenerがお互い正面から近づいている場合・EmitterとListenerが同じ場所にいる場合
            if(listenerObj.nextTargetPointID == emitterObj.currentPointID && emitterObj.nextTargetPointID == listenerObj.currentPointID ||
                listenerObj.currentPointID == emitterObj.currentPointID || listenerObj.nextTargetPointID == emitterObj.nextTargetPointID)
            {
                targetPosition = emitterObj.transform.position;
            }
            SetSoundMakerTargetPosition(targetPosition);
        }
        private void SetSoundMakerTargetPosition(Vector3 targetPosition)
        {
            soundMaker.SetTargetPosition(targetPosition);

            if (!soundMaker.isActionable)
            {
                soundMaker.DoAction();
            }
        }

        /// <summary>
        /// 前回のダイクストラ法計算で残っている情報をクリア
        /// </summary>
        private void ClearAllPointPrevData()
        {
            for (int i = 0; i < soundDistancePoints.Count; i++)
            {
                soundDistancePoints[i].ClearPrev();
            }
        }
        /// <summary>
        /// 指定したノードとPrevとの距離を返す
        /// </summary>
        /// <param name="_point"></param>
        /// <param name="prevID"></param>
        /// <returns></returns>
        private float GetPrevMagnitude(SoundDistancePoint _point, int prevID = 0)
        {
            float mag = 0f;
            if(prevID >= _point.prevPointList.Count) { Debug.Log("prevの要素数オーバー"); return mag; }

            SoundDistancePointNode node = _point.GetPointNodeWithID(_point.prevPointList[prevID].ID);
            if (node != null)
            {
                mag = node.distanceMagnitude;
            }
            else { Debug.Log("ノードがありません : " + _point.prevPointList[prevID].ID); }
            return mag;
        }
        #endregion

        #region Collider
        /// <summary>
        /// Listenerが当たり判定に当たった時に呼び出す
        /// </summary>
        /// <param name="point"></param>
        public void SetPointToListener(SoundDistancePoint point)
        {
            listenerObj.SetPrevPointID(listenerObj.currentPointID);
            listenerObj.SetCurrentPointID(point.ID);
            listenerObj.currentHittingPoint = point;
            ReCalcDikstra();
        }
        /// <summary>
        /// Listenerが当たり判定から出た時に呼び出す
        /// </summary>
        public void ExitPointToListener(int _exitPointID, int _nextPointID)
        {
            listenerObj.currentHittingPoint = null;
            listenerObj.SetNextTargetPointID(_nextPointID);
            ReCalcDikstra();
        }

        /// <summary>
        /// Emitterが当たり判定に当たった時に呼び出す
        /// </summary>
        /// <param name="point"></param>
        public void SetPointToEmitter(SoundDistancePoint point)
        {
            emitterObj.SetPointID(point.ID);
            ReCalcDikstra();
        }
        /// <summary>
        /// Emitterが当たり判定から出た時に呼び出す
        /// </summary>
        public void ExitPointToEmitter(int _exitPointID, int _nextPointID)
        {
            emitterObj.SetNextTargetPointID(_nextPointID);
            ReCalcDikstra();
        }

        private void ReCalcDikstra()
        {
            if (!IsAllInitSeted()) return;
            if (!isActionFrame)
            {
                CalcRouteSeachUpdate();
                ResetFrameCount();
            }
        }
        #endregion

        #region SoundDistanceMaker
        /// <summary>
        /// SoundDistanceMakerの初期設定・動作させる
        /// </summary>
        /// <param name="_clip"></param>
        /// <param name="_maxVolume"></param>
        public void StartSoundDistanceMaker(AudioClip _clip, float _maxVolume)
        {
            if(_clip == null) { Debug.LogError("AudioClipがありません"); return; }
            soundMaker.maxVolume = _maxVolume;
            soundMaker.SetClipAndPlay(_clip, 0);
        }
        public void StopSoundDistanceMaker()
        {
            soundMaker.SoundStop();
        }
        public void SetMaxVolumeToMaker(float _maxVolume)
        {
            soundMaker.maxVolume = _maxVolume;
        }
        public void SetSoundMakerSoundEnable(bool _enable)
        {
            if (_enable)
            {
                soundMaker.SoundUnPause();
            }
            else
            {
                soundMaker.SoundPause();
            }
        }
        #endregion
    }
}

/// <summary>
/// Dictionaryをインスペクターに表示する機能
/// </summary>
namespace Serialize
{

    /// <summary>
    /// テーブルの管理クラス
    /// </summary>
    [System.Serializable]
    public class TableBase<TKey, TValue, Type> where Type : KeyAndValue<TKey, TValue>
    {
        [SerializeField]
        private List<Type> list;
        private Dictionary<TKey, TValue> table;


        public Dictionary<TKey, TValue> GetTable()
        {
            if (table == null)
            {
                table = ConvertListToDictionary(list);
            }
            return table;
        }

        /// <summary>
        /// Editor Only
        /// </summary>
        public List<Type> GetList()
        {
            return list;
        }

        static Dictionary<TKey, TValue> ConvertListToDictionary(List<Type> list)
        {
            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();
            foreach (KeyAndValue<TKey, TValue> pair in list)
            {
                dic.Add(pair.Key, pair.Value);
            }
            return dic;
        }
    }

    /// <summary>
    /// シリアル化できる、KeyValuePair
    /// </summary>
    [System.Serializable]
    public class KeyAndValue<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;

        public KeyAndValue(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
        public KeyAndValue(KeyValuePair<TKey, TValue> pair)
        {
            Key = pair.Key;
            Value = pair.Value;
        }
    }
}