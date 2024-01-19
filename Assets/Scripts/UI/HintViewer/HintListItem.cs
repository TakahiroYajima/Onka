using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using SoundSystem;

public class HintListItem : MonoBehaviour
{
    public class Parameter
    {
        public HintSet HintSet { get; set; }
        public UnityAction OnClick { get; set; }
    }

    [SerializeField]
    private TextButton textButton;

    public HintSet HintSet { get; private set; }

    public void Setup(Parameter param)
    {
        HintSet = param.HintSet;
        textButton.button.onClick.RemoveAllListeners();
        textButton.button.onClick.AddListener(param.OnClick);
        textButton.button.onClick.AddListener(()=>SoundManager.Instance.PlaySeWithKey("menuse_click"));
        textButton.text.text = TextMaster.GetHint(param.HintSet.titleKey);
    }
}
