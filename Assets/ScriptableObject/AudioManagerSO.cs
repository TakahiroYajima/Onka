using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundSystem;
using Onka.Manager.Data;

[CreateAssetMenu]
public class AudioManagerSO : ScriptableObject {
	//private AudioManager _audioManager;
	//public void SetAudioManager(AudioManager audioManager) {
	//    this._audioManager = audioManager;
	//}

	public void PlaySE(string name)
	{
		if (SoundManager.Instance != null) {
            SoundManager.Instance.PlaySe(name);
		}
	}

    public void PlaySeWithKey(string key)
    {
        SoundData data = DataManager.Instance.GetMenuSE(key);
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySeWithKey(key);
        }
    }
    /// <summary>
    /// 汎用サウンド用 0:ボタン押下（OK)、1:キャンセル
    /// </summary>
    /// <param name="_seType"></param>
    //public void PlaySECommon(int _seType)
    //{
    //	if (SoundDataManager.Instance != null)
    //	{
    //		CommonSE commonSeType = (CommonSE)Enum.ToObject(typeof(CommonSE), _seType);
    //		SoundDataManager.Instance.PlayCommonSE(commonSeType);
    //       }
    //}

    //  public void PlayVOICE(string name) {
    //      if (SoundPlayer.Instance != null) {
    //          SoundPlayer.Instance.play(name);
    //}
    //  }
}