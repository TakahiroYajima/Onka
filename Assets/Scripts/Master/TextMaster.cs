using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public partial class TextMaster
{
    //private static readonly Dictionary<string, TextLanguage> Text = new Dictionary<string, TextLanguage>()
    //{
    //    {"text_title", new TextLanguage("怨禍", "Onka") },
    //    {"text_new_game", new TextLanguage("初めから", "New game") },
    //    {"text_continue_game", new TextLanguage("続きから", "Continue") },
    //    {"text_quit_game", new TextLanguage("終了", "Quit") },
    //    {"text_bonus", new TextLanguage("おまけ", "Bonus") },
    //    {"text_twitter", new TextLanguage("作者Twitter", "Twitter") },
    //    {"text_select_save_data", new TextLanguage("データを選んでください", "Please select the save data.") },
    //    {"text_back", new TextLanguage("戻る", "Back") },
    //    {"text_select_save_data_item_title", new TextLanguage($"データ {0}", $"Data {0}") },
    //    {"text_select_save_data_item_no_data", new TextLanguage("データはありません", "No save data.") },
    //    {"text_language_setting", new TextLanguage("Language", "言語") },
    //    {"text_language_setting_title", new TextLanguage("言語設定（Language）", "言語設定（Language）") },

    //    {"text_loading", new TextLanguage("ロード中", "Loading") },

    //     {"text_menu_title", new TextLanguage("メニュー", "Menu") },

    //    {"text_game_over", new TextLanguage("死", "Death") },
    //    {"text_game_over_back_to_title", new TextLanguage("タイトルへ", "Back to title") },

    //    //キャラクター関連（名前などはCharacterDataの方に作る。本当は統一したかった…）
    //    {"text_name", new TextLanguage("名前", "Name") },
    //    {"text_age", new TextLanguage("年齢", "Age") },
    //    {"text_gender", new TextLanguage("性別", "Gender") },
    //    {"text_gender_man", new TextLanguage("男", "Man") },
    //    {"text_gender_woman", new TextLanguage("女", "Woman") },
    //    {"text_gender_unknown", new TextLanguage("不明", "???") },
    //};

    private static Dictionary<string, StringTable> MessageTables = new Dictionary<string, StringTable>();

    private static Language language = Language.Ja;
    public static Language CurrentLanguage { get { return language; } }

    public static void Initialize()
    {
        MessageTables.Clear();
        MessageTables.Add("Message", LocalizationSettings.StringDatabase.GetTable("Message"));
    }

    public static string GetText(string key)
    {
        if (!MessageTables.ContainsKey("Message")) 
        {
            Initialize();
        }
        return MessageTables["Message"].GetEntry(key).Value;
    }

    public static string HandOverMaster(string ja, string en)
    {
        switch (language)
        {
            case Language.Ja: return ja;
            case Language.En: return en;
            default: return en;
        };
    }

    public static List<string> HandOverMaster(List<string> ja, List<string> en)
    {
        switch (language)
        {
            case Language.Ja: return ja;
            case Language.En: return en;
            default: return en;
        };
    }

    public static void ChangeLanguage(Language change)
    {
        language = change;
        switch (language)
        {
            case Language.Ja:
                LocalizationSettings.SelectedLocale = Locale.CreateLocale("ja"); 
                break;
            case Language.En:
                LocalizationSettings.SelectedLocale = Locale.CreateLocale("en");
                break;
            default:
                LocalizationSettings.SelectedLocale = Locale.CreateLocale("ja");
                break;
        }
        Initialize();
    }
}

[System.Serializable]
public class TextLanguage
{
    public string ja;
    public string en;

    public TextLanguage(string ja, string en)
    {
        this.ja = ja;
        this.en = en;
    }
}

public enum Language
{
    Ja,
    En,
}