using System.Collections.Generic;
using System.Linq;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public partial class TextMaster
{
    private static Dictionary<string, StringTable> MessageTables = new Dictionary<string, StringTable>();

    private static Language language = Language.Ja;
    public static Language CurrentLanguage { get { return language; } }

    public static void Initialize()
    {
        MessageTables.Clear();
        MessageTables.Add("Message", LocalizationSettings.StringDatabase.GetTable("Message"));
        MessageTables.Add("StoryOpening", LocalizationSettings.StringDatabase.GetTable("StoryOpening"));
        MessageTables.Add("StoryEventAfterOutHouse", LocalizationSettings.StringDatabase.GetTable("StoryEventAfterOutHouse"));
        MessageTables.Add("StoryEnding", LocalizationSettings.StringDatabase.GetTable("StoryEnding"));
    }

    public static string GetText(string key)
    {
        if (!MessageTables.ContainsKey("Message")) 
        {
            Initialize();
        }
        return MessageTables["Message"].GetEntry(key).Value;
    }

    public static string[] GetOpeningConversationTexts()
    {
        return MessageTables["StoryOpening"].Values.OrderBy(v => v.KeyId).Select(v => v.Value).ToArray();
    }

    public static List<string> GetEndingEventConversationTexts()
    {
        return MessageTables["StoryEventAfterOutHouse"].Values.OrderBy(v => v.KeyId).Select(v => v.Value).ToList();
    }

    public static List<string> GetEndingSceneMessageTexts()
    {
        return MessageTables["StoryEnding"].Values.Where(v => v.Key.Contains("story_ending_narration")).OrderBy(v => v.KeyId).Select(v => v.Value).ToList();
    }

    public static List<string> GetEndingSceneConversationTexts()
    {
        return MessageTables["StoryEnding"].Values.Where(v => v.Key.Contains("story_ending_words")).OrderBy(v => v.KeyId).Select(v => v.Value).ToList();
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

public enum Language
{
    Ja,
    En,
}