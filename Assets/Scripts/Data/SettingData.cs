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