using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FirstSplashSceneManager : MonoBehaviour
{
    [SerializeField] private UpdateFader faderObj = null;
    [SerializeField] private GameObject[] splashGroupObjcts = null;
    [SerializeField] private Text warningText = null;
    [SerializeField] private Text warningHeadText = null;

    private int currentSplashStateNum = 0;
    private bool isMoving = false;

    private bool isSceneChangeOK = false;

    // Start is called before the first frame update
    void Start()
    {
        SetTexts();
        currentSplashStateNum = -1;//DoNextでインクリメントするため、-1で初期化
        DoNext();
        //yield return SceneControlManager.Instance.InitializeLoadBeforeBeginGame();
        isSceneChangeOK = true;
    }

    private void SetTexts()
    {
        warningText.text = TextMaster.GetText("splash_warning");
        warningHeadText.text = TextMaster.GetText("splash_warning_head");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMoving) return;

        if (Input.GetMouseButtonDown(0))
        {
            
            DoNext();
        }
    }

    private void DoNext()
    {
        StopCoroutine(DoWaitAndOut());
        faderObj.FadeCansel();
        faderObj.InitColor();
        StartCoroutine(DoWaitAndNext());
    }

    private IEnumerator DoWaitAndNext()
    {
        isMoving = false;
        yield return new WaitForSeconds(0.3f);
        
        if(currentSplashStateNum >= 0)
        {
            splashGroupObjcts[currentSplashStateNum].SetActive(false);
        }
        currentSplashStateNum++;
        if (currentSplashStateNum < splashGroupObjcts.Length)
        {
            splashGroupObjcts[currentSplashStateNum].SetActive(true);
            faderObj.FadeStart(FadeType.In, 0.7f, () => { StartCoroutine(DoWaitAndOut());});
            isMoving = true;
        }
        else
        {
            faderObj.InitColor();//時間差で表示されないようにもう一度呼ぶ
            DoEnd();
        }
    }

    private IEnumerator DoWaitAndOut()
    {
        float t = 0f;
        while (t < 2.8f)
        {
            if (!isMoving) { break; }

            t += Time.deltaTime;
            yield return null;
        }
        
        if (isMoving)
        {
            faderObj.FadeStart(FadeType.Out, 0.7f, DoNext);
        }
    }

    private void DoEnd()
    {
        StartCoroutine(DoEndWaitInitSceneLoad());
    }

    private IEnumerator DoEndWaitInitSceneLoad()
    {
        while (!isSceneChangeOK)
        {
            yield return null;
        }
        isMoving = false;
        faderObj.SetActive(false);
        SceneControlManager.Instance.ChangeSceneAsyncWithLoading("Game", true, null, FadeManager.FadeColorType.None, FadeManager.FadeColorType.Black);
    }
}
