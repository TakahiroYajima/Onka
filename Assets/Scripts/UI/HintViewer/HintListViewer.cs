using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using SoundSystem;

public class HintListViewer : MonoBehaviour
{
    [SerializeField]
    private bool deleteWhenClosed = true;
    [SerializeField]
    private GameObject baseObject;
    [SerializeField]
    private HintListItem listItemPrefab;
    [SerializeField]
    private Transform listParent;
    [SerializeField]
    private Button backButton;

    private List<HintListItem> instancedList = new();
    private IReadOnlyList<ItemData> itemList;

    public Action onClosed = null;

    public void ViewList(IReadOnlyList<ItemData> itemList)
    {
        this.itemList = itemList;
        DestroyList();
        InstanceOrRefreshList();
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(CloseList);
        backButton.onClick.AddListener(() => SoundManager.Instance.PlaySeWithKey("menuse_click"));
        baseObject.SetActive(true);
    }

    public void CloseList()
    {
        baseObject.SetActive(false);
        onClosed?.Invoke();
        if (deleteWhenClosed)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void InstanceOrRefreshList()
    {
        var dataList = HintData.GetDisplayHintList(itemList);
        for (int i = 0; i < dataList.Count; ++i)
        {
            var data = dataList[i];
            var param = new HintListItem.Parameter()
            {
                HintSet = data,
                OnClick = () => { OnClickItem(data); }
            };
            //var instance = instancedList.FirstOrDefault(v => v.HintSet.displayGetedItemKey == data.displayGetedItemKey);
            //HintListItem hintListItem;
            //if (instance == null)
            //{
            //    hintListItem = Instantiate(listItemPrefab, listParent);
            //    hintListItem.transform.SetSiblingIndex(i);
            //    if (i < instancedList.Count)
            //    {
            //        instancedList.Insert(i, hintListItem);
            //    }
            //    else
            //    {
            //        instancedList.Add(hintListItem);
            //    }
            //}
            //else
            //{
            //    hintListItem = instance;
            //}
            var hintListItem = Instantiate(listItemPrefab, listParent);
            hintListItem.Setup(param);
            instancedList.Add(hintListItem);
        }
    }

    public void DestroyList()
    {
        for (int i = 0; i < instancedList.Count; i++)
        {
            Destroy(instancedList[i].gameObject);
        }
        instancedList.Clear();
    }

    private void OnClickItem(HintSet hintSet)
    {
        DialogManager.Instance.OpenTemplateDialog(TextMaster.GetHint(hintSet.descriptionKey), TempDialogType.Back, (result) => { });
    }
}
