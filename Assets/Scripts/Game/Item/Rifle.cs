using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    Animator _ani;

    [SerializeField] Camera _scope;
    [SerializeField] Transform _cylinder;
    [SerializeField] bool _isOpenedCylinder;

    [SerializeField] Transform _trigger;
    [SerializeField] Magazine _magazine;
    [SerializeField] Transform _popAmmoDir;
    [SerializeField] bool _isSetStand;

    Rigidbody _curAmmo;

    void Start()
    {
        _isOpenedCylinder = true;
        _isSetStand = false;
        _curAmmo = null;
        _ani = GetComponent<Animator>();
    }

    public void OnSensorDataReceived(Dictionary<string, int> receivedValues)
    {
        Debug.Log("Rifle received data:");

        // 각 센서 값에 따른 로직 처리
        // 예시: "Z", "R", "E", "W", "P" 키를 사용하여 값 접근
        if (receivedValues.TryGetValue("Z", out int zoomValue))
        {
            Zoom(zoomValue);
        }
        if (receivedValues.TryGetValue("R", out int reloadValue))
        {
            if (reloadValue > 50)
            {
                if (_isOpenedCylinder) PopOutAmmo();
                else PushInAmmo();
            }
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
            float minFov = 15f;
            float maxFov = 60f;
            _scope.fieldOfView = Mathf.Lerp(maxFov, minFov, zoomValue / 100f);
            Debug.Log($"Zoom value: {zoomValue}, FOV: {_scope.fieldOfView}");
        }
    }

    private void PopOutAmmo()
    {
        if (_curAmmo == null) return;
        _curAmmo.AddForce(_popAmmoDir.position.normalized * 10f, ForceMode.Impulse);
    }

    private void PushInAmmo()
    {
        if (_curAmmo != null) return;
        //_curAmmo = 
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