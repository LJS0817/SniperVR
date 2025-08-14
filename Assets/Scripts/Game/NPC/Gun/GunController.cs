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
        if (_gunState == GUN_STATE.E_FIRE && !_gun.IsEmptyAmmo())
        {
            _gun.Fire();
        }
    }

    public void Fire()
    {
        _gunState = GUN_STATE.E_FIRE;
    }
}
