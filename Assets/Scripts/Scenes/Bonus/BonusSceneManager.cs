using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Onka.Manager.Data;

public class BonusSceneManager : SceneBase
{
    [HideInInspector] public CharactorDataList charactorDataList = null;
    [SerializeField] private BonusMenu bonusMenu = null;

    protected override void Initialize()
    {
        base.Initialize();
        charactorDataList = FileManager.LoadSaveData<CharactorDataList>(SaveType.MasterData, DataManager.CharactorDataFileName);
        if(charactorDataList != null && charactorDataList != default)
        {
            for(int i = 0; i < charactorDataList.charactorDataList.Count; i++)
            {
                if (string.IsNullOrEmpty(charactorDataList.charactorDataList[i].imageName)) continue;
                charactorDataList.charactorDataList[i].sprite = ResourceManager.LoadResourceSprite(ResourceManager.CharactorResourcePath, charactorDataList.charactorDataList[i].imageName);
            }
        }

        bonusMenu.Initialize();
    }
}
