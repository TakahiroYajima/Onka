using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using SoundSystem;
using Onka.Manager.Data;

public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    private List<ItemObject> itemList = new List<ItemObject>();
    [SerializeField] private GameObject itemsParent = null;
    [SerializeField] private OneItemViewer oneItemViewerPref = null;
    private OneItemViewer instancedOneItemViewer = null;
    [SerializeField] private WatchDiaryManager watchDiaryManager = null;
    public WatchDiaryManager WatchDiaryManager { get { return watchDiaryManager; } }

    [SerializeField] private GameObject canvasUI = null;
    
    public UnityAction watchItemEventEndedCallback = null;
    public ItemData currentGettingItemData { get; private set; } = null;

    private bool isLoopFlg = false;

    public void Initialize()
    {
        itemList = itemsParent.GetComponentsInChildren<ItemObject>().ToList();
        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].HiddenParent != null)
            {
                itemList[i].transform.parent = itemList[i].HiddenParent;
            }
            itemList[i].Initialize();
        }
    }
    /// <summary>
    /// アイテムを取得する。ギミック付きなら解かせる。解けていれば入手
    /// </summary>
    /// <param name="itemObject"></param>
    /// <param name="isGetedAction"></param>
    public void ItemGetAction(ItemObject itemObject, UnityAction<bool> isGetedAction)
    {
        ItemData data = DataManager.Instance.GetItemData(itemObject.ItemKey);
        if(data != null)
        {
            if (itemObject.isGimmickItem && itemObject.gimmickEvent != null)
            {
                itemObject.gimmickEvent.ForceStartEvent(() =>
                {
                    NormalItemGetAction(data, itemObject, isGetedAction);
                });
            }
            else
            {
                NormalItemGetAction(data, itemObject, isGetedAction);
            }
        }
        else
        {
            Debug.LogError("取得したアイテムは存在しないか、キーが間違っています。 " + itemObject.ItemKey);
        }
    }

    private void NormalItemGetAction(ItemData data, ItemObject itemObject, UnityAction<bool> isGetedAction)
    {
        isGetedAction(true);
        DataManager.Instance.ItemGetAction(data, itemObject.OnGetedItemCallback);
        DisplayItem(data);
    }

    public void DisplayItem(ItemData _data)
    {
        switch (_data.type)
        {
            case ItemType.DoorKey:
                break;
            case ItemType.GetOnly:
                break;
            case ItemType.Useable:
                break;
            case ItemType.WatchOnly:
                break;
        }
        //Debug.Log("ItemLoad : " + ResourcesPath + _data.spriteName);
        currentGettingItemData = _data;
        //Texture2D tex = Resources.Load(ResourcesPath + _data.spriteName) as Texture2D;
        //Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        Sprite sprite = ResourceManager.LoadResourceSprite(ResourceManager.ItemResourcePath, _data.spriteName);//sprite;
        //itemNameText.text = _data.name;
        //itemContentText.text = _data.description;
        canvasUI.SetActive(true);
        instancedOneItemViewer = Instantiate(oneItemViewerPref, canvasUI.transform);
        instancedOneItemViewer.ViewItem(sprite, _data.name, _data.description);
        //oneItemViewerPref.SetActive(true);
        SoundManager.Instance.PlaySeWithKey("menuse_enter");
        StartCoroutine(WatchingItemUpdate(_data));
    }

    private IEnumerator WatchingItemUpdate(ItemData _data)
    {
        isLoopFlg = true;
        yield return null;//同じフレーム内で行うとアイテム取得の際のクリックで次の処理に入ってしまうため、1フレーム空ける
        while (isLoopFlg)
        {
            if (Input.GetMouseButtonDown(0))
            {
                switch (_data.type)
                {
                    case ItemType.WatchOnly:
                        watchDiaryManager.gameObject.SetActive(true);
                        //oneItemViewerPref.SetActive(false);
                        instancedOneItemViewer.CloseView();
                        watchDiaryManager.StartWatchDiary(_data);
                        break;
                    default:
                        FinishWatchItem();
                        
                        break;
                }
                yield break;
            }
            yield return null;
        }
    }

    /// <summary>
    /// アイテム閲覧モード終了
    /// </summary>
    /// <param name="isEnforcement">強制で終了する場合、コールバックは呼ばない</param>
    public void FinishWatchItem(bool isEnforcement = false)
    {
        canvasUI.SetActive(false);
        isLoopFlg = false;
        StopCoroutine(WatchingItemUpdate(null));
        //oneItemViewerPref.SetActive(false);
        if(instancedOneItemViewer != null) { instancedOneItemViewer.CloseView(); }
        watchDiaryManager.gameObject.SetActive(false);

        if (!isEnforcement && watchItemEventEndedCallback != null)
        {
            watchItemEventEndedCallback();
        }
    }
    public void FinishWatchingItemEnforcement()
    {
        watchDiaryManager.EndWatchingItem();
        FinishWatchItem();
    }
}
