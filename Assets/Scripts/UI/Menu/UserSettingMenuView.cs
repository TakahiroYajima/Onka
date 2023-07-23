using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 明るさ、マウス感度、難易度設定
/// </summary>
public class UserSettingMenuView : MonoBehaviour
{
    [SerializeField] private TextButton okButton = null;
    [SerializeField] private TextButton cancelButton = null;
    [SerializeField] private Slider brightnessAdjustmentSlider = null;
    [SerializeField] private Text brightnessAdjustmentLabel = null;
    [SerializeField] private Text brightnessText = null;
    [SerializeField] private Slider mouseSensitivitySlider = null;
    [SerializeField] private Text mouseSensitivityLabel = null;
    [SerializeField] private Text mouseSensitivityText = null;

    [SerializeField] private TextButton normalDifficaryButton = null;
    [SerializeField] private TextButton hardDifficaryButton = null;
    [SerializeField] private Text difficaryLabel = null;
    [SerializeField] private Text difficaryDescriptionTexst = null;

    private GameSceneManager gameSceneManager;
    private SettingData settingData;
    private Action onClose = null;
    private Difficulty currentDifficulty = Difficulty.Normal;
    private float brightnessValue = 0f;
    private float mouseSensitivityValue = 0f;

    public void SetUp(Action onClose)
    {
        gameSceneManager = GameSceneManager.Instance as GameSceneManager;
        this.onClose = onClose;
        settingData = GameManager.Instance.GetSettingData();

        okButton.button.onClick.RemoveAllListeners();
        okButton.button.onClick.AddListener(OnClickOK);
        cancelButton.button.onClick.RemoveAllListeners();
        cancelButton.button.onClick.AddListener(Close);

        brightnessAdjustmentSlider.onValueChanged.RemoveAllListeners();
        brightnessAdjustmentSlider.onValueChanged.AddListener((value) => { CalcBrightnessValue(); brightnessText.text = brightnessValue.ToString("F1"); gameSceneManager.SetDisplayBrightness(brightnessValue); });
        float brightnessMagnification = SettingConstant.BrightnessMax - SettingConstant.BrightnessMin;
        float brightnessInitValue = (settingData.brightness - SettingConstant.BrightnessMin) / brightnessMagnification;
        brightnessAdjustmentSlider.value = brightnessInitValue;
        brightnessText.text = settingData.brightness.ToString("F1");

        mouseSensitivitySlider.onValueChanged.RemoveAllListeners();
        mouseSensitivitySlider.onValueChanged.AddListener((value) => { CalcMouseSensitivityValue(); mouseSensitivityText.text = mouseSensitivityValue.ToString("F1"); gameSceneManager.SetMouseSensitivity(mouseSensitivityValue); });
        float mouseSensitivityMagnification = SettingConstant.MouseSensitivityMax - SettingConstant.MouseSensitivityMin;
        float mouseSensitivityInitValue = (settingData.mouseSensitivity - SettingConstant.MouseSensitivityMin) / mouseSensitivityMagnification;
        mouseSensitivitySlider.value = mouseSensitivityInitValue;
        mouseSensitivityText.text = settingData.mouseSensitivity.ToString("F1");

        CalcBrightnessValue();
        CalcMouseSensitivityValue();

        currentDifficulty = settingData.difficulty;
        normalDifficaryButton.button.onClick.RemoveAllListeners();
        normalDifficaryButton.button.onClick.AddListener(()=> SetDifficulty(Difficulty.Normal));
        hardDifficaryButton.button.onClick.RemoveAllListeners();
        hardDifficaryButton.button.onClick.AddListener(() => SetDifficulty(Difficulty.Hard));
        SetDifficulty(settingData.difficulty);
        SetTexts();
        SetDifficulyDescriptionText(currentDifficulty);
        Debug.Log($"SaveData : {settingData.brightness}, {settingData.mouseSensitivity}, {settingData.difficulty}");
        Debug.Log($"Init : {brightnessValue}, {mouseSensitivityValue}, {currentDifficulty}");
    }

    public void SetTexts()
    {
        okButton.text.text = TextMaster.GetText("text_decision");
        cancelButton.text.text = TextMaster.GetText("text_cancel");
        brightnessAdjustmentLabel.text = TextMaster.GetText("setting_brightness_label");
        mouseSensitivityLabel.text = TextMaster.GetText("setting_mouse_sensitivity_label");

        normalDifficaryButton.text.text = TextMaster.GetText("setting_normal_difficary");
        hardDifficaryButton.text.text = TextMaster.GetText("setting_hard_difficary");
        difficaryLabel.text = TextMaster.GetText("setting_difficary_label");
    }

    private void SetDifficulyDescriptionText(Difficulty difficulty)
    {
        if (difficulty == Difficulty.Normal)
        {
            difficaryDescriptionTexst.text = TextMaster.GetText("setting_difficary_description_normal");
        }
        else
        {
            difficaryDescriptionTexst.text = TextMaster.GetText("setting_difficary_description_hard");
        }
    }

    private void CalcBrightnessValue()
    {
        float brightnessMagnification = SettingConstant.BrightnessMax - SettingConstant.BrightnessMin;
        brightnessValue = SettingConstant.BrightnessMin + brightnessAdjustmentSlider.value * brightnessMagnification;
    }

    private void CalcMouseSensitivityValue()
    {
        float mouseSensitivityMagnification = SettingConstant.MouseSensitivityMax - SettingConstant.MouseSensitivityMin;
        mouseSensitivityValue = SettingConstant.MouseSensitivityMin + mouseSensitivitySlider.value * mouseSensitivityMagnification;
    }

    private void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        normalDifficaryButton.button.interactable = currentDifficulty != Difficulty.Normal;
        hardDifficaryButton.button.interactable = currentDifficulty != Difficulty.Hard;
        SetDifficulyDescriptionText(currentDifficulty);
    }

    private void OnClickOK()
    {
        Debug.Log($"Save : {brightnessValue}, {mouseSensitivityValue}, {currentDifficulty}");

        GameManager.Instance.SetUserSettings(brightnessValue, mouseSensitivityValue, currentDifficulty);
        Close();
    }

    private void Close()
    {
        gameSceneManager.SetDisplayBrightness(settingData.brightness);
        onClose?.Invoke();
    }
}
