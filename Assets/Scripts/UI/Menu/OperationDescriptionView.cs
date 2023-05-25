using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class OperationDescriptionView : MonoBehaviour
{
    [SerializeField] private GameObject layoutBaseObject = null;
    [SerializeField] private Text walkText = null;
    [SerializeField] private Text eyeText = null;
    [SerializeField] private Text dashText = null;
    [SerializeField] private Text crouchText = null;
    [SerializeField] private Text backText = null;

    public UnityAction onClosed = null;

    public void InitAndShow(UnityAction _onClosed)
    {
        SetTexts();
        layoutBaseObject.SetActive(true);
        onClosed = _onClosed;
    }

    private void SetTexts()
    {
        walkText.text = TextMaster.GetText("text_menu_operation_move");
        eyeText.text = TextMaster.GetText("text_menu_operation_viewpoint");
        dashText.text = TextMaster.GetText("text_menu_operation_dash");
        crouchText.text = TextMaster.GetText("text_menu_operation_crouch");
        backText.text = TextMaster.GetText("text_back");
    }

    public void Close()
    {
        layoutBaseObject.SetActive(false);
        if (onClosed != null)
        {
            onClosed();
        }
        Destroy(gameObject);
    }
}
