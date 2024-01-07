[System.Serializable]
public class SettingData
{
    public Language language;
    public float brightness;//画面の明るさ
    public float mouseSensitivity;//マウス感度
    public Difficulty difficulty;//難易度
}

public enum Difficulty
{
    Normal,
    Hard,
}

public class SettingConstant
{
    //設定表示用
    public const float BrightnessMin = 1f;
    public const float BrightnessMax = 3f;
    public const float MouseSensitivityMin = 1f;
    public const float MouseSensitivityMax = 10f;
    //プレイヤーのライト
    public const float PlayerLightDark = 0.9f;
    public const float PlayerLightBright = 1.2f;
    //PostProcessの各パラメータ
    //public const float BloomIntensityDark = 1f;
    //public const float BloomIntensityBright = 3f;
    //public const float BloomThresholdDark = 0.57f;
    //public const float BloomThresholdBright = 0.3f;
    //public const float AmbientIntensityDark = 0.95f;
    //public const float AmbientIntensityBright = 0.3f;
    //PostProcessの各パラメータ（URP）
    public const float BloomThreshold = 0.9f;
    public const float BloomIntensityMax = 1f;
    public const float BloomIntensityMin = 0.4f;
    public const float ColorAdjustmentsMax = 2.2f;
    public const float ColorAdjustmentMin = 0.3f;
    //public const float ColorAdjustmentDefault = 1f;

}