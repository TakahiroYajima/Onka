using UnityEngine;

/// <summary>
/// アイテムが出現した後しばらく獲得できていなければ光らせる機能
/// </summary>
public class AppearedItemLightupper : MonoBehaviour
{
    [SerializeField] private Light light = null;
    [SerializeField] private ItemObject target = null;

    private float currentWaitTime = 0f;
    private float currentActionTime = 0f;
    private const float LightupTime = 20f;//20秒
    private const float MaxIntensity = 0.2f;

    private bool isGeted = false;

    private void OnEnable()
    {
        currentWaitTime = 0f;
        currentActionTime = 0f;
        light.intensity = 0f;
        light.enabled = false;
        isGeted = Onka.Manager.Data.DataManager.Instance.IsGetedItem(target.ItemKey);
    }

    private void OnDisable()
    {
        currentWaitTime = 0f;
        currentActionTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGeted)
        {
            Destroy(this);
            return;
        }
        if (target.gameObject.activeSelf)
        {
            if (currentWaitTime < LightupTime)
            {
                currentWaitTime += Time.deltaTime;
            }
            else
            {
                if (!light.enabled)
                {
                    light.enabled = true;
                }
                light.intensity = Mathf.PingPong(currentActionTime, MaxIntensity);
                currentActionTime += Time.deltaTime * MaxIntensity;
            }
        }
        isGeted = Onka.Manager.Data.DataManager.Instance.IsGetedItem(target.ItemKey);
        if (isGeted)
        {
            light.intensity = 0f;
            light.enabled = false;
        }
    }
}
