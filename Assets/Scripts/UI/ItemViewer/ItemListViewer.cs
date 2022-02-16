using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private Sprite defaultSprite = null;

    public UnityAction onViewed = null;
    public UnityAction onClosed = null;

    public enum ViewMode
    {
        Master,
        Playing,
    }

    public void Initialize()
    {
        DestroyList();
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
                itemName = itemData.name;
            }
            else
            {
                itemSprite = defaultSprite;
                itemName = "？？？";
            }
            if (itemData.geted)
            {
                instancedList[i].Initialize(itemSprite, itemName, () => { DisplayItem(itemSprite, itemData); });
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

    private void DisplayItem(Sprite _itemSprite, ItemData _itemData)
    {
        OneItemViewer v = Instantiate(oneItemViewerPref, viewerParent);
        v.ViewItem(_itemSprite, _itemData.name, _itemData.description, true, CloseOneItemView);
    }
    private void CloseOneItemView()
    {
        
    }
}
