using System.Collections;
using System;
using UnityEngine;

public class EA_AfterGetKozoDiary3 : EventActorBase
{
    [SerializeField] private BoxCollider statusMoveCollider = null;
    [SerializeField] private CollisionEnterEvent statusMoveCollisionEnterEvent = null;
    [SerializeField] private BoxCollider kozoActiveCollider = null;
    [SerializeField] private CollisionEnterEvent kozoActiveCollisionEnterEvent = null;
    [SerializeField] private BoxCollider hatsuActiveCollider = null;
    [SerializeField] private CollisionEnterEvent hatsuActiveCollisionEnterEvent = null;
    [SerializeField] private BoxCollider eventEndCollider = null;
    [SerializeField] private CollisionEnterEvent eventEndCollisionEnterEvent = null;
    [SerializeField] private SoundPlayerObject statusSoundPlayer = null;

    [SerializeField] private Enemy_Kozo kozo = null;
    [SerializeField] private Enemy_Hatsu hatsu = null;
    [SerializeField] private HatsuThreatenAction threatenAction = null;

    public Event_AfterGetKozoDiary3 eventBase { private get; set; } = null;

    [SerializeField] private Canvas canvasObj = null;
    private CRT crt = null;
    private Camera worldCamera = null;

    protected override void Initialize()
    {
        crt = StageManager.Instance.Player.CameraObj.GetComponent<CRT>();
        worldCamera = StageManager.Instance.Player.CameraObj.GetComponent<Camera>();
        canvasObj.renderMode = RenderMode.ScreenSpaceCamera;
        canvasObj.worldCamera = worldCamera;
        canvasObj.gameObject.SetActive(false);

        statusMoveCollider.enabled = true;
        kozoActiveCollider.enabled = false;
        hatsuActiveCollider.enabled = false;
        eventEndCollider.enabled = false;
        parent.SetCanBeStarted(false);
        StageManager.Instance.Kozo = kozo;
        StageManager.Instance.Hatsu = hatsu;
        kozo.SetUp();
        kozo.gameObject.SetActive(false);
        hatsu.SetUp();
        hatsu.gameObject.SetActive(false);
        threatenAction.Initialize();
    }

    public override void EventStart()
    {
        statusSoundPlayer.PlaySE("se_lug");
        StartCoroutine(MoveStatus(eventBase.MoveObject , () =>
        {
            statusMoveCollider.enabled = false;
            statusMoveCollider.gameObject.SetActive(false);
            kozoActiveCollider.enabled = true;
        }));
    }
    public override void EventUpdate()
    {

    }
    public override void EventEnd()
    {
        StageManager.Instance.Kozo = null;
        StageManager.Instance.Hatsu = null;
    }

    private IEnumerator MoveStatus(Transform moveObject, Action onComplete)
    {
        float currentTime = 0f;
        while (currentTime < 0.5)
        {
            moveObject.Rotate(0, 45 * (Time.deltaTime / 0.5f), 0);
            currentTime += Time.deltaTime;
            yield return null;
        }
        moveObject.rotation = eventBase.moveObjectFinishedRotationEular;
        onComplete();
    }

    public void OnStatusMoveCollisionEnterEvent()
    {
        if (Utility.Instance.IsTagNameMatch(statusMoveCollisionEnterEvent.HitCollision.gameObject, Tags.Player))
        {
            parent.SetCanBeStarted(true);
            parent.InitiationContact();
        }
    }

    public void OnKozoActiveCollisionEnterEvent()
    {
        if (Utility.Instance.IsTagNameMatch(kozoActiveCollisionEnterEvent.HitCollision.gameObject, Tags.Player))
        {
            kozoActiveCollider.enabled = false;
            kozoActiveCollider.gameObject.SetActive(false);
            hatsuActiveCollider.enabled = true;
            eventEndCollider.enabled = true;

            kozo.gameObject.SetActive(true);
            kozo.ChangeState(EnemyState.ChasePlayer);
            crt.enabled = true;
            canvasObj.gameObject.SetActive(true);
        }
    }

    public void OnHatsuActiveCollisionEnterEvent()
    {
        if (Utility.Instance.IsTagNameMatch(hatsuActiveCollisionEnterEvent.HitCollision.gameObject, Tags.Player))
        {
            StartCoroutine(threatenAction.HatsuEvent(() =>
            {
                hatsuActiveCollider.enabled = false;
                hatsuActiveCollider.gameObject.SetActive(false);
                hatsu.gameObject.SetActive(true);
                hatsu.ChangeState(EnemyState.ChasePlayer);
                crt.enabled = true;
            }));
        }
    }

    public void OnEventEndCollisionEnterEvent()
    {
        if (Utility.Instance.IsTagNameMatch(eventEndCollisionEnterEvent.HitCollision.gameObject, Tags.Player))
        {
            kozo.ChangeState(EnemyState.Init);
            hatsu.ChangeState(EnemyState.Init);
            kozo.gameObject.SetActive(false);
            hatsu.gameObject.SetActive(false);
            crt.enabled = false;
            canvasObj.gameObject.SetActive(false);
            parent.EventClearContact();
        }
    }
}
