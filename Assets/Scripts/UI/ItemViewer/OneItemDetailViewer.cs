using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// おまけモードにて、アイテムの詳細（日記ならその前に中身）を見せる
/// </summary>
public class OneItemDetailViewer : MonoBehaviour
{
    [SerializeField] private GameObject baseObject = null;
    [SerializeField] private Image itemImage = null;
    [SerializeField] private Text nameText = null;
    [SerializeField] private Text descriptionText = null;
    [SerializeField] private TextButton backButton = null;
    [SerializeField] private TextButton watchButton = null;
    public UnityAction onClosed = null;

    [SerializeField] private WatchDiaryManager watchDiaryManager = null;

    private ItemData currentWatchingItemData = null;

    public void View(ItemData _data, UnityAction _onClosed = null)
    {
        SetTexts();
        currentWatchingItemData = _data;
        itemImage.sprite = ResourceManager.LoadResourceSprite(ResourceManager.ItemResourcePath, currentWatchingItemData.spriteName);
        nameText.text = currentWatchingItemData.Name;
        descriptionText.text = currentWatchingItemData.DescriptionDetail;

        onClosed = _onClosed;

        watchButton.gameObject.SetActive(_data.type == ItemType.WatchOnly);
        StartItemDetailView();
    }

    private void SetTexts()
    {
        watchButton.text.text = TextMaster.GetText("text_more_watch");
        backButton.text.text = TextMaster.GetText("text_back");
    }

    public void CloseView()
    {
        baseObject.SetActive(false);
        if (onClosed != null)
        {
            onClosed();
        }
        Destroy(gameObject);
    }

    private void StartItemDetailView()
    {
        baseObject.SetActive(true);

    }

    public void StartWatchDiary()
    {
        baseObject.SetActive(false);
        watchDiaryManager.gameObject.SetActive(true);
        watchDiaryManager.OnFinishWatched = OnEndedWatchDiary;
        watchDiaryManager.StartWatchDiary(currentWatchingItemData);
    }

    private void OnEndedWatchDiary()
    {
        watchDiaryManager.gameObject.SetActive(false);
        StartItemDetailView();
    }
}
