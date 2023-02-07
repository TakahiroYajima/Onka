using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SoundDistance
{
    /// <summary>
    /// 建物内で曲がり角から曲がった先で発する音が聞こえるようにする機能の管理
    /// 注意：現状、枝分かれ後は1本道にしか対応していないので、編集時には注意する事
    /// </summary>
    public class SoundDistanceManager : SingletonMonoBehaviour<SoundDistanceManager>
    {
        [System.NonSerialized] public bool isActive = false;
        [SerializeField] private SoundDistanceListener listenerObj = null;//音を聞く側
        public SoundDistanceListener Listener { get { return listenerObj; } }
        [SerializeField] private SoundDistanceEmitter emitterObj = null;//音を発する側
        public SoundDistanceEmitter Emitter { get { return emitterObj; } }
        [SerializeField] private SoundDistanceMakerObj soundMaker = null;//listenerObjから音の方向に位置取り、サウンドを鳴らすオブジェクト
        public SoundDistanceMakerObj Maker { get { return soundMaker; } }

        [SerializeField, Range(0, 1f)] private float canNotHearRatio = 0.5f;//音が聞こえなくなる距離の割合（外周に対してどれだけEmitterとListenerが離れていれば音が聞こえなくなるかの割合）
        /// <summary>
        /// 音が聞こえなくなる距離の割合（外周に対してどれだけEmitterとListenerが離れていれば音が聞こえなくなるかの割合）
        /// </summary>
        public float CanNotHearRatio { get { return canNotHearRatio; } }

        [field: SerializeField, ReadOnly]public List<SoundDistancePoint> soundDistancePoints { get; private set; } = new List<SoundDistancePoint>();
        public float OuterCircumference { get; private set; } = 0f;//1周の距離（袋小路は考慮しない）

        //ダイクストラで計算された最短距離（goalからstartまで）
        private List<List<SoundDistancePoint>> serchEachRouteList = new List<List<SoundDistancePoint>>();//全ルート
        public List<SoundDistancePoint> calcMinDistancePoints { get; private set; } = new List<SoundDistancePoint>();//最短ルート
        private List<float> costList = new List<float>();
        private float subCost = 0f;
        private float adjustmentEachMovedDistance = 0f;//PointからListenerとEmitterが移動した距離によって調整する→ボリューム変化が滑らかになる
        private int currentMinID = -1;
        //現在のListenerからEmitterまでの距離（ダイクストラで計算された最短距離）
        public float currentDistanceListenerToEmitter { get; private set; } = 0f;

        #region フレームカウント
        //フレームカウント
        private const int ActionFrameNum = 6;//ActionFrameNumフレームに1回処理させる
        private int currentFrame = 0;
        private bool isActionFrame { get { return currentFrame >= ActionFrameNum; } }
        #endregion

        public SoundDistancePoint GetSoundDistancePoint(string key)
        {
            return soundDistancePoints.FirstOrDefault(v => v.pointKey == key);
        }
        public void SetUp(SoundDistanceListener listener, SoundDistanceEmitter emitter)
        {
            listenerObj = listener;
            emitterObj = emitter;
        }

        public void Initialize()
        {
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
            if (!isActive) return;
            if (!IsAllInitSeted()) return;
            if (DoUpdateFrameAndJudge())
            {
                //Debug.Log($"SD_IDs emitter : {emitterObj.currentOuterPointID} {emitterObj.currentPointID} listener : {listenerObj.currentOuterPointID} {listenerObj.currentPointID}");
                CalcRouteSeachUpdate();
            }

            SetCurrentDistanceListenerToEmitter(costList, currentMinID);
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
        /// <summary>
        /// 音の設定（ボリューム）を先にさせておくため、一度強制で計算させる
        /// </summary>
        public void ForceInitCalc()
        {
            CalcRouteSeachUpdate();
            SetCurrentDistanceListenerToEmitter(costList, currentMinID);
        }
        #endregion

        #region RouteSerch
        /// <summary>
        /// ListenerからEmitterまでのルートをserchEachRouteListに設定する
        /// </summary>
        /// <param name="emitterCurrentPointID"></param>
        /// <param name="listenerCurrentPointID"></param>
        private void SetAllRouteToDikstraAllRouteList(int emitterCurrentPointID, int listenerCurrentPointID)
        {
            ClearAllPointPrevData();//Prev情報を初期化
            this.serchEachRouteList.Clear();
            this.serchEachRouteList.Add(new List<SoundDistancePoint>());
            this.costList.Clear();
            this.costList.Add(0f);

            SoundDistancePoint goal = soundDistancePoints[emitterCurrentPointID];
            int routeArrayNum = 0;
            //emitterCurrentIDとlistenerCurrentIDが一致していたら同じ場所（or同じ部屋の入口）にいるのでreturn
            if (emitterCurrentPointID == listenerCurrentPointID)
            {
                //goalだけは追加する
                
                this.serchEachRouteList[0].Add(goal);
                return;
            }

            List<SoundDistancePoint> currentSearchList = new List<SoundDistancePoint>();//探索中のノード
            List<SoundDistancePoint> searchedPointList = new List<SoundDistancePoint>();//次のループでcurrentSearchListに差し替えるノード

            currentSearchList.Add(goal);
            bool isEnd = false;
            //EmitterがいるところからListenerがいるところまで
            while (!isEnd)
            {
                searchedPointList.Clear();
                if(currentSearchList.Count == 0) { break; }

                for (int i = 0; i < currentSearchList.Count; i++)
                {
                    foreach (Direction fromGoalDirection in Enum.GetValues(typeof(Direction)))
                    {
                        SoundDistancePointNode n = currentSearchList[i].GetPointNode(fromGoalDirection);
                        //逆走できないようにする(Prevに設定されていない場所だけ追加できる)
                        if (!n.IsExist) { continue; }
                        bool isAddable = currentSearchList[i].prevPointList.FirstOrDefault(x => x.ID == n.node.ID) == null;
                        if (!isAddable) { continue; }
                        n.node.AddPrev(currentSearchList[i]);
                        searchedPointList.Add(n.node);
                        if(n.node.ID == listenerCurrentPointID)
                        {
                            isEnd = true;
                            break;
                        }
                    }
                    if (isEnd) { break; }
                }
                currentSearchList.Clear();
                currentSearchList = new List<SoundDistancePoint>(searchedPointList);

               
            }

            SoundDistancePoint p = soundDistancePoints[listenerCurrentPointID];
            while (true)
            {
                this.serchEachRouteList[routeArrayNum].Add(p);
                if (p.ID == emitterCurrentPointID) { break; }
                else
                {
                    this.costList[routeArrayNum] += (p.transform.position - p.prevPointList[0].transform.position).sqrMagnitude;
                    p = p.prevPointList[0];
                }
            }
            this.serchEachRouteList[routeArrayNum].Reverse();

            //string dev = "";
            //for(int i = 0; i < serchEachRouteList[routeArrayNum].Count; i++)
            //{
            //    dev += serchEachRouteList[routeArrayNum][i].gameObject.name + " ";
            //}
            //Debug.Log("serch : " + dev);
            //Debug.Log("cost : " + costList[routeArrayNum]);
        }

        /// <summary>
        /// ゴールから全ての方向のルートを辿り、スタートまでの道のりをserchEachRouteListに設定する
        /// リスト順番：0←(Emitter・Listener)→List.Count
        /// </summary>
        /// <param name="emitterCurrentOuterID"></param>
        /// <param name="listenerCurrentOuterID"></param>
        //private void SetAllRouteToDikstraEachOuterRouteList(int emitterCurrentOuterID,int emitterCurrentPointID,  int listenerCurrentOuterID)
        //{
        //    ClearAllPointPrevData();//Prev情報を初期化
        //    this.serchEachRouteList.Clear();
        //    this.costList.Clear();

        //    SoundDistancePoint goal = soundDistancePoints.FirstOrDefault(x => x.ID == emitterCurrentOuterID);
        //    int routeArrayNum = 0;
        //    //emitterCurrentIDとlistenerCurrentIDが一致していたら同じ場所（or同じ部屋の入口）にいるのでreturn
        //    if (emitterCurrentOuterID == listenerCurrentOuterID) {
        //        if (emitterCurrentOuterID == emitterCurrentPointID)
        //        {
        //            //goalだけは追加する
        //            this.serchEachRouteList.Add(new List<SoundDistancePoint>());
        //            this.serchEachRouteList[0].Add(goal);
        //            return;
        //        }
        //        else
        //        {
        //            //部屋の中・枝分かれルートの奥に入っていたら何も入れない
        //            this.serchEachRouteList.Add(new List<SoundDistancePoint>());
        //            return;
        //        }
        //    }

        //    foreach (Direction fromGoalDirection in Enum.GetValues(typeof(Direction)))
        //    {
        //        SoundDistancePointNode n = goal.GetPointNode(fromGoalDirection);
        //        if(!n.IsExist || !n.IsOuter) { continue; }

        //        this.serchEachRouteList.Add(new List<SoundDistancePoint>());
        //        this.serchEachRouteList[routeArrayNum].Add(goal);
        //        n.node.AddPrev(goal);
        //        this.costList.Add(0f);
                
        //        //goalから1方向のルートを順番に探索するため、最初にgoalの隣のノードを追加しておく
        //        this.serchEachRouteList[routeArrayNum].Add(n.node);
        //        this.costList[routeArrayNum] += n.distanceMagnitude;

        //        for (int i = 1; true; i++)
        //        {
        //            SoundDistancePoint currentSeachPoint = this.serchEachRouteList[routeArrayNum][i];
        //            //スタート地点に来ていたら終了
        //            if (currentSeachPoint.ID == listenerCurrentOuterID) { break; }

        //            //全ての方向のデータ分を回す
        //            foreach (Direction d in Enum.GetValues(typeof(Direction)))
        //            {
        //                SoundDistancePointNode node = currentSeachPoint.GetPointNode(d);
        //                //外周のものをルートに追加
        //                if (node.IsExist && node.IsOuter)
        //                {
        //                    //逆走できないようにする(Prevに設定されていない場所だけ追加できる)
        //                    bool isAddable = currentSeachPoint.prevPointList.FirstOrDefault(x => x.ID == node.node.ID) == null;
        //                    if (isAddable)
        //                    {
        //                        //次のノードのPrevに現在のノードを設定、距離も追加
        //                        node.node.AddPrev(currentSeachPoint);
        //                        this.serchEachRouteList[routeArrayNum].Add(node.node);
        //                        this.costList[routeArrayNum] += node.distanceMagnitude;
        //                    }
        //                }
        //            }
        //        }
        //        routeArrayNum++;
        //    }
        //}
        /// <summary>
        /// 外周から外れた部分（部屋など）にListenerがいる時、そこから外周へ戻るまでのルートを取得
        /// </summary>
        //private List<SoundDistancePoint> CalcSerachSideRoute(int currentListenerOuterID, int currentListenerPointID)
        //{
        //    List<List<SoundDistancePoint>> points = new List<List<SoundDistancePoint>>();
        //    List<float> subCostList = new List<float>();
        //    SoundDistancePoint outer = soundDistancePoints.FirstOrDefault(x => x.ID == currentListenerOuterID);
        //    //Debug.Log($"outer : {outer.gameObject.name} ID : {currentListenerOuterID}");
        //    int routeCount = 0;
        //    bool isEnd = false;
            
        //    //
        //    SoundDistancePoint currentPoint = soundDistancePoints.FirstOrDefault(x => x.ID == listenerObj.currentPointID);
        //    foreach (Direction d in Enum.GetValues(typeof(Direction)))
        //    {
        //        SoundDistancePointNode n = currentPoint.GetPointNode(d);
        //        points.Add(new List<SoundDistancePoint>());
        //        points[routeCount].Add(currentPoint);
        //        subCostList.Add(0f);
        //        if (!n.IsExist) { routeCount++; continue; }//ノードが無いならcontinue
        //        subCostList[routeCount] += n.distanceMagnitude;
        //        //Debug.Log(n.distanceMagnitude);
        //        //枝分かれする元の外周に当たったら計算完了
        //        if (n.IsOuter)
        //        {
        //            if(n.node.ID == currentListenerOuterID)
        //            {
        //                isEnd = true;
        //                break;
        //            }
        //        }
        //        //まだ枝分かれのルートなので、外周に当たるまで計算
        //        n.node.AddPrev(currentPoint);
        //        points[routeCount].Add(n.node);
                
        //        for (int i = 1; true; i++)
        //        {
        //            SoundDistancePoint currentSeachPoint = points[routeCount][i];
        //            //枝分かれする元の外周に当たったら計算完了
        //            if (currentSeachPoint.ID == currentListenerOuterID) { isEnd = true; break; }

        //            int missingCount = 0;
        //            //全ての方向のデータ分を回す
        //            foreach (Direction d2 in Enum.GetValues(typeof(Direction)))
        //            {
        //                SoundDistancePointNode node = currentSeachPoint.GetPointNode(d2);
        //                //枝分かれする元の外周に当たったら計算完了
        //                if (node.IsOuter)
        //                {
        //                    if (node.node.ID == currentListenerOuterID)
        //                    {
        //                        isEnd = true;
        //                        break;
        //                    }
        //                }
        //                if (node.IsExist)
        //                {
        //                    //逆走できないようにする(Prevに設定されていない場所だけ追加できる)
        //                    bool isAddable = currentSeachPoint.prevPointList.FirstOrDefault(x => x.ID == node.node.ID) == null;
        //                    //Debug.Log($"subCostAdd? : {isAddable} {node.distanceMagnitude}");
        //                    if (isAddable)
        //                    {
        //                        //Debug.Log($"NodeAdd : {routeCount} {node.node.gameObject.name}");
        //                        //次のノードのPrevに現在のノードを設定、距離も追加
        //                        node.node.AddPrev(currentSeachPoint);
        //                        points[routeCount].Add(node.node);
        //                        subCostList[routeCount] += node.distanceMagnitude;
        //                    }
        //                    else
        //                    {
        //                        missingCount++;
        //                    }
        //                }
        //                else
        //                {
        //                    missingCount++;
        //                }
        //            }
        //            //行き止まりなら次のルートへ
        //            if (missingCount >= Enum.GetValues(typeof(Direction)).Length || i >= points[routeCount].Count || isEnd)
        //            {
        //                break;
        //            }
        //        }
        //        if (isEnd) { break; }
        //        routeCount++;
        //    }
        //    //string routesStr = "";
        //    //for(int i = 0; i < points[routeCount].Count; i++)
        //    //{
        //    //    routesStr += points[routeCount][i].gameObject.name + " ";
        //    //}
        //    //Debug.Log($"routes : {routesStr}");
        //    subCost = subCostList[routeCount];
        //    if (points[routeCount].Count >= 2)
        //    {
        //        points[routeCount].Reverse();
        //    }
        //    return points[routeCount];//ルートが見つかると強制終了なので、そのままカウントを参照すればそのルートになる
        //}
        
        /// <summary>
        /// 全ルート探索でEmitterまでの経路を計算。Listenerに音を設定させる
        /// </summary>
        private void CalcRouteSeachUpdate()
        {
            subCost = 0f;
            SetAllRouteToDikstraAllRouteList(emitterObj.currentPointID, listenerObj.currentPointID);
            //SetAllRouteToDikstraEachOuterRouteList(emitterObj.currentOuterPointID, emitterObj.currentPointID, listenerObj.currentOuterPointID);
            //ゴールからスタートまでのルート（複数）の中から最短ルートの配列番号を取得
            currentMinID = GetMinID(costList);

            //costにListenerとEmitterの移動済み距離も加味する
            adjustmentEachMovedDistance = 0f;
            CalcAdjustmentEachMovedDistanceListener();
            CalcAdjustmentEachMovedDistanceEmitter();
            
            //Debug.Log("現在のコスト : " + costList[currentMinID]);
            //同じ地点にいたらEmitterの位置を直接設定
            if (listenerObj.currentPointID == emitterObj.currentPointID)
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
        /// ListenerがcurrentPointから動いた分だけボリューム変化させる
        /// </summary>
        private void CalcAdjustmentEachMovedDistanceListener()
        {
            if (IsInTheNoPassageOrInRoomNearOuter())
            {
                adjustmentEachMovedDistance += (listenerObj.transform.position - soundDistancePoints[listenerObj.currentOuterPointID].transform.position).sqrMagnitude;
                //Debug.Log($"L:行き止まり = 遠ざかっている {adjustmentEachMovedDistance}");
                return;
            }
            //Listenerにとって、currentとnextのどちらの方向から音が聞こえるかを確認・保持
            if (serchEachRouteList[currentMinID].Count >= 2)
            {
                float sqrMagnitude = 0f;
                if (serchEachRouteList[currentMinID][serchEachRouteList[currentMinID].Count - 1].ID == listenerObj.currentPointID)
                {
                    //Emitterの方へ向かっている状態
                    if (serchEachRouteList[currentMinID][serchEachRouteList[currentMinID].Count - 2].ID == listenerObj.nextTargetPointID)
                    {
                        //1つ前のルートPointとの距離を取得
                        sqrMagnitude =
                            serchEachRouteList[currentMinID][serchEachRouteList[currentMinID].Count - 1]
                            .PointNodes.GetList().Where(x => x.Value.IsExist)
                            .FirstOrDefault(x => x.Value.node.ID == listenerObj.nextTargetPointID)
                            .Value.distanceMagnitude;
                        float pointToPlayerDistanceMagnitude = (listenerObj.transform.position - serchEachRouteList[currentMinID][serchEachRouteList[currentMinID].Count - 1].transform.position).sqrMagnitude;
                        
                        adjustmentEachMovedDistance -= Mathf.Clamp(pointToPlayerDistanceMagnitude, 0f, sqrMagnitude);//(sqrMagnitude - Mathf.Clamp(pointToPlayerDistanceMagnitude, 0f, sqrMagnitude));
                        //Debug.Log($"L:next = 向かっている {adjustmentEachMovedDistance}");
                    }
                    //Emitterから遠ざかっている場合
                    else
                    {
                        //current = 遠ざかっている
                        //向かっている先のPointへの距離を取得
                        if (listenerObj.currentPointID != listenerObj.nextTargetPointID)
                        {
                            var p = soundDistancePoints[Listener.nextTargetPointID]
                                .PointNodes.GetList().Where(x => x.Value.IsExist)
                                .FirstOrDefault(x => x.Value.node.ID == listenerObj.currentPointID);
                            if (p != null && p != default) {
                                sqrMagnitude = p.Value.distanceMagnitude;
                            }
                            float pointToPlayerDistanceMagnitude = (listenerObj.transform.position - serchEachRouteList[currentMinID][serchEachRouteList[currentMinID].Count - 1].transform.position).sqrMagnitude;
                            
                            adjustmentEachMovedDistance += Mathf.Clamp(pointToPlayerDistanceMagnitude, 0f, sqrMagnitude);
                            //Debug.Log($"L:current = 遠ざかっている {adjustmentEachMovedDistance} next : {soundDistancePoints[listenerObj.nextTargetPointID].gameObject.name}");
                        }
                        else
                        {
                            //行き止まりからOuterに戻ってきたタイミング＝当たり判定の中なので何もしない
                        }
                    }
                }
                else if (serchEachRouteList[currentMinID][serchEachRouteList[currentMinID].Count - 1].ID == listenerObj.nextTargetPointID)
                {
                    //枝分かれルートのPointからOuterのPointに向かっている状態
                    //currentEmitterDirectionPointFromListener = soundDistancePoints[listenerObj.currentOuterPointID];
                    adjustmentEachMovedDistance += (listenerObj.transform.position - soundDistancePoints[listenerObj.currentOuterPointID].transform.position).sqrMagnitude;
                    Debug.LogError("想定外");
                    //Debug.LogError($"serchEachRouteList is beyond expectations data : nextID :: {serchEachRouteList[currentMinID][serchEachRouteList[currentMinID].Count - 1].ID}");
                    ////next = 向かっている
                    ////1つ前のルートPointから距離を取得
                    //sqrMagnitude =
                    //    serchEachRouteList[currentMinID][serchEachRouteList[currentMinID].Count - 2]
                    //    .PointNodes.GetList().Where(x => x.Value.IsExist)
                    //    .FirstOrDefault(x => x.Value.node.ID == listenerObj.nextTargetPointID)
                    //    .Value.distanceMagnitude;
                    //float pointToPlayerDistanceMagnitude = (listenerObj.transform.position - serchEachRouteList[currentMinID][serchEachRouteList[currentMinID].Count - 1].transform.position).sqrMagnitude;
                    //Debug.Log($"next = 向かっている2 {sqrMagnitude} {pointToPlayerDistanceMagnitude} : {(sqrMagnitude - Mathf.Clamp(pointToPlayerDistanceMagnitude, 0f, sqrMagnitude))}");
                    //adjustmentEachMovedDistance -= (sqrMagnitude - Mathf.Clamp(pointToPlayerDistanceMagnitude, 0f, sqrMagnitude));
                }
                else
                {
                    string str = "";
                    string sIDs = $"{listenerObj.prevPointID} {listenerObj.currentPointID} {listenerObj.nextTargetPointID}";
                    for (int errorI = 0; errorI < serchEachRouteList[currentMinID].Count; errorI++)
                    {
                        str += $"{serchEachRouteList[currentMinID][errorI].ID} ";
                    }
                    Debug.LogError($"serchEachRouteList is beyond expectations data : {str} id:{sIDs}");
                }
            }
            else
            {
                //countが1つだけ＝ListenerのcurrentとEmitterのcurrentが一致→同じ場所にいる
            }
        }
        private void CalcAdjustmentEachMovedDistanceEmitter()
        {
            //行き止まり
            if (emitterObj.currentPointID == emitterObj.nextTargetPointID)
            {
                //adjustmentEachMovedDistance += (listenerObj.transform.position - soundDistancePoints[listenerObj.currentPointID].transform.position).sqrMagnitude;
                //Debug.Log($"行き止まり = 遠ざかっている {adjustmentEachMovedDistance}");
                //Listenerと同じ空間にいる
                if(listenerObj.currentPointID == listenerObj.nextTargetPointID)
                {

                }
                else
                {
                    //部屋徘徊中にListenerが脱出したパターン
                    adjustmentEachMovedDistance += (emitterObj.transform.position - soundDistancePoints[emitterObj.currentPointID].transform.position).sqrMagnitude;
                }
                return;
            }
            //nextTargetPointIDは各Pointの当たり判定から出た時に設定される
            bool isEmitterComeOn = serchEachRouteList[currentMinID].FirstOrDefault(x => x.ID == emitterObj.nextTargetPointID) != null;
            float sqrMagnitude = (emitterObj.transform.position - soundDistancePoints[emitterObj.currentPointID].transform.position).sqrMagnitude;
            if (isEmitterComeOn)
            {
                //sqrMagnitude =
                //            serchEachRouteList[currentMinID][0]
                //            .PointNodes.GetList().Where(x => x.Value.IsExist)
                //            .FirstOrDefault(x => x.Value.node.ID == emitterObj.nextTargetPointID)
                //            .Value.distanceMagnitude;
                //float pointToPlayerDistanceMagnitude = (emitterObj.transform.position - serchEachRouteList[currentMinID][0].transform.position).sqrMagnitude;

                //adjustmentEachMovedDistance -= Mathf.Clamp(pointToPlayerDistanceMagnitude, 0f, sqrMagnitude);
                adjustmentEachMovedDistance -= sqrMagnitude;
                //Debug.Log($"next = Emitter:向かっている {-sqrMagnitude}");
            }
            else
            {
                adjustmentEachMovedDistance += sqrMagnitude;
                //Debug.Log($"next = Emitter:遠ざかっている {sqrMagnitude}");
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
            bool isCreep = listenerObj.currentPointID == emitterObj.currentPointID;//Emitterの真後ろにListenerがいる場合 listenerObj.currentPointID == emitterObj.currentPointID;
            if ((isCloseBy || isCreep || calcMinDistancePoints.Count == 0) && ((minID >= costList.Count || minID < 0) && !IsInTheNoPassageOrInRoomNearOuter()))
            {
                currentDistanceListenerToEmitter = (listenerObj.transform.position - emitterObj.transform.position).sqrMagnitude;
                //Debug.Log($"接近 {currentDistanceListenerToEmitter} {isCloseBy.ToString()} {isCreep.ToString()} {calcMinDistancePoints.Count} {minID}:{costList.Count}");
            }
            else
            {
                //string debStr = "";

                //for (int i = 0; i < calcMinDistancePoints.Count; i++)
                //{
                //    debStr += calcMinDistancePoints[i].gameObject.name + " ";
                //}
                if (minID >= 0 && minID < costList.Count)
                {
                    currentDistanceListenerToEmitter = costList[minID] + adjustmentEachMovedDistance;
                    //Debug.Log($"通常 : {currentDistanceListenerToEmitter} sub : {subCost} adj : {adjustmentEachMovedDistance} routes : {debStr}");
                }
                else
                {
                    currentDistanceListenerToEmitter = 
                        (listenerObj.transform.position - soundDistancePoints[listenerObj.currentOuterPointID].transform.position).sqrMagnitude +
                        (emitterObj.transform.position - soundDistancePoints[emitterObj.currentOuterPointID].transform.position).sqrMagnitude;
                    //Debug.Log($"接近2 {currentDistanceListenerToEmitter} sub : {subCost} adj : {adjustmentEachMovedDistance} routes : {debStr} :: {calcMinDistancePoints.Count} {minID}:{costList.Count}");
                }
                //float distanceToCurrentPoint = (emitterObj.transform.position - soundDistancePoints[emitterObj.currentPointID].transform.position).sqrMagnitude;
                ////EmitterがListener側に向かってきていたらその分距離を近づける。逆方向なら離れている分距離を追加する
                //bool isEmitterComeOn = calcMinDistancePoints.FirstOrDefault(x => x.ID == emitterObj.nextTargetPointID) != null;
                //if (isEmitterComeOn)    currentDistanceListenerToEmitter -= distanceToCurrentPoint;
                //else                    currentDistanceListenerToEmitter += distanceToCurrentPoint;

                ////Listenerの方も同様に計算
                ////ListenerがEmitterへ向かうように動いているか
                //float distanceToCurrentPointListener = (listenerObj.transform.position - soundDistancePoints[listenerObj.currentPointID].transform.position).sqrMagnitude;
                //bool isListenerHeadToEmitter = calcMinDistancePoints.FirstOrDefault(x => x.ID == listenerObj.nextTargetPointID) != null;//nextTargetPointIDは各Pointの当たり判定から出た時に設定される
                //if (isListenerHeadToEmitter) currentDistanceListenerToEmitter -= distanceToCurrentPointListener;
                //else currentDistanceListenerToEmitter += distanceToCurrentPointListener;
                //Debug.Log($"{OuterCircumference} : {currentDistanceListenerToEmitter.ToString("f2")} sub : {subCost}");
                //Debug.Log($"通常の設定 {OuterCircumference} :: {costList[minID]} : {isEmitterComeOn}:{distanceToCurrentPoint.ToString("f2")} {isListenerHeadToEmitter}:{distanceToCurrentPointListener.ToString("f2")} :: {currentDistanceListenerToEmitter}");
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
            //ある程度離れている場合
            if (_eachRouteList[_minID].Count >= 2)
            {
                //遠ざかっているときに、「自分の2つ向こう」のpointを参照してしまうため、「自分の1つ向こう」を参照するように調整
                if (_eachRouteList[currentMinID].Contains(soundDistancePoints[listenerObj.nextTargetPointID]))
                {
                    listenerObj.SetEmitDirectionPointID(_eachRouteList[_minID][_eachRouteList[_minID].Count - 2].ID);
                    target = _eachRouteList[_minID][_eachRouteList[_minID].Count - 2];//最後の要素の1つ前 = スタートの隣のPoint情報
                }
                else
                {
                    listenerObj.SetEmitDirectionPointID(_eachRouteList[_minID][_eachRouteList[_minID].Count - 1].ID);
                    target = _eachRouteList[_minID][_eachRouteList[_minID].Count - 1];//最後の要素の1つ前 = スタートの隣のPoint情報
                    
                }
                //string dev = ""; for (int i = 0; i < _eachRouteList[_minID].Count; i++) { dev += _eachRouteList[_minID][i].gameObject.name + " "; }
                //Debug.Log($"離れている場所 {target.gameObject.name} : {dev}");
            }
            else
            {
                //Debug.Log($"同じ場所 {_eachRouteList[_minID][_eachRouteList[_minID].Count - 1].gameObject.name}");
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
            //string devStr = "";
            //for(int i = 0; i < serchEachRouteList[currentMinID].Count; i++)
            //{
            //    devStr += $"{serchEachRouteList[currentMinID][i].ID} ";
            //}
            //Debug.Log(devStr);
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
        /// 行き止まり方向に入っている・部屋の中に入り、且つ枝分かれルートに当たっていない場合にtrue
        /// </summary>
        /// <returns></returns>
        private bool IsInTheNoPassageOrInRoomNearOuter()
        {
            return listenerObj.currentHittingPoint == null &&
                    (listenerObj.currentPointID == listenerObj.nextTargetPointID ||
                    (!soundDistancePoints[listenerObj.nextTargetPointID].IsOuter && soundDistancePoints[listenerObj.currentPointID].IsOuter));// || //枝分かれルートがある部屋に入った直後
                    //(soundDistancePoints[listenerObj.nextTargetPointID].IsOuter && !soundDistancePoints[listenerObj.currentPointID].IsOuter)); //枝分かれルートのPointからOuterに向かっているとき
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
            //Debug.Log($"setMaxVol {_maxVolume}");
            soundMaker.maxVolume = _maxVolume;
            soundMaker.SetClipAndPlay(_clip, 0);
        }
        public void StopSoundDistanceMaker()
        {
            soundMaker.SoundStop();
        }
        public void SetMaxVolumeToMaker(float _maxVolume)
        {
            //Debug.Log($"setMaxVol {_maxVolume}");
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
        public void SetVolumeONEnable(bool _isEnable)
        {
            soundMaker.isVolumeON = _isEnable;
        }
        #endregion

        #if UNITY_EDITOR
    public void SetSoundPoints()
        {
            soundDistancePoints.Clear();
            soundDistancePoints = transform.GetComponentsInChildren<SoundDistancePoint>().ToList();
        }
#endif
    }

#if UNITY_EDITOR
[CustomEditor(typeof(SoundDistanceManager))]
    public class CustomSoundDistanceManagerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var component = (SoundDistanceManager)target;
            //イベントのタイルを一斉にセットするボタンを表示
            if (GUILayout.Button("AllSoundPointsSetup"))
            {
                //https://light11.hatenadiary.com/entry/2019/10/12/170109
                //https://bluebirdofoz.hatenablog.com/entry/2021/08/17/224314
                component.SetSoundPoints();
                EditorUtility.SetDirty(component);
            }
        }
    }
#endif
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