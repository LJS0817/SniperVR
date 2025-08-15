using Unity.VisualScripting;
using UnityEngine;
using static NPC;

public class NPCController : MonoBehaviour
{
    DetectionIndicator _indicator;
    CoverController _cover;
    GunController _gunController;
    EnemyDetector _detector;
    FieldOfView _fov;

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
                _detector.DetectEnemy(_fov.Targets[2]);
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
            case NPC_STATE.E_ATTACK:
                _gunController.Fire();
                ChangeNPCState(NPC_STATE.E_ATTACKING);
                break;
            case NPC_STATE.E_ATTACKING:
                //_gunController.Fire();
                //_npcState.SetState(NPC_STATE.E_ATTACKING);
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

    void onDead()
    {
        _cover.Dead();
        _gunController.enabled = false;
        _fov.enabled = false;
        enabled = false;
    }
}
