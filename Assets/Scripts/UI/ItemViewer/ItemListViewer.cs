using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemListViewer : MonoBehaviour
{
    [SerializeField] private ItemViewerOneItem itemPref = null;
    private List<ItemViewerOneItem> instancedList = new List<ItemViewerOneItem>();
    [SerializeField] private GameObject baseObject = null;
    [SerializeField] private Transform listParent = null;
    [SerializeField] private OneItemViewer oneItemViewerPref = null;
    [SerializeField] private Transform viewerParent = null;
    [SerializeField] private Sprite defaultSprite = null;

    public UnityAction onViewed = null;
    public UnityAction onClosed = null;

    public void Initialize()
    {
        DestroyList();
    }

    public void ViewList()
    {
        InstanceList();
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
        Destroy(gameObject);
    }
    
    private void InstanceList()
    {
        if(instancedList.Count > 0)
        {
            DestroyList();
        }
        
        for (int i = 0; i < DataManager.Instance.GetAllItemData().Count; i++)
        {
            Sprite itemSprite = defaultSprite;
            string itemName = "？？？";
            instancedList.Add(Instantiate(itemPref, listParent));
            ItemData itemData = DataManager.Instance.GetAllItemData()[i];
            if (itemData.geted)
            {
                itemSprite = ItemManager.Instance.LoadResourceSprite(itemData.spriteName);
                itemName = itemData.name;
            }
            else
            {
                itemSprite = defaultSprite;
                itemName = "？？？";
            }
            instancedList[i].Initialize(itemSprite, itemName, ()=> { if (itemData.geted) { DisplayItem(itemSprite, itemData); } });
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
