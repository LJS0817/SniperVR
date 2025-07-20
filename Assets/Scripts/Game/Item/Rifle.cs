using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    Animator _ani;

    public bool ShootTrigger;

    [SerializeField] Camera _scope;
    [SerializeField] Transform _scopeDial;
    float _targetZoom;
    Quaternion _targetZoomDial;

    [SerializeField] Transform _cylinder;
    [SerializeField] Transform _maxCylinderPosition;
    [SerializeField] Transform _frontCylinder;
    Vector3 _targetCylinder;
    Vector3 _defaultCylinderPosition;
    int _prevCylinderValue;
    float _targetCylinderDist;

    [SerializeField] bool _isOpenedCylinder;

    [SerializeField] Transform _trigger;
    [SerializeField] Magazine _magazine;
    [SerializeField] Transform _popAmmoDir;
    [SerializeField] bool _isSetStand;

    GameObject _curAmmo;

    // START, RANGE
    private readonly Vector2 ZOOM_RANGE = new Vector2(60f, 35f);
    private readonly Vector2 ZOOM_DIAL_RANGE = new Vector2(-140f, 215f);

    void Start()
    {
        _ani = GetComponent<Animator>();
        
        _isOpenedCylinder = true;
        _defaultCylinderPosition = _targetCylinder = _cylinder.position;
        _targetCylinderDist = _maxCylinderPosition.position.z - _cylinder.position.z;
        _prevCylinderValue = 0;

        _isSetStand = false;
        _curAmmo = null;

        _targetZoom = ZOOM_RANGE.x;
        _targetZoomDial = Quaternion.identity;
    }

    private void Update()
    {
        followScopeZoom();
        followCylinderPosition();
    }

    void followScopeZoom()
    {
        if (_scope.fieldOfView != _targetZoom)
        {
            _scope.fieldOfView = Mathf.Lerp(_scope.fieldOfView, _targetZoom, 30f * Time.deltaTime);
            _scopeDial.localRotation = Quaternion.Lerp(_scopeDial.localRotation, _targetZoomDial, 50f * Time.deltaTime);
        }
    }

    void followCylinderPosition()
    {
        if(_cylinder.position.z != _targetCylinder.z)
        {
            _cylinder.position = Vector3.Lerp(_cylinder.position, _targetCylinder, 30f * Time.deltaTime);
        }
    }

    public void OnSensorDataReceived(Dictionary<string, int> receivedValues)
    {
        Debug.Log("Rifle received data:");

        if (receivedValues.TryGetValue("Z", out int zoomValue))
        {
            Zoom(zoomValue);
        }
        if (receivedValues.TryGetValue("R", out int reloadValue))
        {
            _targetCylinder = _cylinder.position;
            _targetCylinder.z = _defaultCylinderPosition.z + _targetCylinderDist * reloadValue * 0.01f;
            if (reloadValue > 82)
            {
                _isOpenedCylinder = (reloadValue - _prevCylinderValue) > 0;
                if (_isOpenedCylinder) PopOutAmmo();
                else PushInAmmo();
            }
            _prevCylinderValue = reloadValue;
        }
        if (receivedValues.TryGetValue("E", out int elevationValue))
        {
            AdjustElevation(elevationValue);
        }
        if (receivedValues.TryGetValue("W", out int windageValue))
        {
            AdjustWindage(windageValue);
        }
        if (receivedValues.TryGetValue("P", out int parallaxValue))
        {
            AdjustParallax(parallaxValue);
        }
    }

    public void OpenCylinder()
    {
        if (_isOpenedCylinder) return;
        _ani.SetTrigger("OpenCylinder");
        _isOpenedCylinder = true;
        Debug.Log("Cylinder Opened.");
    }

    public void CloseCylinder()
    {
        if (!_isOpenedCylinder) return;
        _ani.SetTrigger("CloseCylinder");
        _isOpenedCylinder = false;
        Debug.Log("Cylinder Closed.");
    }

    public void Zoom(int zoomValue)
    {
        if (_scope != null)
        {
            float zoomRatio = zoomValue * 0.01f;
            _targetZoom = ZOOM_RANGE.x - ZOOM_RANGE.y * zoomRatio;
            _targetZoomDial = Quaternion.Euler(0f, ZOOM_DIAL_RANGE.x + ZOOM_DIAL_RANGE.y * zoomRatio, 0f);
            Debug.Log($"Zoom value: {zoomValue}, FOV: {_targetZoom}");
        }
    }

    private void PopOutAmmo()
    {
        Debug.Log("POP OUT");
        if (_curAmmo == null) return;
        _curAmmo.GetComponent<Rigidbody>().AddForce(_popAmmoDir.right * 5f, ForceMode.Impulse);
        _curAmmo.GetComponent<Rigidbody>().useGravity = true;
        BulletPool.Instance.ReturnAmmo(_curAmmo);
        _curAmmo = null;
    }

    private void PushInAmmo()
    {
        if (_curAmmo != null) return;
        Debug.Log("TEST");
        _curAmmo = _magazine.PopAmmo();
        _curAmmo.transform.parent = _frontCylinder;
        _curAmmo.transform.localPosition = Vector3.zero;
    }

    //public void StartReload()
    //{
    //    if (_isReloading) return;
    //    _isReloading = true;
    //    _ani.SetTrigger("Reload");
    //    Debug.Log("Reloading started.");
    //}

    //private void FinishReload()
    //{
    //    _isReloading = false;
    //    Debug.Log("Reloading finished.");
    //}

    public void AdjustElevation(int elevationValue)
    {
        Debug.Log($"Elevation value received: {elevationValue}");
    }

    public void AdjustWindage(int windageValue)
    {
        Debug.Log($"Windage value received: {windageValue}");
    }

    public void AdjustParallax(int parallaxValue)
    {
        Debug.Log($"Parallax value received: {parallaxValue}");
    }
}