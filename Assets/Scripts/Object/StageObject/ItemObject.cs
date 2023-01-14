using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Onka.Manager.Data;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private string itemKey = "";
    public string ItemKey { get { return itemKey; } }

    //ギミックを解いて入手するアイテム設定
    [SerializeField] private ItemGimickType gimmickType = ItemGimickType.None;
    public ItemGimickType GimmickType { get { return gimmickType; } }
    public bool isGimmickItem { get { return gimmickType != ItemGimickType.None; } }
    [field: SerializeField] public string gimmickKey { get; private set; }

    private Collider thisCollider = null;
    [SerializeField] private bool isDefaultInactive = false;//デフォルトで非表示にするか
    [SerializeField] private bool isHiddenItem = false;//どこかにしまってあるアイテムか

    /// <summary>
    /// 引き出しなど、動いた時に追従する対象を設定
    /// </summary>
    /// <param name="parent"></param>
    public void SetHiddenParent(Transform parent)
    {
        transform.parent = parent;
    }

    private int gettableCallCount = 0;//両開きの棚では両方閉まっていないと判定が消えないようにする
    
    // Start is called before the first frame update
    void Awake()
    {
        thisCollider = GetComponent<Collider>();
        if(thisCollider == null)
        {
            Debug.Log("アイテムにコライダーがありません : " + this.name);
            thisCollider = this.gameObject.AddComponent<BoxCollider>();
        }
    }

    public void Initialize()
    {//最初からアイテムを取得できないようにする
        if (isHiddenItem)
        {
            thisCollider.enabled = false;
        }
        gameObject.SetActive(!isDefaultInactive);

        ItemData _itemData = DataManager.Instance.GetItemData(itemKey);
        if(_itemData != null)
        {
            if (_itemData.geted)
            {
                OnGetedItemCallback(_itemData);
            }
        }
        gettableCallCount = 0;
    }
    
    /// <summary>
    /// 引き出しを開けたときなどに設定するコールバック
    /// </summary>
    public void DoGettableItem()
    {
        thisCollider.enabled = true;
        gettableCallCount++;
    }
    /// <summary>
    /// 引き出しを閉めたときなどに設定するコールバック
    /// </summary>
    public void DoNotGettableItem()
    {
        gettableCallCount--;
        if (gettableCallCount <= 0)
        {
            thisCollider.enabled = false;
        }
    }
    
    public void OnGetedItemCallback(ItemData _itemData)
    {
        switch (_itemData.type)
        {
            case ItemType.DoorKey:
                Destroy(gameObject);
                break;
            case ItemType.GetOnly:
                Destroy(gameObject);
                break;
            case ItemType.Useable:
                Destroy(gameObject);
                break;
            case ItemType.WatchOnly:

                break;
        }
    }
}

public enum ItemGimickType
{
    None,
    HighAltitude,//高所（棒か何かで取る）
    Eraser,//消しゴムで消すと何か見えてくるやつ
}