using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureAnimation : MonoBehaviour
{
    [SerializeField] private Material material = null;
    [SerializeField] private Texture2D[] animationTextures = null;

    private int currentIndex = 0;
    private float currentTime = 0f;
    private float changeTime = 0.5f;
    private bool isLoop = true;

    private bool isEnable = false;

    public void SetUp(float changeTime = 0.07f, bool isLoop = true)
    {
        this.changeTime = changeTime;
        this.isLoop = isLoop;
        material.mainTexture = animationTextures[0];
        isEnable = false;
    }

    public void StartAction()
    {
        isEnable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnable) return;

        if(currentTime >= changeTime)
        {
            var next = currentIndex + 1;
            if (!isLoop && next >= animationTextures.Length)
            {
                isEnable = false;
                return;
            }

            currentIndex = next % (animationTextures.Length);
            material.mainTexture = animationTextures[currentIndex];
            currentTime = 0f;
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }
}
