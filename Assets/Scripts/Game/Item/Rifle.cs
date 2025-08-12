using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    [SerializeField] Scope _scope;
    [SerializeField] Cylinder _cylinder;

    [SerializeField] Transform _trigger;
    [SerializeField] Transform _firePoint;
    [SerializeField] Magazine _magazine;
    [SerializeField] bool _isSetStand;

    [SerializeField] TextMeshProUGUI _ammoText;

    Bullet _bullet;

    int _bulletCount;
    const int _MAX_BULLET_COUNT = 35;

    void Start()
    {
        _isSetStand = false;

        _bulletCount = _MAX_BULLET_COUNT;

        _bullet = BulletPool.Instance.CreateBullet().GetComponent<Bullet>();
        _bullet.Init(_firePoint);
        _magazine.SetFullMagazine();

        _cylinder.SetAmmoEvent(PopAmmo);
        _ammoText.text = _magazine.GetCurrentAmmoCount() + " / " + _bulletCount;
    }

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            fire();
        }
    }

    public Ammo PopAmmo()
    {
        Ammo am = _magazine.PopAmmo();
        if(am != null) _bulletCount--;
        return am;
    } 

    public void SetTriggerState(float value)
    {
        //-90 ~ -70
        _trigger.localRotation = Quaternion.Euler(-90f + 20 * value, _trigger.localRotation.y, _trigger.localRotation.z);
        if(value == 1f && _cylinder.ReadyToFire())
        {
            fire();
        }
    }

    void fire()
    {
        _cylinder.Fire();
        _bulletCount--;
        _bullet.gameObject.SetActive(true);
        _bullet.Fire();
        _ammoText.text = _magazine.GetCurrentAmmoCount() + " / " + _bulletCount;
    }

    public void Reload()
    {
        if (_magazine == null) return;
        _magazine.SetFullMagazine();
    }

    public void OnSensorDataReceived(Dictionary<string, int> receivedValues)
    {
        Debug.Log("Rifle received data:");
        if (receivedValues.ContainsKey("Z") && receivedValues.TryGetValue("Z", out int zoomValue))
        {
            _scope.Zoom(zoomValue);
        }
        if (receivedValues.ContainsKey("R") && receivedValues.TryGetValue("R", out int reloadValue))
        {
            _cylinder.SetCylinderPosition(reloadValue);
        }
        if (receivedValues.ContainsKey("E") && receivedValues.TryGetValue("E", out int elevationValue))
        {
            _scope.AdjustElevation(elevationValue);
        }
        if (receivedValues.ContainsKey("W") && receivedValues.TryGetValue("W", out int windageValue))
        {
            _scope.AdjustWindage(windageValue);
        }
        if (receivedValues.ContainsKey("P") && receivedValues.TryGetValue("P", out int parallaxValue))
        {
            _scope.AdjustParallax(parallaxValue);
        }
    }
}