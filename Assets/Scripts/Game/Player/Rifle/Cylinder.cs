using UnityEngine;

public class Cylinder : MonoBehaviour
{
    [SerializeField] Transform _cylinder;
    [SerializeField] Transform _maxCylinderPosition;
    [SerializeField] Transform _frontCylinder;
    [SerializeField] Transform _ammoInsertPoint;
    [SerializeField] Transform _popAmmoDir;

    public delegate Ammo getAmmo();

    public getAmmo _getAmmo;

    Vector3 _targetCylinder;
    Vector3 _defaultCylinderPosition;
    int _prevCylinderValue;
    float _targetCylinderDist;

    [SerializeField] bool _isOpenedCylinder;
    Ammo _curAmmo;

    public void SetAmmoEvent(getAmmo e)
    {
        _getAmmo += e;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isOpenedCylinder = true;
        _defaultCylinderPosition = _targetCylinder = _cylinder.localPosition;
        _targetCylinderDist = _maxCylinderPosition.localPosition.z - _cylinder.localPosition.z;
        _prevCylinderValue = 0;
        _curAmmo = null;

        BulletPool.Instance.InitializeMagazine(_ammoInsertPoint, _popAmmoDir.right);
    }

    // Update is called once per frame
    void Update()
    {
        followCylinderPosition();
    }

    void followCylinderPosition()
    {
        if (_cylinder.localPosition.z != _targetCylinder.z)
        {
            _cylinder.localPosition = Vector3.Lerp(_cylinder.localPosition, _targetCylinder, 30f * Time.deltaTime);
        }
    }

    public bool ReadyToFire()
    {
        return _curAmmo != null && _curAmmo.isAvaliable();
    }

    public void Fire()
    {
        if (_curAmmo != null) _curAmmo.Fire();
    }

    public void SetCylinderPosition(int reloadValue)
    {
        _targetCylinder = _cylinder.localPosition;
        _targetCylinder.z = _defaultCylinderPosition.z + _targetCylinderDist * reloadValue * 0.01f;
        if (reloadValue > 82)
        {
            _isOpenedCylinder = (reloadValue - _prevCylinderValue) > 0;
            if (_isOpenedCylinder) popOutAmmo();
            else pushInAmmo();
        }
        _prevCylinderValue = reloadValue;
    }

    private void popOutAmmo()
    {
        Debug.Log("POP OUT");
        if (_curAmmo == null) return;
        _curAmmo.PopOut();
        BulletPool.Instance.ReturnAmmo(_curAmmo);
        _curAmmo = null;
    }

    private void pushInAmmo()
    {
        if (_curAmmo != null) return;
        _curAmmo = _getAmmo();
        _curAmmo.LoadAmmo(_frontCylinder);
    }
}
