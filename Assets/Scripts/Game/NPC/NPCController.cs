using UnityEngine;
using static GameCharacter;

public class NPCController : MonoBehaviour
{
    DetectionIndicator _indicator;
    CoverController _cover;
    GunController _gunController;
    EnemyDetector _detector;
    FieldOfView _fov;
    TaggableObject _tag;

    ChangeState _event;
    NPC_STATE _state;

    public void AddEvent(ChangeState callback)
    {
        _event += callback;
    }

    private void Start()
    {
        _fov = transform.GetChild(1).GetComponent<FieldOfView>();
        _indicator = GetComponent<DetectionIndicator>();
        _detector = GetComponent<EnemyDetector>();
        _cover = GetComponent<CoverController>();
        _gunController = GetComponent<GunController>();
        AddEvent((NPC_STATE state) => { _state = state; });
    }

    private void Update()
    {
        switch (_state)
        {
            case NPC_STATE.E_SEARCH:
                if(_detector.DetectEnemy(_fov.Targets[2]))
                {
                    _gunController.SetTarget(_detector.GetTarget());
                    ChangeNPCState(NPC_STATE.E_AIMING);
                } else if(_gunController.FollowingTarget()) _gunController.SetTarget(null);
                else if (_cover.FindDeadFrined(_fov.Targets[1]))
                { 
                    //ChangeNPCState(NPC_STATE.E_COVER);
                }
                break;
            case NPC_STATE.E_CHASE:
                _cover.SeekCover(_fov.Targets[0], _detector.GetTargetPos());
                ChangeNPCState(NPC_STATE.E_COVER);
                break;
            case NPC_STATE.E_COVER:
                _cover.ReachDestnation();
                break;
            case NPC_STATE.E_PEEK:
                _cover.Peeking();
                break;
            case NPC_STATE.E_AIMING:
                if(_indicator.FindTarget())
                {
                    ChangeNPCState(NPC_STATE.E_ATTACK);
                }
                
                if (_gunController.LookAtTarget()) _indicator.Seek();
                else if(_gunController.MissedTarget())
                {
                    _indicator.ResetIndicator();
                    ChangeNPCState(NPC_STATE.E_SEARCH);
                }
                break;
            case NPC_STATE.E_ATTACK:
                if (_gunController.MissedTarget())
                {
                    Debug.Log("MISSSING");
                    _indicator.ResetIndicator();
                    ChangeNPCState(NPC_STATE.E_SEARCH);
                } else if (_gunController.LookAtTarget())
                {
                    Debug.Log("Fire");
                    _gunController.Fire();
                }
                break;
            case NPC_STATE.E_DEAD:
                onDead();
                break;
        }
    }

    public void Init(Transform player, RectTransform img, Transform canvas, ref Camera cam)
    {
        _indicator.SetIndicatorImage(img, canvas, ref cam);
        _cover.SetPlayerTransform(player);
    }

    public void ChangeNPCState(NPC_STATE state)
    {
        _event(state);
    }

    public void SetTag(Transform tag)
    {
        _tag = tag.GetComponent<TaggableObject>();
    }

    void onDead()
    {
        _cover.Dead();
        _indicator.Dead();
        _tag.Dead();
        _gunController.Dead();
        _fov.Dead();
        transform.name = "_";
        enabled = false;
    }
}
