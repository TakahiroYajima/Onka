using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 順番に通過点を通って徘徊する動作のポイントを管理するクラス
/// 各通過点のGameObjectは、これがアタッチされているGameObjectの子に作成する事
/// </summary>
public class WanderingPointManager : SingletonMonoBehaviour<WanderingPointManager>
{
    public Dictionary<WanderingEnemyType, List<WanderingPoint>> wanderingPoints { get; private set; } = new Dictionary<WanderingEnemyType, List<WanderingPoint>>();

    // Start is called before the first frame update
    public void Initialize()
    {
        //徘徊用の通過ポイントを敵種類ごとに保存
        List<WanderingPoint> list = GetComponentsInChildren<WanderingPoint>().ToList();
        for(int i = 0; i < Enum.GetNames(typeof(WanderingEnemyType)).Length; i++)
        {
            WanderingEnemyType selectType = (WanderingEnemyType)Enum.ToObject(typeof(WanderingEnemyType), i);
            List<WanderingPoint> select = list.Where(x => x.WanderingEnemyType == selectType).ToList().OrderBy(x => x.PointNum).ToList();
            //Debug.Log("徘徊通過地点 : " + selectType.ToString());
            //foreach(var s in select)
            //{
            //    Debug.Log(s.PointNum);
            //}
            wanderingPoints.Add(selectType, select);
            for(int j = 0; j < wanderingPoints[selectType].Count; j++)
            {
                wanderingPoints[selectType][j].Initialize();
            }
        }
    }


}

/// <summary>
/// 徘徊する敵のタイプ選択用
/// </summary>
public enum WanderingEnemyType
{
    Yukie,

}