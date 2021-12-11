using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraZoom : MonoBehaviour
{
    [HideInInspector] public float initZoom = 60f;
    [HideInInspector] public float minZoom = 26f;
    [HideInInspector] public float zoomDel = 0.5f;

    private Camera camera;
    private float currentTime = 0f;
    private float targetZoom = 0f;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void Start()
    {
        initZoom = camera.fieldOfView;
    }

    public void Initialize(float _initZoom, float _minZoom, float _zoomDel)
    {
        initZoom = _initZoom;
        minZoom = _minZoom;
        zoomDel = _zoomDel;
    }

    public IEnumerator ZoomIn()
    {
        currentTime = 0f;
        targetZoom = initZoom - minZoom;
        while (currentTime < 1f)
        {
            camera.fieldOfView = initZoom - (targetZoom * currentTime);
            currentTime += Time.deltaTime / zoomDel;
            yield return null;
        }
        camera.fieldOfView = minZoom;
    }
    public IEnumerator ZoomOut()
    {
        //カメラズームを戻す
        currentTime = 0f;
        targetZoom = initZoom - minZoom;
        while (currentTime < 1f)
        {
            camera.fieldOfView = minZoom + (targetZoom * currentTime);
            currentTime += Time.deltaTime / zoomDel;
            yield return null;
        }
        camera.fieldOfView = initZoom;
    }
}
