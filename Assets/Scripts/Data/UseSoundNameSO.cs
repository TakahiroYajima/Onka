using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class UseSoundNameSO
{
    public List<UseSoundNameData> useSoundNameDataList = new List<UseSoundNameData>();

    public List<UseSoundNameData> GetOneSceneUseData(SceneType _type)
    {
        return useSoundNameDataList.Where(x => x.scene == _type).ToList();
    }
}

[System.Serializable]
public class UseSoundNameData
{
    public SceneType scene;
    public string key;
}

[System.Serializable]
public enum SceneType
{
    Initialize,
    Title,
    Game,
}