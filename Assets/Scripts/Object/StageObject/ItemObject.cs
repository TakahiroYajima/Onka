using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private string itemKey = "";
    public string ItemKey { get { return itemKey; } }
    private Collider thisCollider = null;
    [SerializeField] private bool isHiddenItem = false;//どこかにしまってあるアイテムか

    // Start is called before the first frame update
    void Start()
    {
        thisCollider = GetComponent<Collider>();
        if(thisCollider == null)
        {
            Debug.Log("アイテムにコライダーがありません : " + this.name);
            thisCollider = this.gameObject.AddComponent<BoxCollider>();
        }

        //最初からアイテムを取得できないようにする
        if (isHiddenItem)
        {
            thisCollider.enabled = false;
        }
    }

    public void Initialize()
    {
        ItemData _itemData = DataManager.Instance.GetItemData(itemKey);
        if(_itemData != null)
        {
            if (_itemData.geted)
            {
                OnGetedItemCallback(_itemData);
            }
        }
    }

    /// <summary>
    /// 引き出しを開けたときなどに設定するコールバック
    /// </summary>
    public void DoGettableItem()
    {
        thisCollider.enabled = true;
    }
    /// <summary>
    /// 引き出しを閉めたときなどに設定するコールバック
    /// </summary>
    public void DoNotGettableItem()
    {
        thisCollider.enabled = false;
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
