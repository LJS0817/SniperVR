using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CoverController : MonoBehaviour
{
    Transform _player;
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

    private NavMeshAgent _navAgent;

    NPCController _controller;

    public void SetPlayerTransform(Transform p)
    {
        _player = p;
    }

    void Start()
    {
        _coverTime = 0f;
        _peekTime = 0f;
        _peekTargetTimeValue = 1;
        _startPos = transform.position;
        _navAgent = GetComponent<NavMeshAgent>();
        _controller = GetComponent<NPCController>();
    }

    private void Update()
    {
        
    }

    void resetPosition()
    {
        _coverTime += Time.deltaTime;
        if(_coverTime > 25f)
        {
            _navAgent.SetDestination(_startPos);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0f);
            _coverTime = 0f;
            _controller.ChangeNPCState(NPC.NPC_STATE.E_SEARCH);
        }
    }

    public void ReachDestnation()
    {
        if (!_navAgent.pathPending)
        {
            if (_navAgent.remainingDistance <= _navAgent.stoppingDistance)
            {
                if (!_navAgent.hasPath || _navAgent.velocity.sqrMagnitude == 0f)
                {
                    transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, getYRotationValue(), 0f);
                    _controller.ChangeNPCState(NPC.NPC_STATE.E_PEEK);
                }
            }
        }
    }

    public void Dead()
    {
        _navAgent.enabled = false;
        enabled = false;
    }

    float getYRotationValue()
    {
        if (_coverDir == CoverPoint.BLOCKED_COVER_DIRECTION.E_LEFT) return 90f;
        else if (_coverDir == CoverPoint.BLOCKED_COVER_DIRECTION.E_BOTTOM) return 0f;
        else if (_coverDir == CoverPoint.BLOCKED_COVER_DIRECTION.E_RIGHT) return 270f;
        else return 180f;
    }

    public void Peeking()
    {
        _peekTime += Time.deltaTime * _peekTargetTimeValue;
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, getYRotationValue(), (Mathf.Lerp(transform.localEulerAngles.z, _peekValue, _peekTime)));
        //transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y, (Mathf.Lerp(transform.localRotation.z, _peekValue, _peekTime)));

        if ((_peekTime >= 2f && _peekTargetTimeValue > 0) || (_peekTime <= -1f && _peekTargetTimeValue < 0)) 
        {
            _peekTargetTimeValue *= -1;
        }

        resetPosition();
    }

    // 최적의 엄폐 지점을 찾는 함수
    private Transform FindBestCoverInList(List<CoverPoint> covers, Vector3 targetPos)
    {
        Transform bestPoint = null;
        float minSqrDistance = float.MaxValue; // 제곱 거리로 비교
        Vector3 agentPos = transform.position;

        for(int i = 0; i < covers.Count; i++)
        {
            // 플레이어 시야 체크
            Vector3 coverPos = covers[i].transform.position;
            Vector3 dirToPlayer = (targetPos - coverPos).normalized;
            if (targetPos == Vector3.zero || !Physics.Raycast(coverPos, dirToPlayer, 500f, playerLayer))
            {
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
        Vector3 rst = _player.position - transform.position;
        rst.Normalize();
        rst *= -1;
        rst.x *= bestPoint.lossyScale.x * 0.5f;
        rst.z *= bestPoint.lossyScale.z * 0.5f;
        rst.y = transform.position.y;
        rst = bestPoint.position + rst;
        return rst;
    }

    // 이 함수를 AI의 상태 머신에서 호출하여 엄폐를 시작
    public void SeekCover(List<Transform> list, Vector3 targetPos)
    {
        _coverTime = 0f;
        List<CoverPoint> nearbyCovers = new List<CoverPoint>();

        for (int i = 0; i < list.Count; i++)
        {
            if(list[i].gameObject.layer == 7)
            {
                nearbyCovers.Add(list[i].GetComponent<CoverPoint>());
            }
        }

        Transform bestCover = FindBestCoverInList(nearbyCovers, targetPos);

        if (bestCover != null)
        {
            CoverPoint point = bestCover.GetComponent<CoverPoint>();
            CoverManager.Instance.OccupyCover(point, gameObject);
            _navAgent.SetDestination(point.GetNearestCoverPosition(getCoverPosition(bestCover), out _peekValue, out _coverDir));
            _peekValue *= MAX_PEEKING_VALUE;
        }
    }
}