using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    private SettingData settingData = null;

    protected override void Awake()
    {
        base.Awake();
        foreach(var v in UnityEngine.Localization.Settings.LocalizationSettings.AvailableLocales.Locales)
        {
            Debug.Log(v);
        }
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
}
