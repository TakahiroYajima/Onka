using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Onka.Manager.Data;

public class ItemListViewer : MonoBehaviour
{
    [SerializeField] private bool deleteWhenClosed = true;
    [SerializeField] private ItemViewerOneItem itemPref = null;
    private List<ItemViewerOneItem> instancedList = new List<ItemViewerOneItem>();
    [SerializeField] private GameObject baseObject = null;
    [SerializeField] private Transform listParent = null;
    [SerializeField] private OneItemViewer oneItemViewerPref = null;
    [SerializeField] private Transform viewerParent = null;
    [SerializeField] private OneItemDetailViewer oneItemDetailViewerPref = null;
    [SerializeField] private Transform detailViewerParent = null;
    [SerializeField] private Sprite defaultSprite = null;

    [SerializeField] private Text backButtonText = null;

    [SerializeField] private WatchDiaryManager watchDiaryManager = null;

    public UnityAction onViewed = null;
    public UnityAction onClosed = null;

    private OneItemViewer currentInstanceViewer = null;
    private OneItemDetailViewer currentInstanceDetailViewer = null;
    private ItemData currentDisplayItem = null;

    public enum ViewMode
    {
        Master,//おまけモード。手に入れたものはすべて表示
        Playing,//プレイ中。プレイ中のデータ内で手に入れたもののみ表示
    }

    public void Initialize()
    {
        SetTexts();
        DestroyList();
    }

    private void SetTexts()
    {
        backButtonText.text = TextMaster.GetText("text_back");
    }

    public void ViewList(ViewMode viewMode)
    {
        InstanceList(viewMode);
        baseObject.SetActive(true);
        if (onViewed != null)
        {
            onViewed();
        }
    }
    public void CloseList()
    {
        baseObject.SetActive(false);
        if(onClosed != null)
        {
            onClosed();
        }
        if (deleteWhenClosed)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    private void InstanceList(ViewMode viewMode)
    {
        if(instancedList.Count > 0)
        {
            DestroyList();
        }
        IReadOnlyList<ItemData> itemList = null;
        if(viewMode == ViewMode.Master) { itemList = DataManager.Instance.GetGeneralItemDataList(); }
        else { itemList = DataManager.Instance.GetAllItemData(); }
        for (int i = 0; i < itemList.Count; i++)
        {
            Sprite itemSprite = defaultSprite;
            string itemName = "？？？";
            instancedList.Add(Instantiate(itemPref, listParent));
            ItemData itemData = itemList[i];
            if (itemData.geted)
            {
                itemSprite = ResourceManager.LoadResourceSprite(ResourceManager.ItemResourcePath, itemData.spriteName);
                itemName = itemData.Name;
            }
            else
            {
                itemSprite = defaultSprite;
                itemName = "？？？";
            }
            if (itemData.geted)
            {
                instancedList[i].Initialize(itemSprite, itemName, () => { DisplayItem(itemSprite, itemData, viewMode); });
            }
            else
            {
                instancedList[i].Initialize(itemSprite, itemName, null);
            }
                
        }
    }
    private void DestroyList()
    {
        for(int i = 0; i < instancedList.Count; i++)
        {
            Destroy(instancedList[i].gameObject);
        }
        instancedList.Clear();
    }

    private void DisplayItem(Sprite _itemSprite, ItemData _itemData, ViewMode viewMode)
    {
        currentDisplayItem = _itemData;
        currentInstanceViewer = Instantiate(oneItemViewerPref, viewerParent);
        if (viewMode == ViewMode.Master)
        {
            currentInstanceViewer.ViewItem(_itemSprite, _itemData.Name, _itemData.Description, true, CloseOneItemView, OnPressItemDetailButtonInViewer);
        }
        else
        {
            //難易度：普通なら日記やノートを見られる
            if (GameManager.Instance.GetSettingData().difficulty == Difficulty.Normal && _itemData.type == ItemType.WatchOnly)
            {
                currentInstanceViewer.ViewItem(_itemSprite, _itemData.Name, _itemData.Description, true, CloseOneItemView, ()=> StartWatchDiary(_itemData));
            }
            else
            {
                currentInstanceViewer.ViewItem(_itemSprite, _itemData.Name, _itemData.Description, true, CloseOneItemView, null);
            }
        }
    }
    private void CloseOneItemView()
    {
        currentInstanceViewer = null;
    }

    public void StartWatchDiary(ItemData currentWatchingItemData)
    {
        currentInstanceViewer.gameObject.SetActive(false);
        watchDiaryManager.gameObject.SetActive(true);
        watchDiaryManager.OnFinishWatched = OnEndedWatchDiary;
        watchDiaryManager.StartWatchDiary(currentWatchingItemData);
    }

    private void OnEndedWatchDiary()
    {
        currentInstanceViewer.gameObject.SetActive(true);
        watchDiaryManager.gameObject.SetActive(false);
    }

    private void OnPressItemDetailButtonInViewer()
    {
        currentInstanceDetailViewer = Instantiate(oneItemDetailViewerPref, detailViewerParent);
        currentInstanceDetailViewer.View(currentDisplayItem, OnCloseOneItemDetailView);
        if(currentInstanceViewer != null)
        {
            currentInstanceViewer.gameObject.SetActive(false);
        }
    }
    private void OnCloseOneItemDetailView()
    {
        currentInstanceDetailViewer = null;
        if (currentInstanceViewer != null)
        {
            currentInstanceViewer.gameObject.SetActive(true);
        }
    }
}
