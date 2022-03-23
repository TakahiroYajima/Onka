using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using SoundSystem;

public class WatchDiaryManager : MonoBehaviour
{
    private bool isUpdateMoving = false;

    private ItemData diaryData = null;
    [SerializeField] private Text pageNumText = null;
    [SerializeField] private Text contentText = null;

    [SerializeField] private Font[] fonts;

    public UnityAction OnFinishWatched = null;

    public void StartWatchDiary(ItemData _diaryData)
    {
        if (!isUpdateMoving)
        {
            isUpdateMoving = true;
            diaryData = _diaryData;
            StartCoroutine(WatchingItemUpdate());
        }
    }

    private IEnumerator WatchingItemUpdate()
    {
        int pageNum = 0;
        DoOpenPage(pageNum);
        yield return null;//同じフレーム内で行うとアイテム取得の際のクリックで次の処理に入ってしまうため、1フレーム空ける
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                pageNum++;
                if (pageNum >= diaryData.fileItem.content.Count)
                {
                    
                    EndWatchingItem();
                    yield break;
                }
                else
                {
                    DoOpenPage(pageNum);
                }
            }
            yield return null;
        }
    }

    private void DoOpenPage(int page)
    {
        //Debug.Log("DoOpenPage : " + page);
        pageNumText.text = $"{page + 1} / {diaryData.fileItem.content.Count}";
        contentText.text = diaryData.fileItem.content[page];
        contentText.font = fonts[(int)diaryData.fileItem.fontType];
        contentText.color = diaryData.fileItem.GetTextColor();
        SoundManager.Instance.PlaySeWithKey("menuse_book_page");
    }

    public void EndWatchingItem()
    {
        StopCoroutine(WatchingItemUpdate());
        isUpdateMoving = false;
        if (OnFinishWatched != null)
        {
            OnFinishWatched();
        }
    }
}
