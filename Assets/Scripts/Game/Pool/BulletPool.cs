using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    private Stack<GameObject> _ammoPool;
    private const int MAX_CAPACITY = 60;
    public GameObject AmmoPrefab;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _ammoPool = new Stack<GameObject>();
        InitializeMagazine();
    }

    private void InitializeMagazine()
    {
        for (int i = 0; i < MAX_CAPACITY; i++)
        {
            GameObject ammo = Instantiate(AmmoPrefab, transform);
            ammo.SetActive(false);
            _ammoPool.Push(ammo);
        }
    }

    public void ReturnAmmo(GameObject ammoToReturn)
    {
        if (ammoToReturn == null) return;

        ammoToReturn.SetActive(false);
        ammoToReturn.transform.parent = transform;
        _ammoPool.Push(ammoToReturn);
    }

    public GameObject GetAmmo()
    {
        if (_ammoPool.Count > 0)
        {
            GameObject ammo = _ammoPool.Pop();
            return ammo;
        }
        else
        {
            Debug.LogWarning("BulletPool Empty");
            return null;
        }
    }

    public int GetAvailableBulletCount()
    {
        return _ammoPool.Count;
    }
}
