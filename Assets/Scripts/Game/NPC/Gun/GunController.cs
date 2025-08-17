using UnityEngine;

public class GunController : MonoBehaviour
{
    public enum GUN_TYPE { E_NONE, E_RIFLE, E_SNIPER }
    public enum POSE_TYPE { E_IDLE, E_STAND, E_CROUCH, E_CRAWL }
    public enum GUN_STATE { E_NONE, E_AIM, E_AIMED, E_FIRE, E_RELOAD }
    [SerializeField] GUN_TYPE _gunType;
    [SerializeField] POSE_TYPE _poseType;
    [SerializeField] GUN_STATE _gunState;
    [SerializeField] Transform _gunParent;
    Gun _gun;
    Animator _ani;

    public float _aimTime;
    float _aimSpeed;

    [SerializeField] Transform _target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _ani = GetComponent<Animator>();
        _ani.SetInteger("Pose", (int)_poseType - 1);

        _gun = _gunParent.GetChild(((int)_gunType) - 1).GetComponent<Gun>();
        _gun.gameObject.SetActive(true);

        _aimSpeed = 0.35f;

        if (_gunType == GUN_TYPE.E_SNIPER)
        {
            transform.localEulerAngles = new Vector3(90, transform.localEulerAngles.y, transform.localEulerAngles.z);
            Vector3 headAngle = transform.GetChild(1).localEulerAngles;
            headAngle.x -= 90;
            transform.GetChild(1).localEulerAngles = headAngle;
            Debug.Log(transform.localEulerAngles);
        }
    }

    private void Update()
    {
        Debug.DrawRay(_gun.GetFirePoint(), _gun.GetFireDirection() * 1000f, Color.aliceBlue);
        if (_gunState == GUN_STATE.E_FIRE && _gun.CanFire())
        {
            _gun.Fire();
            SetTarget(_target);
        }
        if (_target != null && _gunState == GUN_STATE.E_AIM)
        {
            Debug.Log("ASKXZC");
            _aimTime += Time.deltaTime * _aimSpeed;
            Vector3 dir = _target.position - _gun.GetFirePoint();
            _gun.transform.rotation = Quaternion.Lerp(_gun.transform.rotation, Quaternion.LookRotation(-dir), _aimTime);
            if (Physics.Raycast(_gun.GetFirePoint(), _gun.GetFireDirection(), out RaycastHit hit, 500f))
            {
                if (hit.collider != null)
                {
                    Debug.Log(hit.transform.name);
                    if (hit.transform.gameObject.layer == 7)
                    {
                        Debug.Log("TERST");
                        SetTarget(null);
                        return;
                    } else if (hit.transform.gameObject.layer == _target.gameObject.layer)
                    {
                        _aimTime = 1f;
                        _target = hit.transform;
                        _gunState = GUN_STATE.E_AIMED;
                    }
                    //Fire();
                } else if (_aimTime >= 1f)
                {
                    Debug.Log("ASDJOZXCIOUWQE");
                    SetTarget(null);
                } else
                {
                    Debug.Log("12345");
                }
            }
            else if (_target != null && _aimTime >= 1f)
            {
                Debug.Log("4235rfdsz5");
                SetTarget(null);
            }
        }
    }

    public bool FollowingTarget()
    {
        return _target != null;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        if (target == null)
        {
            _aimTime = 1f;
            _gunState = GUN_STATE.E_NONE;
        }
        else
        {
            _aimTime = 0f;
            _gunState = GUN_STATE.E_AIM;
            Debug.Log("ASDJOZXCIOUWQE");
        }
    }

    public void Fire()
    {
        _gunState = GUN_STATE.E_FIRE;
    }

    public bool LookAtTarget() { return _gunState == GUN_STATE.E_AIM || _gunState == GUN_STATE.E_AIMED; }
    public bool MissedTarget() { return _gunState == GUN_STATE.E_NONE || _target == null; }

    public void Dead()
    {
        _gunState = GUN_STATE.E_NONE;
        _target = null;
        _gun.enabled = false;
    }
}
