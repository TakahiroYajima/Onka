using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EndingManager : SingletonMonoBehaviour<EndingManager>
{
    [SerializeField] private GameObject panel = null;

    private bool actioning = false;

    private List<StaffRollTexts> staffRollTexts = new List<StaffRollTexts>();

    [SerializeField] private StaffRollItem staffRollItemPref = null;
    [SerializeField] private Transform titleParent = null;
    [SerializeField] private Transform staffRollParent = null;
    private StaffRollItem instanceTitleStaffRollItem = null;
    private List<StaffRollItem> instanceStaffRollList = new List<StaffRollItem>();

    [SerializeField] private LayoutGroup parentLayoutGroup = null;
    [SerializeField] private LayoutGroup staffRollLayoutGroup = null;

    private void Start()
    {
        panel.SetActive(false);
        
    }

    public void StartEnding(UnityAction onComplete)
    {
        panel.SetActive(true);
        if (!actioning)
        {
            actioning = true;
            staffRollTexts = new StaffRollTextCoreator().Create();
            StartCoroutine(EndingEvent(onComplete));
        }
    }
    /// <summary>
    /// スタッフロールを垂れ流す（表示してはフェードアウトを繰り返す）
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndingEvent(UnityAction onComplete)
    {
        for(int i = 0; i < staffRollTexts.Count; i++)
        {
            InstanceStaffRollItem(staffRollTexts[i]);
            yield return new WaitForEndOfFrame();
            staffRollLayoutGroup.enabled = false;
            parentLayoutGroup.enabled = false;
            yield return null;
            staffRollLayoutGroup.enabled = true;
            //parentLayoutGroup.enabled = false;
            //yield return null;
            parentLayoutGroup.enabled = true;
            yield return StartCoroutine(StaffRollItemFadeAction());
            yield return new WaitForSeconds(0.69f);
        }
        
        yield return new WaitForSeconds(2f);
         actioning = false;
        //終了
        if(onComplete != null)
        {
            onComplete();
        }
    }

    /// <summary>
    /// スタッフロールアイテム生成
    /// </summary>
    /// <param name="_staffRollTexts"></param>
    private void InstanceStaffRollItem(StaffRollTexts _staffRollTexts)
    {
        if (!string.IsNullOrEmpty(_staffRollTexts.oneScreenDisplayData.title))
        {
            if (instanceTitleStaffRollItem == null)
            {
                instanceTitleStaffRollItem = Instantiate(staffRollItemPref, titleParent);
            }
            instanceTitleStaffRollItem.Init(_staffRollTexts.oneScreenDisplayData.title, TextAnchor.MiddleCenter);
        }
        if (_staffRollTexts.oneScreenDisplayData.staffNameList == null)
        {
            staffRollParent.gameObject.SetActive(false);
            //if (instanceStaffRollList.Count > 0)
            //{

            //}
            //else
            //{

            //}
        }
        else
        {
            staffRollParent.gameObject.SetActive(true);
            TextAnchor textAnchor = TextAnchor.MiddleCenter;
            if(_staffRollTexts.oneScreenDisplayData.staffNameList.Count > 1) { textAnchor = TextAnchor.MiddleLeft; }
            for (int i = 0; i < _staffRollTexts.oneScreenDisplayData.staffNameList.Count; i++)
            {
                //既に生成されていれば使いまわす
                if (i < instanceStaffRollList.Count)
                {
                    instanceStaffRollList[i].gameObject.SetActive(true);
                    instanceStaffRollList[i].Init(_staffRollTexts.oneScreenDisplayData.staffNameList[i], textAnchor);
                }
                else
                {
                    instanceStaffRollList.Add(Instantiate(staffRollItemPref, staffRollParent));
                    instanceStaffRollList[i].Init(_staffRollTexts.oneScreenDisplayData.staffNameList[i], textAnchor);
                }
            }
        }
    }
    /// <summary>
    /// スタッフロールのフェード
    /// </summary>
    /// <param name="_in_out_time"></param>
    /// <returns></returns>
    private IEnumerator StaffRollItemFadeAction(float _in_out_time = 0.3f)
    {
        Color color = new Color(0, 0, 0, 0);
        color.a = 0f;

        float alpha = 0f;
        float currentTime = 0f;
        while (alpha < 1)
        {
            alpha = currentTime / _in_out_time;
            color.a = alpha;
            if (instanceTitleStaffRollItem != null)
            {
                instanceTitleStaffRollItem.SetAlpha(alpha);
            }
            for (int i = 0; i < instanceStaffRollList.Count; i++)
            {
                if (instanceStaffRollList[i].gameObject.activeSelf)
                {
                    instanceStaffRollList[i].SetAlpha(alpha);
                }
            }
            currentTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(2.7f);
        currentTime = 0f;
        while (alpha > 0)
        {
            alpha = 1 - currentTime / _in_out_time;
            color.a = alpha;
            if (instanceTitleStaffRollItem != null)
            {
                instanceTitleStaffRollItem.SetAlpha(alpha);
            }
            for (int i = 0; i < instanceStaffRollList.Count; i++)
            {
                if (instanceStaffRollList[i].gameObject.activeSelf)
                {
                    instanceStaffRollList[i].SetAlpha(alpha);
                }
            }
            currentTime += Time.deltaTime;
            yield return null;
        }
        for (int i = 0; i < instanceStaffRollList.Count; i++)
        {
            instanceStaffRollList[i].gameObject.SetActive(false);
        }
    }
}

public class StaffRollData
{
    public string title;
    public List<string> staffNameList;

    public StaffRollData(string _title, List<string> _nameList)
    {
        title = _title;
        staffNameList = _nameList;
    }
}

public class StaffRollTexts
{
    public StaffRollData oneScreenDisplayData;//一度に表示するリスト。一つだけの表示なら要素数は１。
    public StaffRollTexts(StaffRollData _data)
    {
        oneScreenDisplayData = _data;
    }
}

public class StaffRollTextCoreator
{
    public List<StaffRollTexts> Create()
    {
        List<StaffRollTexts> returnList = new List<StaffRollTexts>()
        {
            new StaffRollTexts(new StaffRollData("怨 禍", null)),/*new StaffRollTexts(new StaffRollData("Planning", new List<string>() { "Sample1 Yajin", "Sample2 Yajin", "Sample3 Yajin" })),*/
            new StaffRollTexts(new StaffRollData("Staff", null)),
            new StaffRollTexts(new StaffRollData("Planning", new List<string>() { "Yajin" })),
            new StaffRollTexts(new StaffRollData("Design", new List<string>() { "AssetStore", "notargs", "Yajin" })),
            new StaffRollTexts(new StaffRollData("Sound", new List<string>() { "OtoLogic", "びたちー素材館", "DOVA-SYNDROME", "小森 平", "効果音ラボ",  "Yajin" })),
            new StaffRollTexts(new StaffRollData("Font", new List<string>() { "おたもん","暗黒工房","源界明朝","フォントダス" })),
            new StaffRollTexts(new StaffRollData("Programing", new List<string>() { "Yajin" })),
            new StaffRollTexts(new StaffRollData("Create Tool", new List<string>() { "Unity", "CharacterCreator3", "Photoshop", "Audacity" })),
            new StaffRollTexts(new StaffRollData("Presents", new List<string>() { "Yajin" })),
            new StaffRollTexts(new StaffRollData("Thank you for playing.", null)),
        };
        return returnList;
    }
}