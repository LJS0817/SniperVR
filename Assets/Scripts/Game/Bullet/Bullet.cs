using UnityEngine;

public class Bullet : MonoBehaviour
{
    Transform _firePoint;
    Rigidbody _rig;

    private void Start()
    {
        
    }

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

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
    }

    public void Fire()
    {
        transform.position = _firePoint.position;
        _rig.angularVelocity = Vector3.zero;
        _rig.linearVelocity = Vector3.zero;
        _rig.AddForce(_firePoint.forward * 8f, ForceMode.Impulse);
    }
}
