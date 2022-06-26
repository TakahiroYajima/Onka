using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : SingletonMonoBehaviour<CrosshairManager>
{
    [SerializeField] private Sprite[] centerSprites = null;

    //FirstPersonAIOのUI
    [HideInInspector] public GameObject Crosshair { get; private set; } = null;
    private Image crosshairImage = null;

    [HideInInspector] public GameObject StaminaGauge { get; private set; } = null;
    public void FindCrosshair()
    {
        if (Crosshair == null)
        {
            Crosshair = GameObject.Find("Crosshair");
            if (Crosshair != null)
            {
                crosshairImage = Crosshair.GetComponent<Image>();
            }
        }
    }
    public void SetCrosshairActive(bool _isActive)
    {
        FindCrosshair();
        if (Crosshair != null)
        {
            crosshairImage.enabled = _isActive;
        }
    }
    public void SetCrosshairSprite(Sprite sprite)
    {
        FindCrosshair();
        if (crosshairImage != null)
        {
            crosshairImage.sprite = sprite;
        }
    }

    public void ChangeCenterSprites(CrosshairType type)
    {
        if ((int)type >= centerSprites.Length) return;
        SetCrosshairSprite(centerSprites[(int)type]);
    }

    public void FindStaminaGauge()
    {
        if (StaminaGauge == null)
        {
            StaminaGauge = GameObject.Find("StaminaMeter");
        }
    }
    public void SetStaminaGaugeActive(bool _isActive)
    {
        FindStaminaGauge();
        if (StaminaGauge != null)
        {
            StaminaGauge.SetActive(_isActive);
        }
    }
}

public enum CrosshairType
{
    Normal = 0,
    Tapable = 1,
    DoorKey = 2,
    Door = 3,
}