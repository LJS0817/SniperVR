using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    bool _canFire;

    int _amount;

    protected int _limitAmount;
    [SerializeField] Transform _firePoint;
    [SerializeField] Transform _popPoint;
    [SerializeField] GameObject _magazineOrigin;

    Animator _ani;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        _ani = GetComponent<Animator>();
        _amount = _limitAmount;
        _canFire = true;
        //_fireTime = _fireLimitTime;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        Debug.DrawRay(_firePoint.position, _firePoint.forward * 1000f, Color.red);
    }

    public void Fire()
    {
        if(_canFire)
        {
            _ani.SetTrigger("LoadAmmo");
            _amount--;
            _canFire = false;
        }
    }

    public void Reload()
    {
        _amount = _limitAmount;
    }

    public void LoadAmmo()
    {
        if (_amount > 0) _canFire = true;
        else _ani.SetTrigger("Reload");
    }

    public bool IsEmptyAmmo()
    {
        return _amount == 0;
    }

    public void PopOutAmmo()
    {
        Debug.Log("POP");
    }
}
