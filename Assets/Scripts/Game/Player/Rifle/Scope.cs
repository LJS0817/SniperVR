using System.Collections.Generic;
using UnityEngine;

public class Scope : MonoBehaviour
{
    public enum SCOPE_TYPE
    {
        E_ZOOM,
        E_WINDAGE,
        E_ELEVATION,
        E_PARALLAX
    }

    [SerializeField] Camera _scope;

    [SerializeField] List<Transform> _dials;

    float _targetZoom;
    Quaternion _targetZoomDial;

    // START, RANGE
    private readonly Vector2 ZOOM_RANGE = new Vector2(12.8f, 12.4f);
    private readonly Vector2 ZOOM_DIAL_RANGE = new Vector2(-140f, 215f);
    const float ROTATION_PER_CLICK = 0.00775f;  //0.0155f * 0.5f;
    const float DIAL_ROTATION_PER_CLICK = 3.6f;

    int _prevElevationValue;
    int _prevWindageValue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //0.0155f
    void Start()
    {
        _prevWindageValue = _prevElevationValue = 0;
        _targetZoom = ZOOM_RANGE.x;
        _targetZoomDial = Quaternion.identity;
    } 

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(_scope.transform.position, _scope.transform.forward * 1000f, Color.blue);
        followScopeZoom();
        if (Input.GetKeyDown(KeyCode.W))
        {
            AdjustElevation(_prevElevationValue + 1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            AdjustElevation(_prevElevationValue - 1);
        }
    }

    void followScopeZoom()
    {
        if (_scope.fieldOfView != _targetZoom)
        {
            _scope.fieldOfView = Mathf.Lerp(_scope.fieldOfView, _targetZoom, 30f * Time.deltaTime);
            getDial(SCOPE_TYPE.E_ZOOM).localRotation = Quaternion.Lerp(getDial(SCOPE_TYPE.E_ZOOM).localRotation, _targetZoomDial, 50f * Time.deltaTime);
        }
    }

    public void Zoom(int zoomValue)
    {
        if (_scope != null)
        {
            float zoomRatio = zoomValue * 0.01f;
            _targetZoom = ZOOM_RANGE.x - ZOOM_RANGE.y * zoomRatio;
            _targetZoomDial = Quaternion.Euler(0f, ZOOM_DIAL_RANGE.x + ZOOM_DIAL_RANGE.y * zoomRatio, 0f);
            Debug.Log($"Zoom value: {zoomValue}, FOV: x{(ZOOM_RANGE.x / _targetZoom).ToString("F1")}");
        }
    }
    
    public void AdjustElevation(int eVal)
    {
        Debug.Log($"Elevation value received: {eVal},     {_prevElevationValue}");
        bool dir = eVal - _prevElevationValue < 0;
        _scope.transform.Rotate(dir ? ROTATION_PER_CLICK : -ROTATION_PER_CLICK, 0, 0);
        getDial(SCOPE_TYPE.E_ELEVATION).Rotate(0, dir ? DIAL_ROTATION_PER_CLICK : -DIAL_ROTATION_PER_CLICK, 0);
        _prevElevationValue = eVal;
    }
    
    public void AdjustWindage(int wVal)
    {
        Debug.Log($"Windage value received: {wVal},     {_prevWindageValue}");
        bool dir = wVal - _prevWindageValue > 0;
        _scope.transform.Rotate(0, dir ? ROTATION_PER_CLICK : -ROTATION_PER_CLICK, 0);
        getDial(SCOPE_TYPE.E_WINDAGE).Rotate(dir ? DIAL_ROTATION_PER_CLICK : -DIAL_ROTATION_PER_CLICK, 0, 0);
        _prevWindageValue = wVal;
    }

    public void AdjustParallax(int parallaxValue)
    {
        Debug.Log($"Parallax value received: {parallaxValue}");
    }

    Transform getDial(SCOPE_TYPE t)
    {
        return _dials[((int)t)];
    }
}
