using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    Animator _ani;

    [SerializeField] Camera _scope;
    [SerializeField] Transform _scopeDial;
    float _targetZoom;
    Quaternion _targetZoomDial;

    [SerializeField] Transform _cylinder;
    [SerializeField] Transform _maxCylinderPosition;
    [SerializeField] Transform _frontCylinder;
    [SerializeField] Transform _ammoInsertPoint;
    Vector3 _targetCylinder;
    Vector3 _defaultCylinderPosition;
    int _prevCylinderValue;
    float _targetCylinderDist;

    [SerializeField] bool _isOpenedCylinder;

    [SerializeField] Transform _trigger;
    [SerializeField] Transform _firePoint;
    [SerializeField] Magazine _magazine;
    [SerializeField] Transform _popAmmoDir;
    [SerializeField] bool _isSetStand;

    Ammo _curAmmo;
    Bullet _bullet;

    // START, RANGE
    private readonly Vector2 ZOOM_RANGE = new Vector2(60f, 35f);
    private readonly Vector2 ZOOM_DIAL_RANGE = new Vector2(-140f, 215f);

    void Start()
    {
        _ani = GetComponent<Animator>();
        
        _isOpenedCylinder = true;
        _defaultCylinderPosition = _targetCylinder = _cylinder.localPosition;
        _targetCylinderDist = _maxCylinderPosition.localPosition.z - _cylinder.localPosition.z;
        _prevCylinderValue = 0;

        _isSetStand = false;
        _curAmmo = null;

        _targetZoom = ZOOM_RANGE.x;
        _targetZoomDial = Quaternion.identity;

        BulletPool.Instance.InitializeMagazine(_ammoInsertPoint, _popAmmoDir.right);
        _bullet = BulletPool.Instance.CreateBullet().GetComponent<Bullet>();
        _bullet.Init(_firePoint);
        _magazine.SetFullMagazine();
    }

    private void Update()
    {
        followScopeZoom();
        followCylinderPosition();
    }

    public void SetTriggerState(float value)
    {
        //-90 ~ -70
        _trigger.localRotation = Quaternion.Euler(-90f + 20 * value, _trigger.localRotation.y, _trigger.localRotation.z);
        if(value == 1f && _curAmmo != null && _curAmmo.isAvaliable())
        {
            fire();
        }
    }

    void fire()
    {
        _curAmmo.Fire();
        _bullet.gameObject.SetActive(true);
        _bullet.Fire();
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
        if(_cylinder.localPosition.z != _targetCylinder.z)
        {
            _cylinder.localPosition = Vector3.Lerp(_cylinder.localPosition, _targetCylinder, 30f * Time.deltaTime);
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
            _targetCylinder = _cylinder.localPosition;
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
        _curAmmo.PopOut();
        BulletPool.Instance.ReturnAmmo(_curAmmo);
        _curAmmo = null;
    }

    private void PushInAmmo()
    {
        if (_curAmmo != null) return;
        _curAmmo = _magazine.PopAmmo();
        _curAmmo.LoadAmmo(_frontCylinder);
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