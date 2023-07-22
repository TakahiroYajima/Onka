#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class ScreenShot : MonoBehaviour
{
    [SerializeField] private Camera _camera = null;

    public const string FilePath = "Capture";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            CaptureScreenShot("D:/YajinProject/Onka/Onka/宣伝/スクリーンショット/" + FilePath, date + ".png");
        }
    }

    //private void CaptureScreenShot(string filePath)
    //{
    //    ScreenCapture.CaptureScreenshot(filePath);
    //}
    // カメラのスクリーンショットを保存する
    private void CaptureScreenShot(string filePath,string name)
    {
        if(_camera == null)
        {
            _camera = Camera.main;
        }
        var rt = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 24);
        var prev = _camera.targetTexture;
        _camera.targetTexture = rt;
        _camera.Render();
        _camera.targetTexture = prev;
        RenderTexture.active = rt;

        var screenShot = new Texture2D(
            _camera.pixelWidth,
            _camera.pixelHeight,
            TextureFormat.RGB24,
            false);
        screenShot.ReadPixels(new Rect(0, 0, screenShot.width, screenShot.height), 0, 0);
        screenShot.Apply();

        var bytes = screenShot.EncodeToPNG();
        Destroy(screenShot);

        //ディレクトリが無ければ作成
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        Debug.Log("保存 : " + filePath  + "/ :: name :  "+ name);

        File.WriteAllBytes(filePath + "/" + name, bytes);
    }
}
#endif