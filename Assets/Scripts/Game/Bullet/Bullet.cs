using UnityEngine;

public class Bullet : MonoBehaviour
{
    Transform _firePoint;
    Rigidbody _rig;

    public void Init(Transform point)
    {
        _firePoint = point;
        gameObject.SetActive(false);
        if(_rig == null) _rig = GetComponent<Rigidbody>();
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
        BulletPool.Instance.SetHitSign(collision.contacts[0].point);
        if(collision.transform.parent != null && collision.transform.parent.CompareTag("Enemy"))
        {
            collision.transform.GetComponent<MeshRenderer>().material.color = Color.red;
            collision.transform.parent.GetComponent<GameCharacter>().Attacked(100);
        } else if(collision.transform.TryGetComponent<GameCharacter>(out GameCharacter ch))
        {
            ch.Attacked(100);
        }
        gameObject.SetActive(false);
    }

    public void Fire(bool fromNPC)
    {
        if(fromNPC && _rig.useGravity) _rig.useGravity = false;
        else if (!fromNPC && !_rig.useGravity) _rig.useGravity = true;

        transform.position = _firePoint.position;
        _rig.angularVelocity = Vector3.zero;
        _rig.linearVelocity = Vector3.zero;
        _rig.AddForce(_firePoint.forward * 8f, ForceMode.Impulse);
    }
}
