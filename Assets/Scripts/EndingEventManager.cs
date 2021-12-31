using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;

public class EndingEventManager : MonoBehaviour
{
    private MovingObject mainCamera = null;
    private CameraZoom zoom = null;
    [SerializeField] private MovingObject yukie = null;
    [SerializeField] private MovingObject shiori = null;
    [SerializeField] private WalkAnimObj kozo = null;
    [SerializeField] private WalkAnimObj hatsu = null;
    [SerializeField] private WalkAnimObj nobuyuki = null;
    [SerializeField] private WalkAnimObj azuha = null;
    [SerializeField] private WalkAnimObj yuzuha = null;

    [SerializeField] private GameObject turnTarget_DoorDir = null;//ドアの窓の方向
    [SerializeField] private GameObject turnTarget_AzuhaYuzuhaDir = null;//彩珠波と柚子羽がいる窓の方向
    [SerializeField] private GameObject turnTarget_yukieDir = null;//雪絵の方向

    [SerializeField] private AudioClip footStepClip = null;//ここでしか使わないのでデータでなくても良い方針で

    private CRT tvEffect = null;

    SoundPlayerObject yukieSound = null;

    private bool isPlaying = false;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="cameraObj"></param>
    public void Initialize(GameObject cameraObj)
    {
        mainCamera = cameraObj.GetComponent<MovingObject>();
        if(mainCamera == null) { mainCamera = cameraObj.AddComponent<MovingObject>(); }
        zoom = mainCamera.GetComponent<CameraZoom>();
        if(zoom == null) { zoom = cameraObj.AddComponent<CameraZoom>(); }
        zoom.Initialize(60f, 26f, 0.3f);
        tvEffect = cameraObj.GetComponent<CRT>();
        if(tvEffect == null) { tvEffect = cameraObj.AddComponent<CRT>(); }

        yukieSound = yukie.GetComponent<SoundPlayerObject>();

        kozo.gameObject.SetActive(false);
        hatsu.gameObject.SetActive(false);
        yukie.gameObject.SetActive(false);
        shiori.gameObject.SetActive(false);
        nobuyuki.gameObject.SetActive(false);
        azuha.gameObject.SetActive(false);
        yuzuha.gameObject.SetActive(false);

        tvEffect.enabled = false;
    }

