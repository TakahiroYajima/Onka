using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SoundSystem;

public class OpeningEventManager : MonoBehaviour
{
    [SerializeField] private AudioClip bgm = null;

    private bool isPlaying = false;
    private bool isSkip = false;
    private Action onComplete = null;

    private IEnumerator wordMessageCoroutine;

    public void StartAction(Action onComplete)
    {
        if (!isPlaying)
        {
            isPlaying = true;
            this.onComplete = onComplete;
            SoundManager.Instance.PlayBGMWithFadeIn(bgm, 2f, 0.1f);
            FadeManager.Instance.FadeIn(FadeManager.FadeColorType.Black, 2f, () =>
            {
                StartCoroutine(EventAction());
            });
        }
    }

    private IEnumerator EventAction()
    {
        yield return new WaitForSeconds(1f);
        if (CheckSkip()) { yield break; }
       
        WordsMessageManager.Instance.SetDisplayPosition(WordsMessageManager.DisplayPosition.Center);
        WordsMessageManager.Instance.SetTextColor(Color.red);
        WordsMessageManager.Instance.SetFadeTime(3.5f);
        if (CheckSkip()) { yield break; }
        wordMessageCoroutine = WordsMessageManager.Instance.WordsAction(new List<string>()
        {
            TextMaster.GetText("opening_onka")
        });
        yield return wordMessageCoroutine;

        if (CheckSkip()) { yield break; }
        yield return new WaitForSeconds(1f);
        if (CheckSkip()) { yield break; }
        WordsMessageManager.Instance.SetDisplayPosition(WordsMessageManager.DisplayPosition.Under);
        WordsMessageManager.Instance.SetInitTextColor();
        WordsMessageManager.Instance.SetInitFadeTime();
        wordMessageCoroutine = WordsMessageManager.Instance.WordsAction(TextMaster.GetOpeningConversationTexts().ToList());
        yield return wordMessageCoroutine;
        if (CheckSkip()) { yield break; }
        FinishOpening();
    }

    private void FinishOpening()
    {
        SoundManager.Instance.StopBGMWithFadeOut(bgm.name, 2f);
        FadeManager.Instance.FadeOut(FadeManager.FadeColorType.Black, 3.5f, () =>
        {
            SceneControlManager.Instance.changeSceneMoveType = ChangeSceneMoveType.NewGame;
            if (onComplete != null)
            {
                onComplete();
            }
            isPlaying = false;
            //SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Game", true, null, FadeManager.FadeColorType.None, FadeManager.FadeColorType.Black, false);
        });
    }

    public void Skip()
    {
        if (isPlaying)
        {
            isSkip = true;
            isPlaying = false;
            StopCoroutine(EventAction());
            FinishOpening();
        }
    }

    private bool CheckSkip()
    {
        if (isSkip)
        {
            if(wordMessageCoroutine != null)
            {
                StopCoroutine(wordMessageCoroutine);
                WordsMessageManager.Instance.ForceEnd();
            }
            return true;
        }
        return false;
    }
}
