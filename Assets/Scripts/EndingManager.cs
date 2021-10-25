using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingManager : SingletonMonoBehaviour<EndingManager>
{
    [SerializeField] private AudioSource myAudioSource = null;
    [SerializeField] private GameObject panel = null;

    private bool actioning = false;

    private List<StaffRollTexts> staffRollTexts = new List<StaffRollTexts>();

    [SerializeField] private StaffRollItem staffRollItemPref = null;
    [SerializeField] private Transform staffRollParent = null;
    private List<StaffRollItem> instanceStaffRollList = new List<StaffRollItem>();

    private void Start()
    {
        panel.SetActive(false);
        
    }

    public void StartEnding()
    {
        panel.SetActive(true);
        if (!actioning)
        {
            actioning = true;
            staffRollTexts = new StaffRollTextCoreator().Create();
            myAudioSource.Play();
            StartCoroutine(EndingEvent());
        }
    }
    /// <summary>
    /// スタッフロールを垂れ流す（表示してはフェードアウトを繰り返す）
    /// </summary>
    /// <returns></returns>
    private IEnumerator EndingEvent()
    {
        for(int i = 0; i < staffRollTexts.Count; i++)
        {
            InstanceStaffRollItem(staffRollTexts[i]);
            yield return StartCoroutine(StaffRollItemFadeAction());
            yield return new WaitForSeconds(0.7f);
        }
        FadeManager.instance.FadeOut(FadeManager.FadeColorType.Black, 2f);
        yield return StartCoroutine(AudioFadeOut(3f));
        yield return new WaitForSeconds(2f);
         actioning = false;
        //終了
        //GameSceneManager.Instance.FinishEnding();
    }

    /// <summary>
    /// スタッフロールアイテム生成
    /// </summary>
    /// <param name="_staffRollTexts"></param>
    private void InstanceStaffRollItem(StaffRollTexts _staffRollTexts)
    {
        for(int i = 0; i < _staffRollTexts.oneScreenDisplayList.Count; i++)
        {
            //既に生成されていれば使いまわす
            if(i < instanceStaffRollList.Count)
            {
                instanceStaffRollList[i].gameObject.SetActive(true);
                instanceStaffRollList[i].Init(_staffRollTexts.oneScreenDisplayList[i].title, _staffRollTexts.oneScreenDisplayList[i].staffName);
            }
            else
            {
                instanceStaffRollList.Add(Instantiate(staffRollItemPref, staffRollParent));
                instanceStaffRollList[i].Init(_staffRollTexts.oneScreenDisplayList[i].title, _staffRollTexts.oneScreenDisplayList[i].staffName);
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
            for(int i = 0; i < instanceStaffRollList.Count; i++)
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

    private IEnumerator AudioFadeOut(float _in_out_time = 2f)
    {
        float volume = myAudioSource.volume;
        float initVolume = volume;
        float currentTime = initVolume;
        currentTime = 0f;
        while (volume > 0)
        {
            volume = initVolume - currentTime / _in_out_time;
            myAudioSource.volume = volume;
            currentTime += Time.deltaTime;
            yield return null;
        }
    }
}

public class StaffRollData
{
    public string title;
    public string staffName;

    public StaffRollData(string _title, string _name)
    {
        title = _title;
        staffName = _name;
    }
}

public class StaffRollTexts
{
    public List<StaffRollData> oneScreenDisplayList = new List<StaffRollData>();//一度に表示するリスト。一つだけの表示なら要素数は１。
    public StaffRollTexts(List<StaffRollData> staffRollDatas)
    {
        oneScreenDisplayList = staffRollDatas;
    }
}

public class StaffRollTextCoreator
{
    public List<StaffRollTexts> Create()
    {
        List<StaffRollTexts> returnList = new List<StaffRollTexts>()
        {
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("Death Islet", "")}),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("Appearance", "")}),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("Player","You")}),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("Syatyou","_K")}),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("Assets","")}),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("Asset Store", "https://assetstore.unity.com/") }),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("MusMus", "https://musmus.main.jp/")}),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("DOVA-SYNDROME", "https://dova-s.jp/")}),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("Maoudamashii", "https://maou.audio/") }),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("KMF", "http://www.kazamit.com/index.php?p=1") }),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("Komori", "https://taira-komori.jpn.org/horror01.html") }),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("Pocket Sound", "https://pocket-se.info/") }),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("The match makers", "http://osabisi.sakura.ne.jp/m2/") }),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("Other", "Yajima") }),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("Staff","")}),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("Planning","Yajima")}),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("Programing","Yajima")}),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("Design","Yajima")}),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("CreateTool","Unity")}),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("Presents","Yajima")}),
            new StaffRollTexts(new List<StaffRollData>(){ new StaffRollData("Thank you for playing.", "")}),
        };
        return returnList;
    }
}