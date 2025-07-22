using System.Collections.Generic;
using UnityEngine;

public class Magazine : MonoBehaviour
{
    private Stack<Ammo> _ammo;

    public Transform FirstAmmoPosition;
    public Transform MagazineBottom;

    const int MAX_CAPACITY = 5;

    private readonly Vector3 _rightAmmoStep = new Vector3(0, 0.0144f, 0);
    private readonly Vector3 _leftAmmoStep = new Vector3(-0.0092f, -0.0074f, 0);
    private readonly Vector3 _bottomStep = new Vector3(0, -0.006952f, 0);

    private void Update()
    {
        
    }

    private Vector3 GetAmmoLocalPosition(int index)
    {
        Vector3 basePos = FirstAmmoPosition.localPosition;
        Vector3 step = (index % 2 == 0 ? Vector3.zero : _rightAmmoStep + _leftAmmoStep);
        return basePos + step + _rightAmmoStep * ((index) / 2);
    }

    public Ammo PopAmmo()
    {
        if (!IsMagazineEmpty())
        {
            Ammo ammo = _ammo.Pop();
            MagazineBottom.localPosition -= _bottomStep;
            return ammo;
        }
        else
        {
            Debug.Log("탄창이 비었다");
            return null; 
        }
    }

    public int GetCurrentAmmoCount()
    {
        return _ammo.Count;
    }

    public bool IsMagazineEmpty()
    {
        return _ammo.Count == 0;
    }

    public void SetFullMagazine()
    {
        if(_ammo == null) _ammo = new Stack<Ammo>();
        for (int i = _ammo.Count; i < MAX_CAPACITY; i++)
        {
            Ammo obj = BulletPool.Instance.GetAmmo();
            obj.LoadInMagazine(MagazineBottom, GetAmmoLocalPosition(i));
            _ammo.Push(obj);
            if(i > 0) MagazineBottom.localPosition += _bottomStep;
        }
    }
}