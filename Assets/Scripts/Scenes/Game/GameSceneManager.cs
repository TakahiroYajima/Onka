using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
//using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.PostProcessing;
using SoundSystem;
using SoundDistance;
using Onka.Manager.Event;
using Onka.Manager.Menu;

/// <summary>
/// �Q�[���{�҂�Scene����
/// </summary>
/// <remark>���Ƃ��Ƃ�1��Scene���Ő��䂵�Ă������A�^�C�g���̓ǂݍ��݂�0�ɂ���������TitleManager���璼�ڐ��䂳���悤�ɕύX
public class GameSceneManager : SceneBase
{
    [SerializeField] private TitleManager titleManagerPrefab = null;
    private TitleManager titleManager = null;
    [SerializeField] private OpeningEventManager openingEventManagerPrefab = null;
    private OpeningEventManager openingEventManager = null;
    [SerializeField] private EndingSceneManager endingSceneManagerPrefab = null;
    private EndingSceneManager endingSceneManager = null;

    [field: SerializeField] public FieldManager fieldObject { get; private set; }
    private GameObject managerObject = null;

    private NavMeshDataInstance navMeshInstance;

    private PostProcessVolume fieldPostProcess = null;

    public bool IsOpening => openingEventManager != null;

    protected override void OnStartInitialize()
    {
        titleManager = Instantiate(titleManagerPrefab);
        titleManager.Initialize();
        SceneControlManager.Instance.FadeInScene(FadeManager.FadeColorType.Black, null, 1f);
        GameManager.Instance.OnSetUserSettings = OnSetUserSettings;

        fieldPostProcess = fieldObject.fieldPostProcess;
        SetDisplayBrightness(GameManager.Instance.GetSettingData().brightness);
    }

    public PostProcessVolume GetFieldPostProcess()
    {
        return fieldPostProcess;
    }

    private void DeleteTitle()
    {
        if (titleManager != null)
        {
            Destroy(titleManager.gameObject);
        }
    }

    private void DeleteOpening()
    {
        if (openingEventManager != null)
        {
            Destroy(openingEventManager.gameObject);
        }
    }

    public override void SceneStart()
    {
        base.Initialize();
        StartCoroutine(SetStart());
    }

    public override void SceneStartWithOpening()
    {
        base.Initialize();
        DeleteTitle();
        openingEventManager = Instantiate(openingEventManagerPrefab);
        openingEventManager.StartAction(() => { StartCoroutine(SetStart()); });
    }

    public void SkipOpening()
    {
        openingEventManager.Skip();
    }
    
    private IEnumerator SetStart()
    {
        var async = StartCoroutine(InitSceneSetUp());

        LoadingUIManager.Instance.SetActive(true);
        LoadingUIManager.Instance.SetProgress(0f);
        LoadingUIManager.Instance.SetMessage(TextMaster.GetText("text_loading"));
        yield return async;
        DeleteTitle();
        DeleteOpening();
        StartCoroutine(StartScene());
    }

    private IEnumerator InitSceneSetUp()
    {
        var await = new WaitForSeconds(0.14f);
        yield return await;
        
        //yield return LoadInScene();
        LoadingUIManager.Instance.SetProgress(0.25f);
        yield return LoadStage();
        yield return await;
        LoadingUIManager.Instance.SetProgress(0.5f);
        //Debug.Log("0.5");
        //�X�e�[�W����������Ă���
        yield return SetUpManager();
        yield return await;
        LoadingUIManager.Instance.SetProgress(0.75f);
        yield return ActorSetUp();
        yield return await;
        InGameUtil.GCCollect();
        StageManager.Instance.fieldObject.SetUp();
        LoadingUIManager.Instance.SetProgress(1f);
    }
    //private IEnumerator LoadInScene()
    //{
    //    //NavMesh�ǂݍ���
    //    var async = Resources.LoadAsync<NavMeshData>("Stages/NavMesh/NavMesh");
    //    yield return async;
    //    navMeshInstance = NavMesh.AddNavMeshData(async.asset as NavMeshData);
    //}
    private IEnumerator LoadStage()
    {
        if (managerObject == null)
        {
            var async = Resources.LoadAsync<GameObject>("Managers/Managers");
            yield return async;
            managerObject = Instantiate(async.asset as GameObject, this.transform);
        }
        InStageMenuManager.Instance.Initialize();
        StageManager.Instance.Initialize(fieldObject);
        yield return null;
    }
    private IEnumerator SetUpManager()
    {
        WanderingPointManager.Instance.Initialize();
        EventManager.Instance.Initialize();
        ItemManager.Instance.Initialize();
        yield return null;
    }
    private IEnumerator ActorSetUp()
    {
        SoundDistanceManager.Instance.Initialize();
        StageManager.Instance.ActorSetUp();
        SoundDistanceManager.Instance.SetUp(StageManager.Instance.Player.SoundListener, StageManager.Instance.Yukie.SoundEmitter);
        
        yield return null;
    }
    
