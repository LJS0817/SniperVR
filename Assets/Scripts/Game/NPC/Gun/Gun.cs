using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] bool _canFire;

    [SerializeField] int _amount;

    protected int _limitAmount;
    [SerializeField] Transform _firePoint;
    [SerializeField] Transform _popPoint;
    [SerializeField] GameObject _magazineOrigin;
    [SerializeField] ParticleSystem _muzzleEffect;

    Bullet[] _bullet;
    const int MAX_BULLET_COUNT = 9;
    int _bulletIdx;

    Animator _ani;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        _bulletIdx = 0;
        _bullet = new Bullet[MAX_BULLET_COUNT];
        for(int i = 0; i <  MAX_BULLET_COUNT; i++)
        {
            _bullet[i] = BulletPool.Instance.CreateBullet().GetComponent<Bullet>();
            _bullet[i].Init(_firePoint);
        }

        _ani = GetComponent<Animator>();
        _amount = _limitAmount;
        _canFire = true;
        //_fireTime = _fireLimitTime;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //Debug.DrawRay(_firePoint.position, _firePoint.forward * 1000f, Color.red);
    }

    public Vector3 GetFirePoint()
    {
        return _firePoint.position;
    }

    public Vector3 GetFireDirection()
    {
        return _firePoint.forward;
    }

    public void Fire()
    {
        if(_canFire)
        {
            _muzzleEffect.Play();
            _canFire = false;
            _amount--;

            _bullet[_bulletIdx].gameObject.SetActive(true);
            _bullet[_bulletIdx++].Fire(true);
            if (_bulletIdx >= MAX_BULLET_COUNT) _bulletIdx = 0;

            _ani.SetTrigger("LoadAmmo");
        }
    }

    public bool CanFire()
    {
        return _canFire && !IsEmptyAmmo();
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
        //Debug.Log("POP");
    }
}
