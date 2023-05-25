using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class TextMaster
{
    private static readonly Dictionary<string, TextLanguage> MessageText = new Dictionary<string, TextLanguage>()
    {
        {"opening_onka", new TextLanguage(
@"怨禍（おんか）

強い怨みを持つモノによってもたらされる禍（わざわい）。
それは己の怨みに触れた者の全てを奪い、呪い続ける。
その禍から逃れる事はできない。
",
/*英語版の和訳
強い怨念を持つものがもたらす災厄。
その怨念に触れた者の生命を消し去るまで呪い続ける。
呪いから逃れることはできない。
 */
@"On-ka

A calamity brought about by a thing with a strong grudge.
It continues to curse until it wipes out the lives of those touched by its enmity.
There is no escape from the curse.
") },
    };

    public static string GetMessageText(string key)
    {
        if (MessageText.ContainsKey(key))
        {
            switch (language)
            {
                case Language.Ja: return MessageText[key].ja;
                case Language.En: return MessageText[key].en;
                default: return MessageText[key].en;
            };
        }
        else
        {
            Debug.LogError($"key is not found : {key}");
            return key;
        }
    }

    private static readonly TextLanguage[] OpeningConversationTexts = new TextLanguage[]
    {
        new TextLanguage("あなたにはこの事件の調査をお願いします",
            "I ask you to investigate this case."),
        new TextLanguage("…この事件…ですか",
            "...This incident...?"),
        new TextLanguage("はい",
            "Yes."),
        new TextLanguage("…今までこんな事件どころか迷子の捜索すら扱ったことがありませんが",
            "...I've never even handled a lost child search before, and you want me to handle this case?"),
        new TextLanguage("いえ、もう頼めるのがあなたぐらいしか残っていないのです",
            "You are the only person left we can ask."),
        new TextLanguage("…どういう事でしょうか",
            "...What do you mean?"),
        new TextLanguage("今まで関わってきた調査員や探偵",
            "Investigators and detectives who have been involved"),
        new TextLanguage("その誰もが”関わりたくない”と言っているのです",
            "They all fled as soon as they started working on the case."),
        new TextLanguage("中には精神に異常をきたす者まで現れる始末",
            "Some of them have even gone insane."),
        new TextLanguage("…それで私に回ってきた…と",
            "...and that's why you've turned to me?"),
        new TextLanguage("ええ",
            "Yes."),
        new TextLanguage("あなたを残り物のように扱っているのは申し訳ありません",
            "I am sorry I have treated you this way."),
        new TextLanguage("ですがこちらも手詰まりなのです",
            "But we have no other recourse."),
        new TextLanguage("…私でも解決できなかったら？",
            "...What happens if even I can't solve the problem?"),
        new TextLanguage("この事件は迷宮入りします",
            "The case will never be solved."),
        new TextLanguage("…",
            "..."),
        new TextLanguage("どうかよろしくお願いします",
            "We rely on you."),
        new TextLanguage("…",
            "..."),
    };
    
    public static string[] GetOpeningConversationTexts()
    {
        switch (language)
        {
            case Language.Ja: return OpeningConversationTexts.Select(v => v.ja).ToArray();
            case Language.En: return OpeningConversationTexts.Select(v => v.en).ToArray();
            default: return OpeningConversationTexts.Select(v => v.en).ToArray();
        };
    }

    private static readonly TextLanguage[] EndingEventConversationTexts = new TextLanguage[]
    {
       new TextLanguage("…",
            "..."),
        new TextLanguage("……",
            "......"),
        new TextLanguage("…悪かった",
            "...Forgive me!"),
        new TextLanguage("俺が悪かった…",
            "I was wrong...!!"),
        new TextLanguage("金が…",
            "money..."),
        new TextLanguage("金が必要だったんだよ…！！",
            "I needed money...!!"),
        new TextLanguage("助けてくれるって言うから話をしたのに…",
            "You said you'd help me, so I talked to you!!"),
        new TextLanguage("でもあんたらは焦る俺を笑うかのように幸せな姿を見せつけて！！",
            "But you guys just laugh at my impatience and show off your happy-go-lucky lives!!"),
        new TextLanguage("すぐに金が必要だったんだよ…",
            "I needed the money right away!!"),
        new TextLanguage("俺は…！！",
            "I am..."),
    };

    public static List<string> GetEndingEventConversationTexts()
    {
        switch (language)
        {
            case Language.Ja: return EndingEventConversationTexts.Select(v => v.ja).ToList();
            case Language.En: return EndingEventConversationTexts.Select(v => v.en).ToList();
            default: return EndingEventConversationTexts.Select(v => v.en).ToList();
        };
    }

    /*
    "- 富豪宅で男性惨殺！　榊原家の怨念の仕業か！？ -",
            "- 今日未明、榊原宅の庭で男性の遺体が発見された。 -",
            @"- 男性はこの付近に住む探偵 -----さんで
先日の榊原一家殺害事件の調査をしていた。 -",
            @"- 遺体の様子は人の手によって殺害されたのか
疑問に思うほど凄惨な状態だったという。 -",
            @"- この状況から「殺された一家の霊による事件」、
「榊原家の祟りだ」と言う者が現れ、混乱を招いている。 -",
            @"- 警察はこの事件の犯人が-----さんを殺害したとみて
捜査を進めていると発表したが、犯人の手がかりは一切掴めていないという。 -",
    */
    private static readonly TextLanguage[] EndingSceneMessageTexts = new TextLanguage[]
    {
       new TextLanguage(
"- 富豪宅で男性惨殺！　榊原家の怨念の仕業か！？ -",
"- Slaughter of a man at a wealthy man's house!　Was this the work of the Sakakibara family's grudge! -"),
        new TextLanguage(
"- 今日未明、榊原宅の庭で男性の遺体が発見された。 -",
"- Earlier today, a man's body was found in the garden of Sakakibara's house. -"),
        new TextLanguage(
@"- 男性はこの付近に住む探偵 -----さんで
先日の榊原一家殺害事件の調査をしていた。 -",
@"- The man is -----, a private detective who lives in the area.
He was investigating the recent murder of the Sakakibara family. -"),
        new TextLanguage(
@"- 遺体の様子は人の手によって殺害されたのか
疑問に思うほど凄惨な状態だったという。 -",
@"- The bodies were said to be in such a gruesome state that it was questionable whether they had been murdered by human hands. -"),
        new TextLanguage(
@"- この状況から「殺された一家の霊による事件」、
「榊原家の祟りだ」と言う者が現れ、混乱を招いている。 -",
@"- From this situation, the case is 'an incident caused by the spirits of a murdered family',
Some people claim that the Sakakibara family is haunted, leading to confusion. -"),
        new TextLanguage(
 @"- 警察はこの事件の犯人が-----さんを殺害したとみて
捜査を進めていると発表したが、犯人の手がかりは一切掴めていないという。 -",
@"- The police believe that the perpetrators of this incident murdered -----.
They announced that they are investigating, but have no clues as to who did it. -"),
    };

    public static List<string> GetEndingSceneMessageTexts()
    {
        switch (language)
        {
            case Language.Ja: return EndingSceneMessageTexts.Select(v => v.ja).ToList();
            case Language.En: return EndingSceneMessageTexts.Select(v => v.en).ToList();
            default: return EndingSceneMessageTexts.Select(v => v.en).ToList();
        };
    }

    //"ありえない", "夫が…あの人があんなに無惨に殺されたのに！！","幽霊の仕業なんてふざけた事を言って……！！","私がここで……この目で真実を……"
    private static readonly TextLanguage[] EndingSceneConversationTexts = new TextLanguage[]
    {
       new TextLanguage("ありえない！！",
            "Impossible!!"),
        new TextLanguage("夫が…あの人があんなに無惨に殺されたのに！！",
            "Even though her husband was brutally murdered!"),
        new TextLanguage("幽霊の仕業なんてふざけた事を言って…！！",
            "They told me it was the work of ghosts, and I was poked and prodded!!"),
        new TextLanguage("私がここで…この目で真実を…",
            "I'm here... to see the truth with my own eyes!!"),
    };

    public static List<string> GetEndingSceneConversationTexts()
    {
        switch (language)
        {
            case Language.Ja: return EndingSceneConversationTexts.Select(v => v.ja).ToList();
            case Language.En: return EndingSceneConversationTexts.Select(v => v.en).ToList();
            default: return EndingSceneConversationTexts.Select(v => v.en).ToList();
        };
    }

    //private Dictionary<string, TextLanguage> GetMessageData(MessageType type)
    //{
    //    switch (type)
    //    {
    //        case MessageType.Opening: return MessageText;
    //        //case MessageType.Bonus: return EndingEventConversationTexts;
    //    }
    //}

    public enum MessageType
    {
        Opening,
        EndingEvent,
        EndingCredit,
        Bonus,
    }
}