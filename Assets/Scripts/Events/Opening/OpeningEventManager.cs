﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;

public class OpeningEventManager : MonoBehaviour
{
    [SerializeField] private AudioClip bgm = null;

    private bool isPlaying = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartAction()
    {
        if (!isPlaying)
        {
            isPlaying = true;
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
            @"怨禍（おんか）

強い怨みを持つモノによってもたらされる禍（わざわい）。
憎悪に満ちるそのモノは、己の怨みに触れた者の全てを奪うまで呪い続ける。
禍から逃れる術はなく、ただその身が滅ぼされるのを待つのみ。
"
        }));
        yield return new WaitForSeconds(1f);
        WordsMessageManager.Instance.SetDisplayPosition(WordsMessageManager.DisplayPosition.Under);
        WordsMessageManager.Instance.SetInitTextColor();
        WordsMessageManager.Instance.SetInitFadeTime();
        yield return StartCoroutine(WordsMessageManager.Instance.WordsAction(new List<string>() {
            "あなたにはこの事件の調査をお願いします",
            "…この事件…ですか",
            "はい",
            "今までこれほどの事件どころか迷子の捜索すら扱ったことがありませんが…",
            "いえ、もう頼めるのがあなたぐらいしか残っていないのです",
            "…どういう事でしょうか",
            "今まで関わってきた調査員や探偵…",
            "その誰もが”関わりたくない”と言っているのです",
            "中には精神に異常をきたす者まで現れる始末",
            "…それで私に回ってきた…と",
            "ええ",
            "あなたを残り物のように扱っているのは申し訳ありません",
            "ですがこちらも手詰まりなのです",
            "…私でも解決できなかったら？",
            "この事件は迷宮入りします",
            "…",
            "どうかよろしくお願いします",
            "…",
        }));
        FadeManager.Instance.FadeOut(FadeManager.FadeColorType.Black, 3.5f, () =>
        {
            SceneControlManager.Instance.changeSceneMoveType = ChangeSceneMoveType.NewGame;
            SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Game", true, null, FadeManager.FadeColorType.None, FadeManager.FadeColorType.Black);
        });
    }

    
}
