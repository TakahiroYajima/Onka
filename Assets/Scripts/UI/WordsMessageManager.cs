using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 会話システムのようにメッセージを表示する
/// </summary>
public class WordsMessageManager : SingletonMonoBehaviour<WordsMessageManager>
{
    [SerializeField] private GameObject routeObj = null;
    [SerializeField] private Image displayImage = null;//中央に表示する画像
    [SerializeField] private Text messageText = null;//下部に表示するメッセージ（キャラのセリフ）
    [SerializeField] private Text clickText = null;//クリックを促すテキストのオブジェクト

    private List<string> messageList = new List<string>();

    //フェード時間情報
    private float fadeTime = 0f;
    private const float InitFaitTime = 0.35f;
    private float elapsedTime_Common = 0f;//経過時間計測用（汎用）
    public void SetFadeTime(float f)
    {
        fadeTime = f;
    }
    public void SetInitFadeTime()
    {
        fadeTime = InitFaitTime;
    }

    private bool isWaitForClick = false;//クリック待ちフラグ

    public bool isAction { get { return messageList.Count > 0; } }

    private bool isStartable = true;

    //表示位置情報
    private Vector2 initDisplayPosition = Vector2.zero;
    private DisplayPosition displayPosition = DisplayPosition.Under;
    public enum DisplayPosition
    {
        Under,
        Center
    }
    private Vector2 GetDisplayPosition(DisplayPosition _pos)
    {
        switch (_pos)
        {
            case DisplayPosition.Under: return initDisplayPosition;
            case DisplayPosition.Center: return Vector2.zero;
            default: return initDisplayPosition;
        }
    }
    public void SetDisplayPosition(DisplayPosition _pos)
    {
        displayPosition = _pos;
        switch (_pos)
        {
            case DisplayPosition.Under: messageText.GetComponent<RectTransform>().sizeDelta = initWHSize; break;
            case DisplayPosition.Center: messageText.GetComponent<RectTransform>().sizeDelta = centerPos_Size; break;
            default: messageText.GetComponent<RectTransform>().sizeDelta = initWHSize; break;
        }
        
    }
    //中央配置の時は画面全体にサイズを確保
    private Vector2 initWHSize = Vector2.zero;
    private Vector2 centerPos_Size = new Vector2(1800f, 900f);

    //テキストカラー情報
    private Color initColor = Color.white;
    private Color displayColor = Color.white;
    public void SetTextColor(Color _c)
    {
        displayColor = _c;
    }
    public void SetInitTextColor()
    {
        displayColor = initColor;
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        routeObj.SetActive(false);
        clickText.gameObject.SetActive(false);
        clickText.text = TextMaster.GetText("text_click_to_next");
        initDisplayPosition = messageText.GetComponent<RectTransform>().anchoredPosition;
        initWHSize = messageText.GetComponent<RectTransform>().sizeDelta;
        displayColor = initColor;
        fadeTime = InitFaitTime;
        InitImage();
    }

    private void InitImage()
    {
        displayImage.sprite = null;
        displayImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isAction) { return; }

        if (isWaitForClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isWaitForClick = false;
                StartCoroutine(DoNext());
            }
        }
    }

    public IEnumerator WordsAction(List<string> words, Sprite sprite = null)
    {
        messageText.GetComponent<RectTransform>().anchoredPosition = GetDisplayPosition(displayPosition);
        AddMessage(words, sprite);
        StartMessageShow();
        while (isAction) yield return null;
    }

    public void AddMessage(List<string> message, Sprite sprite = null)
    {
        messageList.AddRange(message);
        displayImage.sprite = sprite;
    }
    public void AddMessage(string message, Sprite sprite = null)
    {
        messageList.Add(message);
        displayImage.sprite = sprite;
    }

    public void StartMessageShow()
    {
        if (isStartable)
        {
            elapsedTime_Common = 0f;
            isWaitForClick = false;
            routeObj.SetActive(true);
            StartCoroutine(DoNext(true));
            isStartable = false;
        }
        else
        {
            Debug.Log("スタートできません");
        }
    }

    private IEnumerator DoNext(bool isFirst = false)
    {
        clickText.gameObject.SetActive(false);
        if (!isFirst)
        {
            yield return StartCoroutine(HideText());
            messageList.RemoveAt(0);
        }
        if (isAction)
        {
            yield return StartCoroutine(ShowTextAndSprite());
            isWaitForClick = true;
            clickText.gameObject.SetActive(true);
        }
        else
        {
            //メッセージが0なので終了
            isStartable = true;
            routeObj.SetActive(false);
            clickText.gameObject.SetActive(false);
            InitImage();
        }
    }

    private IEnumerator ShowTextAndSprite()
    {
        messageText.color = new Color(displayColor.r, displayColor.g, displayColor.b, messageText.color.a);
        Color c = messageText.color;
        c.a = 0f;
        messageText.color = c;
        elapsedTime_Common = 0f;

        if(displayImage.sprite != null && !displayImage.gameObject.activeSelf)
        {
            yield return StartCoroutine(ShowImage());
        }
        messageText.text = messageList[0];
        while (elapsedTime_Common < fadeTime)
        {
            c.a = elapsedTime_Common / fadeTime;
            messageText.color = c;
            elapsedTime_Common += Time.deltaTime;
            yield return null;
        }
        c.a = 1f;
        messageText.color = c;
    }

    private IEnumerator ShowImage()
    {
        if (!displayImage.gameObject.activeSelf)
        {
            yield return StartCoroutine(FadeManager.Instance.FadeAction(displayImage, FadeType.Out, fadeTime));
        }
    }

    private IEnumerator HideText()
    {
        Color c = messageText.color;
        c.a = 1f;
        messageText.color = c;
        elapsedTime_Common = 0f;

        while (elapsedTime_Common < fadeTime)
        {
            c.a = 1 - elapsedTime_Common / fadeTime;
            messageText.color = c;
            elapsedTime_Common += Time.deltaTime;
            yield return null;
        }
        c.a = 0f;
        messageText.color = c;
    }
}