    private IEnumerator StartScene()
    {
        SoundDistancePoint savePointSDP = StageManager.Instance.fieldObject.SavePointSDP;

        //�I�[�v�j���O�C�x���g�I���ς�
        if (EventManager.Instance.IsEventEnded("Event_Opnening"))
        {
            //�Z�[�u�n�_����ĊJ
            Debug.Log($"�Z�[�u�n�_����ĊJ ; {StageManager.Instance.Player.name}");
            StageManager.Instance.Player.transform.position = StageManager.Instance.fieldObject.restartPosition.transform.position;
            StageManager.Instance.Player.transform.rotation = Quaternion.Euler(0f, 180f, 0f);//�Z�[�u�|�C���g�̕�����������
        }
        //�I�[�v�j���O�C�x���g�����I���ς�
        if (!EventManager.Instance.IsEventEnded("Event_YukieHint"))
        {
            StageManager.Instance.InactiveYukieAndInitListenerPointID();
            SoundDistanceManager.Instance.Listener.SetCurrentPointID(savePointSDP.ID);
            SoundDistanceManager.Instance.Listener.SetNextTargetPointID(savePointSDP.ID);
        }
        //���ڎ��Ő�G�̉e������C�x���g�I���ς�
        else if (!EventManager.Instance.IsEventEnded("Event_YukieHintEnded"))
        {
            StageManager.Instance.InactiveYukieAndInitListenerPointID();
            SoundDistanceManager.Instance.Listener.SetCurrentPointID(savePointSDP.ID);
            SoundDistanceManager.Instance.Listener.SetNextTargetPointID(savePointSDP.ID);
        }
        //�L�b�`���܂ŋC�z��ǂ��Ă������A�܂���G�ɒǂ��������Ă��Ȃ����
        if(EventManager.Instance.IsEventEnded("Event_YukieHintEnded") && !EventManager.Instance.IsEventEnded("Event_FirstChasedFromYukie"))
        {
            StageManager.Instance.InactiveYukieAndInitListenerPointID();
            SoundDistanceManager.Instance.Listener.SetCurrentPointID(savePointSDP.ID);
            SoundDistanceManager.Instance.Listener.SetNextTargetPointID(savePointSDP.ID);
        }

        //�L�b�`���Ő�G����ǂ��������A������C�x���g�I���ς�
        if (EventManager.Instance.IsEventEnded("Event_FirstChasedFromYukie"))
        {
            //��G�������_���ɔz�u�i�v���C���[���猩���Ȃ��Ƃ���j
            var yukieInitInstancePoints = StageManager.Instance.fieldObject.YukieInitInstancePoints;
            var yukieInitWanderingPoints = StageManager.Instance.fieldObject.YukieInitWanderingPoints;
            int arrayNum = UnityEngine.Random.Range(0, yukieInitInstancePoints.Count - 1);
            Vector3 yukiePos = new Vector3(yukieInitInstancePoints[arrayNum].transform.position.x, StageManager.Instance.Yukie.transform.position.y, yukieInitInstancePoints[arrayNum].transform.position.z);
            StageManager.Instance.Yukie.transform.position = yukiePos;
            StageManager.Instance.Yukie.wanderingActor.SetWanderingID(yukieInitWanderingPoints[arrayNum].PointNum);
            StageManager.Instance.Yukie.ChangeState(EnemyState.Wandering);

            SoundDistanceManager.Instance.Emitter.SetPointID(yukieInitInstancePoints[0].ID);
            SoundDistanceManager.Instance.Listener.SetCurrentPointID(savePointSDP.ID);
            SoundDistanceManager.Instance.Listener.SetNextTargetPointID(savePointSDP.ID);
            SoundDistanceManager.Instance.Maker.SetVolume(0f);
            SoundDistanceManager.Instance.isActive = true;
            SoundDistanceManager.Instance.ForceInitCalc();
            SoundManager.Instance.PlayEnvironmentWithKey("ambient_in_house");
        }
        EventManager.Instance.isEnable = true;
        EventManager.Instance.InitProgressEach();

        SetMouseSensitivity(GameManager.Instance.GetSettingData().mouseSensitivity);

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.2f);
        FadeManager.Instance.FadeIn(FadeManager.FadeColorType.Black, 1f, ()=>
        {
            StageManager.Instance.Player.ChangeState(PlayerState.Free);
        });
        yield return new WaitForSeconds(0.1f);
        LoadingUIManager.Instance.SetInactiveWithFadeOut();
    }

    public void StartEnding()
    {
        if(endingSceneManager == null)
        {
            endingSceneManager = Instantiate(endingSceneManagerPrefab);
        }
        endingSceneManager.StartEnding();
    }

    private void OnSetUserSettings(float brightness, float mouseSensitivity)
    {

    }

    /// <summary>
    /// ��ʂ̖��邳��ݒ�iPostProcess�j
    /// </summary>
    /// <param name="brightness"></param>
    public void SetDisplayBrightness(float brightness)
    {
        float brightnessRatio = (brightness - SettingConstant.BrightnessMin) / (SettingConstant.BrightnessMax - SettingConstant.BrightnessMin);
        //Debug.Log($"brightnessRatio : {brightnessRatio}");
        //PostProcess
        float bloomIndencityMagnification = SettingConstant.BloomIntensityBright - SettingConstant.BloomIntensityDark;
        float bloomIndencityValue = SettingConstant.BloomIntensityDark + bloomIndencityMagnification * brightnessRatio;

        float bloomThresholdMagnification = SettingConstant.BloomThresholdBright - SettingConstant.BloomThresholdDark;//0�ɋ߂Â��قǖ��邭�Ȃ�̂ł���̓}�C�i�X�l�ɂȂ�
        float bloomThresholdValue = SettingConstant.BloomThresholdDark + bloomThresholdMagnification * brightnessRatio;

        float ambientIndencityMagnification = SettingConstant.AmbientIntensityBright - SettingConstant.AmbientIntensityDark;//0�ɋ߂Â��قǖ��邭�Ȃ�̂ł���̓}�C�i�X�l�ɂȂ�
        float ambientIndencityValue = SettingConstant.AmbientIntensityDark + ambientIndencityMagnification * brightnessRatio;

        Bloom bloom = fieldPostProcess.profile.GetSetting<Bloom>();
        if(bloom != null)
        {
            bloom.intensity.Override(bloomIndencityValue);
            bloom.threshold.Override(bloomThresholdValue);
        }

        AmbientOcclusion ambientOcclusion = fieldPostProcess.profile.GetSetting<AmbientOcclusion>();
        if(ambientOcclusion != null)
        {
            ambientOcclusion.intensity.Override(ambientIndencityValue);
        }

        //�{�Ғ��̓v���C���[�̃��C�g�ɉe��
        if (managerObject != null && StageManager.Instance.Player != null)
        {
            Light light = StageManager.Instance.Player.spotLight;
            float lightMagnification = SettingConstant.PlayerLightBright - SettingConstant.PlayerLightDark;
            float lightValue = SettingConstant.PlayerLightDark + lightMagnification * brightnessRatio;
            light.intensity = lightValue;
        }
    }

    public void SetMouseSensitivity(float mouseSensitivity)
    {
        if (managerObject != null && StageManager.Instance.Player != null)
        {
            StageManager.Instance.Player.FirstPersonAIO.mouseSensitivity = mouseSensitivity;
        }
    }
}

public class InGameUtil
{
    /// <summary>
    /// �J�[�\���𑀍�\�ɂ���
    /// </summary>
    public static void DoCursorFree()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    /// <summary>
    /// �J�[�\���𑀍�s�ɂ���
    /// </summary>
    public static void DoCursorLock()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void GCCollect()
    {
        GC.Collect();
    }
}