    public void StartAction()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            StartCoroutine(EventAction());
        }
    }
    
    private IEnumerator EventAction()
    {
        InGameUtil.DoCursorLock();
        yield return null;
        //フェードイン
        FadeManager.Instance.FadeIn(FadeManager.FadeColorType.Black, 2f);
        yield return new WaitForSeconds(2f);

        //主人公の妻のセリフ
        yield return StartCoroutine(Words(new List<string>() { "ありえない", "あの人があんなに無惨に殺されるなんて！！" }));

        yield return StartCoroutine(StartupHatsu());
        //ドアの方を向く
        StartCoroutine(mainCamera.TurnAroundSmooth_Coroutine(turnTarget_DoorDir.transform.position, 12f));
        yield return new WaitForSeconds(0.7f);
        //ドアの窓の向こうに初（カメラズーム）
        yield return StartCoroutine(zoom.ZoomIn());
        //主人公の妻「！！」
        yield return StartCoroutine(Words(new List<string>() { "!!" }));
        
        StartCoroutine(mainCamera.TurnAroundSmooth_Coroutine(mainCamera.transform.position + Vector3.forward, 12f));

        //カメラズームを戻す
        yield return StartCoroutine(zoom.ZoomOut());
        
        yield return new WaitForSeconds(0.35f);
        //主人公の妻が後ずさり
        yield return StartCoroutine(SteppingBackFootStep());

        yield return StartCoroutine(StartupAzuYuzu());
        //笑い声に気づき右を向く
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(mainCamera.TurnAroundSmooth_Coroutine(turnTarget_AzuhaYuzuhaDir.transform.position, 12f));
        //部屋に彩珠波と柚子羽
        //主人公の妻「！！」
        yield return StartCoroutine(Words(new List<string>() { "!!" }));

        //左を向くと孝蔵と詩織がゆっくり迫ってくる。右からは信之
        Vector3 kozoPos = new Vector3(-3f, kozo.transform.position.y, 2f);
        StartupKozo(kozoPos);
        shiori.gameObject.SetActive(true);
        shiori.transform.position = new Vector3(-4f, shiori.transform.position.y, 1.6f);
        StartCoroutine(StartShioriUpdate());
        yield return StartCoroutine(mainCamera.TurnAroundSmooth_Coroutine(new Vector3(shiori.transform.position.x, mainCamera.transform.position.y, shiori.transform.position.z + 0.5f), 12f));

        yield return new WaitForSeconds(1f);

        Vector3 nobuyukiPos = new Vector3(3f, nobuyuki.transform.position.y, -1.6f);
        StartupNobuyuki(nobuyukiPos);
        nobuyukiPos.y = mainCamera.transform.position.y;
        yield return StartCoroutine(mainCamera.TurnAroundSmooth_Coroutine(nobuyukiPos, 12f));
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(mainCamera.TurnAroundSmooth_Coroutine(mainCamera.transform.position + Vector3.forward, 12f));
        //後ずさり
        yield return StartCoroutine(SteppingBackFootStep());

        yield return StartCoroutine(mainCamera.TurnAroundSmooth_Coroutine(mainCamera.transform.position + new Vector3(-1,0,1), 12f));
        yield return StartCoroutine(mainCamera.TurnAroundSmooth_Coroutine(mainCamera.transform.position + new Vector3(1, 0, 1), 12f));
        yield return StartCoroutine(mainCamera.TurnAroundSmooth_Coroutine(mainCamera.transform.position + new Vector3(0.03f, 0, 1), 12f));
        StartCoroutine(LastSteppingBack());

        //BGM鳴り出す
        SoundManager.Instance.PlayBGMWithKey("bgm_ending");
        
        yield return new WaitForSeconds(2.7f);
        //雪絵が迫る
        yukie.gameObject.SetActive(true);
        Vector3 yukieMoveTarget = new Vector3(mainCamera.transform.position.x, yukie.transform.position.y, mainCamera.transform.position.z - 1.3f);
        StartCoroutine(yukie.MoveWithTime(yukieMoveTarget, 1.1f, ()=>
        {
            StartCoroutine(StartEndrollEvent());
        }));

        yield return new WaitForSeconds(0.3f);
        yukieSound.PlaySoundLoop(0);
        //タイミングで振り返り
        StartCoroutine(mainCamera.TurnAroundToTargetAngle_Coroutine(turnTarget_yukieDir.transform.position, 10f));
        StartCoroutine(YukieSoundVolumeAction(0.35f));
    }
    /// <summary>
    /// 後ずさりの挙動。足音付き
    /// </summary>
    /// <param name="stepCount"></param>
    /// <param name="stepTime"></param>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    private IEnumerator SteppingBackFootStep(int stepCount = 2, float stepTime = 0.7f, float waitTime = 0.35f)
    {
        int count = 0;
        float currentTime = 0f;
        float footStepTime = stepTime * 0.7f;
        bool isFootSountPlayed = false;
        while(count < stepCount)
        {
            StartCoroutine(mainCamera.MoveWithTime(mainCamera.transform.position + Vector3.back, stepTime));
            isFootSountPlayed = false;
            currentTime = 0f;
            while (currentTime < stepTime)
            {
                if(currentTime >= footStepTime && !isFootSountPlayed)
                {
                    SoundManager.Instance.PlaySeOne(footStepClip);
                    isFootSountPlayed = true;
                }
                currentTime += Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(waitTime);
            count++;
        }
    }

    private IEnumerator LastSteppingBack()
    {
        yield return StartCoroutine(mainCamera.MoveWithTime(mainCamera.transform.position + Vector3.back, 0.35f));
        SoundManager.Instance.PlaySeOne(footStepClip);
        yield return new WaitForSeconds(0.7f);
        yield return StartCoroutine(mainCamera.MoveWithTime(mainCamera.transform.position + new Vector3(0,0,-0.5f), 0.35f));
        SoundManager.Instance.PlaySeOne(footStepClip);
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(mainCamera.MoveWithTime(mainCamera.transform.position + new Vector3(0, 0, -0.5f), 0.35f));
        SoundManager.Instance.PlaySeOne(footStepClip);
    }

    private IEnumerator StartEndrollEvent()
    {
        //テレビの砂嵐効果でエンディング
        tvEffect.enabled = true;
        yield return new WaitForSeconds(0.05f);
        yukieSound.StopSound();
        kozo.AnimOff();
        kozo.animatorEnabled = false;
        nobuyuki.AnimOff();
        nobuyuki.animatorEnabled = false;
        isKozoUpdate = false;
        isNobuyukiUpdate = false;
        isShioriUpdate = false;
        //スタッフロール
        EndingManager.Instance.StartEnding(FinishEventCallback);
    }

    private IEnumerator Words(List<string> words)
    {
        WordsMessageManager.Instance.AddMessage(words);
        WordsMessageManager.Instance.StartMessageShow();
        while (WordsMessageManager.Instance.isAction) yield return null;
    }

    private void StartupKozo(Vector3 kozoPos)
    {
        kozo.gameObject.SetActive(true);
        kozo.transform.position = kozoPos;
        StartCoroutine(KozoUpdate());
        kozo.AnimOn();
    }
    private IEnumerator StartupHatsu()
    {
        hatsu.gameObject.SetActive(true);
        yield return null;
        hatsu.animatorEnabled = false;
        hatsu.transform.LookAt(new Vector3(mainCamera.transform.position.x, hatsu.transform.position.y, mainCamera.transform.position.z));
    }
    private IEnumerator StartupAzuYuzu()
    {
        azuha.gameObject.SetActive(true);
        yuzuha.gameObject.SetActive(true);
        yield return null;
        azuha.animatorEnabled = false;
        azuha.transform.LookAt(new Vector3(mainCamera.transform.position.x, azuha.transform.position.y, mainCamera.transform.position.z));

        yuzuha.animatorEnabled = false;
        yuzuha.transform.LookAt(new Vector3(mainCamera.transform.position.x, yuzuha.transform.position.y, mainCamera.transform.position.z));
    }
    private void StartupNobuyuki(Vector3 nobuyukiPos)
    {
        nobuyuki.gameObject.SetActive(true);
        nobuyuki.transform.position = nobuyukiPos;
        StartCoroutine(NobuyukiUpdate());
        nobuyuki.AnimOn();
    }

    private IEnumerator YukieSoundVolumeAction(float toMaxTime)
    {
        float volume = 0f;
        float current = 0f;
        float powValue = 0f;
        while(volume < 1f)
        {
            yukieSound.SetVolume(volume);
            powValue += Time.deltaTime / toMaxTime;
            volume = powValue * powValue;
            yield return null;
        }
        yukieSound.SetVolume(1f);
    }

    private bool isKozoUpdate = false;
    private IEnumerator KozoUpdate()
    {
        isKozoUpdate = true;
        while (isKozoUpdate)
        {
            kozo.SetMoveDir((mainCamera.transform.position - kozo.transform.position));
            yield return null;
        }
    }
    private bool isNobuyukiUpdate = false;
    private IEnumerator NobuyukiUpdate()
    {
        isNobuyukiUpdate = true;
        while (isNobuyukiUpdate)
        {
            nobuyuki.SetMoveDir((mainCamera.transform.position - nobuyuki.transform.position));
            yield return null;
        }
    }

    private bool isShioriUpdate = false;
    private IEnumerator StartShioriUpdate()
    {
        isShioriUpdate = true;
        Vector3 dir;
        while (isShioriUpdate)
        {
            dir = mainCamera.transform.position - shiori.transform.position;
            shiori.MoveToTargetDir_Update(dir, 0.2f);
            yield return null;
        }
    }

    private void FinishEventCallback()
    {
        StartCoroutine(FinishEventCoroutine());
    }
    private IEnumerator FinishEventCoroutine()
    {
        FadeManager.Instance.FadeOut(FadeManager.FadeColorType.Black, 2f);
        SoundManager.Instance.StopBGMWithFadeOut(3f);
        yield return new WaitForSeconds(3f);
        isPlaying = false;
    }
}
