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
    [SerializeField] private GameObject restartPosition = null;//�Z�[�u�n�_����ĊJ���鎞�̏ꏊ
    [SerializeField] private SoundDistancePoint savePointSDP = null;
    [SerializeField] private List<WanderingPoint> yukieInitWanderingPoints = new List<WanderingPoint>();
    [SerializeField] private List<SoundDistancePoint> yukieInitInstancePoints = new List<SoundDistancePoint>();
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Initialize();
        InStageMenuManager.Instance.Initialize();
        WanderingPointManager.Instance.Initialize();
        StageManager.Instance.Initialize();
        EventManager.Instance.Initialize();
        ItemManager.Instance.Initialize();
        SoundDistanceManager.Instance.Initialize();

        InGameUtil.GCCollect();

        StartScene();
    }
    
    private void StartScene()
    {
        //�I�[�v�j���O�C�x���g�I���ς�
        if (EventManager.Instance.IsEventEnded("Event_Opnening"))
        {
            //�Z�[�u�n�_����ĊJ
            StageManager.Instance.Player.transform.position = restartPosition.transform.position;
            StageManager.Instance.Player.transform.rotation = Quaternion.Euler(0f, 90f, 0f);//�Z�[�u�|�C���g�̕�����������
            StageManager.Instance.Player.ChangeState(PlayerState.Free);
        }
        //�I�[�v�j���O�C�x���g�����I���ς�
        if (!EventManager.Instance.IsEventEnded("Event_YukieHint"))
        {
            StageManager.Instance.InactiveYukie();
        }
        //���ڎ��Ő�G�̉e������C�x���g�I���ς�
        else if (!EventManager.Instance.IsEventEnded("Event_YukieHintEnded"))
        {
            StageManager.Instance.InactiveYukie();
        }
        //�L�b�`���܂ŋC�z��ǂ��Ă������A�܂���G�ɒǂ��������Ă��Ȃ����
        if(EventManager.Instance.IsEventEnded("Event_YukieHintEnded") && !EventManager.Instance.IsEventEnded("Event_FirstChasedFromYukie"))
        {
            StageManager.Instance.InactiveYukie();
        }

        //�L�b�`���Ő�G����ǂ��������A������C�x���g�I���ς�
        if (EventManager.Instance.IsEventEnded("Event_FirstChasedFromYukie"))
        {
            //��G�������_���ɔz�u�i�v���C���[���猩���Ȃ��Ƃ���j
            int arrayNum = UnityEngine.Random.Range(0, yukieInitInstancePoints.Count - 1);
            Vector3 yukiePos = new Vector3(yukieInitInstancePoints[arrayNum].transform.position.x, StageManager.Instance.Yukie.transform.position.y, yukieInitInstancePoints[arrayNum].transform.position.z);
            StageManager.Instance.Yukie.transform.position = yukiePos;
            StageManager.Instance.Yukie.wanderingActor.SetWanderingID(yukieInitWanderingPoints[arrayNum].PointNum);
            SoundManager.Instance.PlayEnvironmentWithKey("ambient_in_house");
        }

        EventManager.Instance.ProgressEvent();
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