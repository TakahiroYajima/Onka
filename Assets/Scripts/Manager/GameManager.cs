using System;
using Onka.Manager.Data;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    private SettingData settingData = null;
    public Action<float, float> OnSetUserSettings = null;

    protected override void Awake()
    {
        base.Awake();
        TextMaster.Initialize();
        LayerMaskData.Initialize();
    }

    private void Start()
    {
        LoadSettingData();
        //Debug.Log($"LoadSettingData complete");
    }

    private void LoadSettingData()
    {
        settingData = FileManager.LoadSaveData<SettingData>(SaveType.SettingData, DataManager.SettingDataKeyName);
        if(settingData == null || settingData == default)
        {
            settingData = new SettingData();
            settingData.language = Language.Ja;
            settingData.brightness = 1f;
            settingData.mouseSensitivity = 10f;
            settingData.difficulty = Difficulty.Normal;
            FileManager.DataSave<SettingData>(settingData, SaveType.SettingData, DataManager.SettingDataKeyName);
        }
        TextMaster.ChangeLanguage(settingData.language);
    }
    
    public void ChangeLanguage(Language language)
    {
        settingData.language = language;
        TextMaster.ChangeLanguage(settingData.language);
        FileManager.DataSave<SettingData>(settingData, SaveType.SettingData, DataManager.SettingDataKeyName);
    }

    public void SetUserSettings(float brightness, float mouseSensitivity, Difficulty difficulty)
    {
        settingData.brightness = brightness;
        settingData.mouseSensitivity = mouseSensitivity;
        settingData.difficulty = difficulty;
        FileManager.DataSave<SettingData>(settingData, SaveType.SettingData, DataManager.SettingDataKeyName);
        OnSetUserSettings?.Invoke(brightness, mouseSensitivity);
    }

    public SettingData GetSettingData()
    {
        return settingData;
    }

    ///// <summary>
    ///// ディスプレイの明るさを設定
    ///// </summary>
    ///// <param name="brightness"></param>
    //public void SetDisplayBrightness(float brightness)
    //{
    //    RenderSettings.ambientIntensity = brightness;
    //    Debug.Log($"SetDisplayBrightness : {RenderSettings.ambientIntensity}");
    //}
}
