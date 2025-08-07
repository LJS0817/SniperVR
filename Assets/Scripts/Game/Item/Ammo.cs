using UnityEngine;

public class Ammo : MonoBehaviour
{
    Transform _insertPoint;
    Vector3 _popDir;

    bool _used;
    bool _loaded;

    float _moveSpeed;

    Rigidbody _rig;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rig = GetComponent<Rigidbody>();
        _used = false;
        _loaded = false;
        _moveSpeed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(_loaded)
        {
            Vector3 dir = _insertPoint.position - transform.position;
            float dist = dir.magnitude;
            _moveSpeed = 5f * Mathf.Clamp01(dist / 0.1f) + 0.1f;
            if (dist < 0.01f)
            {
                transform.localPosition = Vector3.zero;
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0, 0);
                _loaded = false;
            }
            else if (dist < 0.03f)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * _moveSpeed);
            } 
            else if(dist < 0.06f)
            {
                float angleRadians = Mathf.Atan2(dir.x, dir.z);
                float angleDegrees = angleRadians * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(transform.localEulerAngles.x, angleDegrees, transform.localEulerAngles.z);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * 5f);
            }
            _moveSpeed = dist;
        }
    }

    public bool isAvaliable()
    {
        return !_used;
    }

    public void Fire()
    {
        _used = true;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Init(Transform p, Vector3 pop)
    {
        _insertPoint = p;
        _popDir = pop;
        gameObject.SetActive(false);
    }

    public void LoadInMagazine(Transform parent, Vector3 pos)
    {
        transform.parent = parent;
        transform.localPosition = pos;
        transform.localRotation = Quaternion.Euler(-90, 0, 0);
        _loaded = false;
        _used = false;
        transform.GetChild(0).gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void LoadAmmo(Transform parent)
    {
        _loaded = true;
        transform.parent = parent;
        _moveSpeed = (_insertPoint.position - transform.position).magnitude;
        //transform.localPosition = Vector3.zero;
    }

    public void PopOut()
    {
        _rig.AddForce(_popDir * 5f, ForceMode.Impulse);
        _rig.useGravity = true;
        _loaded = false;
    }
}
