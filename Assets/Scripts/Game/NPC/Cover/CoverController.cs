using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static NPC;

public class CoverController : MonoBehaviour
{
    public Transform playerTransform;
    //public float coverSearchRadius = 20f;
    public LayerMask coverLayer; // 엄폐물 레이어 (벽 등)
    public LayerMask playerLayer; // 플레이어 레이어
    Vector3 _startPos;
    float _coverTime;
    const float MAX_PEEKING_VALUE = 25f;

    float _peekValue;
    float _peekTime;
    int _peekTargetTimeValue;

    CoverPoint.BLOCKED_COVER_DIRECTION _coverDir;

    private NavMeshAgent navAgent;

    NPC _npcState;

    [SerializeField] float hideSensitivity;

    FieldOfView _fov;

    void Start()
    {
        _npcState = GetComponent<NPC>();
        _coverTime = 0f;
        _peekTime = 0f;
        _peekTargetTimeValue = 1;
        _startPos = transform.position;
        _fov = transform.GetChild(1).GetComponent<FieldOfView>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        switch (_npcState.GetState())
        {
            case NPC_STATE.E_SEARCH:
                // 순찰 로직
                break;
            case NPC_STATE.E_CHASE:
                SeekCover();
                _npcState.SetState(NPC_STATE.E_COVER);
                break;
            case NPC_STATE.E_COVER:
                reachDestnation();
                break;
            case NPC_STATE.E_PEEK:
                peeking();
                resetPosition();
                break;
            case NPC_STATE.E_ATTACK:
                break;
            case NPC_STATE.E_DEAD:
                break;
        }
    }

    void resetPosition()
    {
        _coverTime += Time.deltaTime;
        if(_coverTime > 25f)
        {
            navAgent.SetDestination(_startPos);
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, 0f);
            _coverTime = 0f;
            _npcState.SetState(NPC_STATE.E_SEARCH);
        }
    }

    void reachDestnation()
    {
        if (!navAgent.pathPending)
        {
            if (navAgent.remainingDistance <= navAgent.stoppingDistance)
            {
                if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f)
                {
                    transform.localRotation = Quaternion.Euler(transform.localRotation.x, getYRotationValue(), 0f);
                    _npcState.SetState(NPC_STATE.E_PEEK);
                }
            }
        }
    }

    float getYRotationValue()
    {
        if (_coverDir == CoverPoint.BLOCKED_COVER_DIRECTION.E_LEFT) return 90f;
        else if (_coverDir == CoverPoint.BLOCKED_COVER_DIRECTION.E_BOTTOM) return 0f;
        else if (_coverDir == CoverPoint.BLOCKED_COVER_DIRECTION.E_RIGHT) return 270f;
        else return 180f;
    }

    void peeking()
    {
        _peekTime += Time.deltaTime * _peekTargetTimeValue;
        transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, (Mathf.Lerp(transform.localRotation.z, _peekValue, _peekTime)));

        if ((_peekTime >= 2f && _peekTargetTimeValue > 0) || (_peekTime <= -1f && _peekTargetTimeValue < 0)) 
        {
            _peekTargetTimeValue *= -1;
        }
    }

    // 최적의 엄폐 지점을 찾는 함수
    private Transform FindBestCoverInList(List<CoverPoint> covers)
    {
        Transform bestPoint = null;
        float minSqrDistance = float.MaxValue; // 제곱 거리로 비교

        // 플레이어 위치 캐싱 (루프 내에서 반복 접근 방지)
        Vector3 playerPos = playerTransform.position;
        Vector3 agentPos = transform.position;

        for(int i = 0; i < covers.Count; i++)
        {
            // 플레이어 시야 체크
            Vector3 coverPos = covers[i].transform.position;
            Vector3 dirToPlayer = (playerPos - coverPos).normalized;
            if (!Physics.Raycast(coverPos, dirToPlayer, 100f, playerLayer))
            {
                // 제곱 거리를 사용하여 비교 (더 빠름)
                float sqrDistance = (agentPos - coverPos).sqrMagnitude;

                if (sqrDistance < minSqrDistance)
                {
                    minSqrDistance = sqrDistance;
                    bestPoint = covers[i].transform;
                }
            }
        }
        
        return bestPoint;
    }

    Vector3 getCoverPosition(Transform bestPoint)
    {
        Vector3 rst = playerTransform.position - transform.position;
        rst.Normalize();
        rst *= -1;
        rst.x *= bestPoint.lossyScale.x * 0.5f;
        rst.z *= bestPoint.lossyScale.z * 0.5f;
        rst.y = transform.position.y;
        rst = bestPoint.position + rst;
        return rst;
    }

    // 이 함수를 AI의 상태 머신에서 호출하여 엄폐를 시작
    public void SeekCover()
    {
        _coverTime = 0f;
        List<CoverPoint> nearbyCovers = new List<CoverPoint>();
        for (int i = 0; i < _fov.Targets.Count; i++)
        {
            if(_fov.Targets[i].gameObject.layer == 7)
            {
                nearbyCovers.Add(_fov.Targets[i].GetComponent<CoverPoint>());
            }
        }

        Transform bestCover = FindBestCoverInList(nearbyCovers); // 2. 필터링된 리스트 내에서 최적 지점 계산

        if (bestCover != null)
        {
            CoverPoint point = bestCover.GetComponent<CoverPoint>();
            CoverManager.Instance.OccupyCover(point, gameObject);
            navAgent.SetDestination(point.GetNearestCoverPosition(getCoverPosition(bestCover), out _peekValue, out _coverDir));
            _peekValue *= MAX_PEEKING_VALUE;
        }
    }
}