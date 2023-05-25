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
    private Action onComplete = null;

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
        WordsMessageManager.Instance.SetDisplayPosition(WordsMessageManager.DisplayPosition.Center);
        WordsMessageManager.Instance.SetTextColor(Color.red);
        WordsMessageManager.Instance.SetFadeTime(3.5f);
        yield return StartCoroutine(WordsMessageManager.Instance.WordsAction(new List<string>()
        {
            TextMaster.GetMessageText("opening_onka") 
        }));
        yield return new WaitForSeconds(1f);
        WordsMessageManager.Instance.SetDisplayPosition(WordsMessageManager.DisplayPosition.Under);
        WordsMessageManager.Instance.SetInitTextColor();
        WordsMessageManager.Instance.SetInitFadeTime();
        yield return StartCoroutine(WordsMessageManager.Instance.WordsAction(TextMaster.GetOpeningConversationTexts().ToList()));
        FadeManager.Instance.FadeOut(FadeManager.FadeColorType.Black, 3.5f, () =>
        {
            SceneControlManager.Instance.changeSceneMoveType = ChangeSceneMoveType.NewGame;
            if(onComplete != null)
            {
                onComplete();
            }
            //SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Game", true, null, FadeManager.FadeColorType.None, FadeManager.FadeColorType.Black, false);
        });
    }

    
}
