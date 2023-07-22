using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    private SettingData settingData = null;
    public Action<float, float> OnSetUserSettings = null;

    protected override void Awake()
    {
        base.Awake();
        TextMaster.Initialize();
        LoadSettingData();
        LayerMaskData.Initialize();
    }

    private void LoadSettingData()
    {
        settingData = FileManager.LoadSaveData<SettingData>(SaveType.SettingData, "SettingData");
        if(settingData == default)
        {
            settingData = new SettingData();
            settingData.language = Language.Ja;
            settingData.brightness = 1f;
            settingData.mouseSensitivity = 10f;
            settingData.difficulty = Difficulty.Normal;
            FileManager.DataSave<SettingData>(settingData, SaveType.SettingData, "SettingData");
        }
        TextMaster.ChangeLanguage(settingData.language);
    }
    
    public void ChangeLanguage(Language language)
    {
        settingData.language = language;
        TextMaster.ChangeLanguage(settingData.language);
        FileManager.DataSave<SettingData>(settingData, SaveType.SettingData, "SettingData");
    }

    public void SetUserSettings(float brightness, float mouseSensitivity, Difficulty difficulty)
    {
        settingData.brightness = brightness;
        settingData.mouseSensitivity = mouseSensitivity;
        settingData.difficulty = difficulty;
        FileManager.DataSave<SettingData>(settingData, SaveType.SettingData, "SettingData");
        OnSetUserSettings?.Invoke(brightness, mouseSensitivity);
    }

    public SettingData GetSettingData()
    {
        return settingData;
    }
}
