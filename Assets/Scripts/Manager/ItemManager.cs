using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using SoundSystem;

public class ItemManager : SingletonMonoBehaviour<ItemManager>
{
    private List<ItemObject> itemList = new List<ItemObject>();
    [SerializeField] private GameObject itemsParent = null;
    [SerializeField] private GameObject watchItemBase = null;
    [SerializeField] private WatchDiaryManager watchDiaryManager = null;

    [SerializeField] private GameObject canvasUI = null;
    [SerializeField] private Image watchItemImage = null;
    [SerializeField] private Text itemNameText = null;
    [SerializeField] private Text itemContentText = null;

    public const string ResourcesPath = "Sprites/Items/";
    public UnityAction watchItemEventEndedCallback = null;
    public ItemData currentGettingItemData { get; private set; } = null;

    // Start is called before the first frame update
    void Start()
    {
        itemList = itemsParent.GetComponentsInChildren<ItemObject>().ToList();
        for(int i = 0; i < itemList.Count; i++)
        {
            itemList[i].Initialize();
        }
    }

    public void ItemGetAction(ItemObject itemObject)
    {
        ItemData data = DataManager.Instance.GetItemData(itemObject.ItemKey);
        if(data != null)
        {
            DataManager.Instance.ItemGetAction(data, itemObject.OnGetedItemCallback);
            DisplayItem(data);
        }
        else
        {
            Debug.LogError("取得したアイテムは存在しないか、キーが間違っています。 " + itemObject.ItemKey);
        }
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
        Debug.Log("ItemLoad : " + ResourcesPath + _data.spriteName);
        currentGettingItemData = _data;
        Texture2D tex = Resources.Load(ResourcesPath + _data.spriteName) as Texture2D;
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero); 
        watchItemImage.sprite = sprite;
        itemNameText.text = _data.name;
        itemContentText.text = _data.description;
        canvasUI.SetActive(true);
        watchItemBase.SetActive(true);
        SoundManager.Instance.PlaySeWithKey("menuse_enter");
        StartCoroutine(WatchingItemUpdate(_data));
    }

    private IEnumerator WatchingItemUpdate(ItemData _data)
    {
        yield return null;//同じフレーム内で行うとアイテム取得の際のクリックで次の処理に入ってしまうため、1フレーム空ける
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                switch (_data.type)
                {
                    case ItemType.WatchOnly:
                        watchDiaryManager.gameObject.SetActive(true);
                        watchItemBase.SetActive(false);
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

    public void FinishWatchItem()
    {
        canvasUI.SetActive(false);
        StopCoroutine(WatchingItemUpdate(null));
        watchItemBase.SetActive(false);
        watchDiaryManager.gameObject.SetActive(false);
        if (watchItemEventEndedCallback != null)
        {
            watchItemEventEndedCallback();
        }
    }
}
