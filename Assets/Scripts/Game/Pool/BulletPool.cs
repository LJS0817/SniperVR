using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    private Stack<Ammo> _ammoPool;
    private const int MAX_CAPACITY = 60;

    public GameObject HitSign;
    public GameObject AmmoPrefab;
    public GameObject BulletPrefab;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _ammoPool = new Stack<Ammo>();
    }

    public void InitializeMagazine(Transform point, Vector3 popDir)
    {
        for (int i = 0; i < MAX_CAPACITY; i++)
        {
            Ammo ammo = Instantiate(AmmoPrefab, transform).GetComponent<Ammo>();
            ammo.Init(point, popDir);
            _ammoPool.Push(ammo);
        }
    }

    public void SetHitSign(Vector3 pos)
    {
        GameObject obj = Instantiate(HitSign, pos, Quaternion.identity, transform);
        Destroy(obj, 15f);
    }

    public void ReturnAmmo(Ammo ammoToReturn)
    {
        if (ammoToReturn == null) return;

        ammoToReturn.gameObject.SetActive(false);

        ammoToReturn.gameObject.GetComponent<Rigidbody>().useGravity = false;
        ammoToReturn.gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        ammoToReturn.transform.parent = transform;
        _ammoPool.Push(ammoToReturn);
    }

    public Ammo GetAmmo()
    {
        if (_ammoPool.Count > 0)
        {
            Ammo ammo = _ammoPool.Pop();
            return ammo;
        }
        else
        {
            Debug.LogWarning("BulletPool Empty");

            return null;
        }
    }

    public GameObject CreateBullet()
    {
        return Instantiate(BulletPrefab, transform);
    }

    public int GetAvailableBulletCount()
    {
        return _ammoPool.Count;
    }
}
