using UnityEngine;

public class EndingSceneManager : MonoBehaviour
{
    [SerializeField] private EndingEventManager endingEventManager = null;
    [SerializeField] private GameObject camera = null;
    
    public void StartEnding()
    {
        StageManager.Instance.Player.gameObject.SetActive(false);
        Onka.Manager.Data.DataManager.Instance.SetCurrentSceneUseSound(SceneType.Ending);
        endingEventManager.Initialize(camera);
        endingEventManager.StartAction(EndingEventType.EndingScene);
    }
}
