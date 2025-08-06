using UnityEngine;

public class Bullet : MonoBehaviour
{
    Transform _firePoint;
    Rigidbody _rig;

    public void Init(Transform point)
    {
        _firePoint = point;
        gameObject.SetActive(false);
        _rig = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (WindController.Instance != null)
        {
            Vector3 windForce = WindController.Instance.GetWindForceAtPosition();
            _rig.AddForce(windForce, ForceMode.Force);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.transform.name);
        gameObject.SetActive(false);
        BulletPool.Instance.SetHitSign(collision.contacts[0].point);
    }

    public void Fire()
    {
        transform.position = _firePoint.position;
        _rig.angularVelocity = Vector3.zero;
        _rig.linearVelocity = Vector3.zero;
        _rig.AddForce(_firePoint.forward * 8f, ForceMode.Impulse);
    }
}
