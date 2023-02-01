using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using SoundSystem;
using SoundDistance;
using Onka.Manager.Event;
using Onka.Manager.Menu;

public class GameSceneManager : SceneBase
{
    //[SerializeField] private GameObject restartPosition = null;//�Z�[�u�n�_����ĊJ���鎞�̏ꏊ
    //[SerializeField] private SoundDistancePoint savePointSDP = null;
    //[SerializeField] private List<WanderingPoint> yukieInitWanderingPoints = new List<WanderingPoint>();
    //[SerializeField] private List<SoundDistancePoint> yukieInitInstancePoints = new List<SoundDistancePoint>();
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Initialize();
        InStageMenuManager.Instance.Initialize();
        StageManager.Instance.Initialize();

        //�X�e�[�W����������Ă���
        WanderingPointManager.Instance.Initialize();
        EventManager.Instance.Initialize();
        ItemManager.Instance.Initialize();

        StageManager.Instance.ActorSetUp();
        SoundDistanceManager.Instance.SetUp(StageManager.Instance.Player.SoundListener, StageManager.Instance.Yukie.SoundEmitter);
        SoundDistanceManager.Instance.Initialize();


        StartCoroutine(InitSceneSetUp());
    }

    private IEnumerator InitSceneSetUp()
    {
        StageManager.Instance.fieldObject.SetUp();
        InGameUtil.GCCollect();
        yield return null;
        yield return null;
        StartScene();
    }
    
    private void StartScene()
    {
        SoundDistancePoint savePointSDP = StageManager.Instance.fieldObject.SavePointSDP;

        //�I�[�v�j���O�C�x���g�I���ς�
        if (EventManager.Instance.IsEventEnded("Event_Opnening"))
        {
            //�Z�[�u�n�_����ĊJ
            StageManager.Instance.Player.transform.position = StageManager.Instance.fieldObject.restartPosition.transform.position;
            StageManager.Instance.Player.transform.rotation = Quaternion.Euler(0f, 180f, 0f);//�Z�[�u�|�C���g�̕�����������
            StageManager.Instance.Player.ChangeState(PlayerState.Free);
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
        Debug.Log("EventManager.Instance.InitProgressEach");
        EventManager.Instance.InitProgressEach();
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