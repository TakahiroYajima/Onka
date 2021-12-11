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

    private List<string> messageList = new List<string>();

    //private float currentWaitTime = 0f;
    private const float WaitTime = 0.35f;

    private float elapsedTime_Common = 0f;//経過時間計測用（汎用）

    private bool isWaitForClick = false;//クリック待ちフラグ

    public bool isAction { get { return messageList.Count > 0; } }

    private bool isStartable = true;

    // Start is called before the first frame update
    void Start()
    {
        routeObj.SetActive(false);
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
        if (!isFirst)
        {
            yield return StartCoroutine(HideText());
            messageList.RemoveAt(0);
        }
        if (isAction)
        {
            yield return StartCoroutine(ShowTextAndSprite());
            isWaitForClick = true;
        }
        else
        {
            //メッセージが0なので終了
            isStartable = true;
            routeObj.SetActive(false);
            InitImage();
        }
    }

    private IEnumerator ShowTextAndSprite()
    {
        Color c = messageText.color;
        c.a = 0f;
        messageText.color = c;
        elapsedTime_Common = 0f;

        if(displayImage.sprite != null && !displayImage.gameObject.activeSelf)
        {
            yield return StartCoroutine(ShowImage());
        }
        messageText.text = messageList[0];
        while (elapsedTime_Common < WaitTime)
        {
            c.a = elapsedTime_Common / WaitTime;
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
            yield return StartCoroutine(FadeManager.Instance.FadeAction(displayImage, FadeType.Out, WaitTime));
        }
    }

    private IEnumerator HideText()
    {
        Color c = messageText.color;
        c.a = 1f;
        messageText.color = c;
        elapsedTime_Common = 0f;

        while (elapsedTime_Common < WaitTime)
        {
            c.a = 1 - elapsedTime_Common / WaitTime;
            messageText.color = c;
            elapsedTime_Common += Time.deltaTime;
            yield return null;
        }
        c.a = 0f;
        messageText.color = c;
    }
}
