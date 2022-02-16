using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharactorListViewer : MonoBehaviour
{
    [SerializeField] private bool deleteWhenClosed = false;
    [SerializeField] private CharactorListViewerOneItem itemPref = null;
    private List<CharactorListViewerOneItem> instancedList = new List<CharactorListViewerOneItem>();
    [SerializeField] private GameObject baseObject = null;
    [SerializeField] private Transform listParent = null;
    [SerializeField] private OneCharactorView oneCharactorViewerPref = null;
    [SerializeField] private Transform viewerParent = null;

    private CharactorDataList dataList = null;
    public UnityAction onViewed = null;
    public UnityAction onClosed = null;
    public void Initialize()
    {
        DestroyList();
    }

    public void ViewList(CharactorDataList _dataList)
    {
        dataList = _dataList;
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
        if (onClosed != null)
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

    private void InstanceList()
    {
        if (instancedList.Count > 0)
        {
            DestroyList();
        }

        for (int i = 0; i < dataList.charactorDataList.Count; i++)
        {
            string itemName = "？？？";
            instancedList.Add(Instantiate(itemPref, listParent));
            CharactorData data = dataList.charactorDataList[i];
            instancedList[i].Initialize(data, ()=> { DisplayContent(data); });
        }
    }
    private void DestroyList()
    {
        for (int i = 0; i < instancedList.Count; i++)
        {
            Destroy(instancedList[i].gameObject);
        }
        instancedList.Clear();
    }

    private void DisplayContent(CharactorData data)
    {
        OneCharactorView v = Instantiate(oneCharactorViewerPref, viewerParent);
        v.View(data, true, CloseOneItemView);
    }
    private void CloseOneItemView()
    {

    }
}
