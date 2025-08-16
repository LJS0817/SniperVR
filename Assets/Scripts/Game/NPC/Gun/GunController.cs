using UnityEngine;

public class GunController : MonoBehaviour
{
    public enum GUN_TYPE { E_NONE, E_RIFLE, E_SNIPER }
    public enum POSE_TYPE { E_IDLE, E_STAND, E_CROUCH, E_CRAWL }
    public enum GUN_STATE { E_NONE, E_AIM, E_FIRE, E_RELOAD }
    [SerializeField] GUN_TYPE _gunType;
    [SerializeField] POSE_TYPE _poseType;
    [SerializeField] GUN_STATE _gunState;
    [SerializeField] Transform _gunParent;
    Gun _gun;
    Animator _ani;

    [SerializeField] Transform _target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _ani = GetComponent<Animator>();
        _ani.SetInteger("Pose", (int)_poseType - 1);

        _gun = _gunParent.GetChild(((int)_gunType) - 1).GetComponent<Gun>();
        _gun.gameObject.SetActive(true);

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
        if(_target != null && _gunState == GUN_STATE.E_AIM)
        {
            Vector3 dir = _target.position - _gun.GetFirePoint();
            _gun.transform.rotation = Quaternion.LookRotation(-dir);
            Debug.DrawRay(_gun.GetFirePoint(), _gun.GetFireDirection() * 1000f, Color.aliceBlue);
            if(Physics.Raycast(_gun.GetFirePoint(), _gun.GetFireDirection(), out RaycastHit hit, 500f))
            {
                Debug.Log(hit.transform.name);
                if (hit.collider != null && hit.transform.gameObject.layer == _target.gameObject.layer)
                {
                    Debug.Log("ASDSA");
                    Fire();
                }
            }
        }
        if (_gunState == GUN_STATE.E_FIRE && !_gun.IsEmptyAmmo())
        {
            _gun.Fire();
        }
    }

    public bool FollowingTarget()
    {
        return _target != null;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        _gunState = GUN_STATE.E_AIM;
    }

    public bool Fire()
    {
        _gunState = GUN_STATE.E_FIRE;
        return true;
    }

    public void Dead()
    {
        _gunState = GUN_STATE.E_NONE;
        _target = null;
        _gun.enabled = false;
    }
}
