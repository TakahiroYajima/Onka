using System.Linq;
using System.Collections.Generic;

public class HintData
{
    //ヒントの出現キーとタイトル・詳細のセット。本当はマスタでテキスト出力させたかったけど時間ないのでこれで作る
    public static readonly HintSet[] HintTable = new HintSet[]
    {
        new("diary_shiori_3", false, "hint_bathroom_key", "hint_bathroom_key_description"),
        new("key_azuha_yuzuha", false, "hint_in_childrenroom", "hint_in_childrenroom_description"),
        new("key_maidroom", true, "hint_keylock_shiori", "hint_keylock_shiori_description"),
        new("key_keymanagementcabinet", true, "hint_keylock_garage", "hint_keylock_garage_description"),
        new("small_memo1", false, "hint_smallnote_1", "hint_smallnote_1_description"),
        new("small_memo1", false, "hint_smallnote_1_2", "hint_smallnote_1_2_description"),
        new("small_memo2", false, "hint_smallnote_2", "hint_smallnote_2_description"),
        new("small_memo2", false, "hint_smallnote_2_2", "hint_smallnote_2_2_description"),
        new("small_memo3", false, "hint_smallnote_3", "hint_smallnote_3_description"),
        new("small_memo3", false, "hint_smallnote_3_2", "hint_smallnote_3_2_description"),
        new("key_1fhiddenroom", true, "hint_numberpadlock", "hint_numberpadlock_description"),
        new("key_1fhiddenroom", true, "hint_numberpadlock_2", "hint_numberpadlock_description_2"),
        new("inputkeypadlock_1", false, "hint_underground", "hint_underground_description"),
    };

    public static List<HintSet> GetDisplayHintList(IReadOnlyList<ItemData> itemList)
    {
        var list = new List<HintSet>();
        for(int i = 0; i < HintTable.Length; ++i)
        {
            var hint = HintTable[i];
            var itemData = itemList.FirstOrDefault(v => v.key == hint.displayGetedItemKey);
            if (hint.isUse)
            {
                if (itemData.used)
                {
                    list.Add(hint);
                }
            }
            else if(itemData.geted)
            {
                list.Add(hint);
            }
        }
        return list;
    }
}

public class HintSet
{
    public string displayGetedItemKey;
    public bool isUse;
    public string titleKey;
    public string descriptionKey;
    public HintSet(string displayKey, bool isUse, string titleKey, string descriptionKey)
    {
        this.displayGetedItemKey = displayKey;
        this.isUse = isUse;
        this.titleKey = titleKey;
        this.descriptionKey = descriptionKey;
    }
}
