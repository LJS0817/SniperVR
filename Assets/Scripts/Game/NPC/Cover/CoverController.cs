using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class CoverController : MonoBehaviour
{
    public Transform playerTransform;
    //public float coverSearchRadius = 20f;
    public LayerMask coverLayer; // 엄폐물 레이어 (벽 등)
    public LayerMask playerLayer; // 플레이어 레이어

    private NavMeshAgent navAgent;

    public enum AIState { E_SEARCH, E_CHASE, E_COVER, E_PEEK, E_ATTACK, E_DEAD }
    public AIState currentState;

    [SerializeField] float hideSensitivity;

    FieldOfView _fov;

    void Start()
    {
        _fov = GetComponent<FieldOfView>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case AIState.E_SEARCH:
                // 순찰 로직
                break;
            case AIState.E_CHASE:
                SeekCover();
                currentState = AIState.E_COVER;
                break;
            case AIState.E_COVER:
                break;
            case AIState.E_PEEK:
                break;
            case AIState.E_ATTACK:
                break;
            case AIState.E_DEAD:
                break;
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

        Vector3 rst = playerPos - agentPos;
        rst.Normalize();
        rst *= -1;
        rst.x *= bestPoint.lossyScale.x * 0.5f;
        rst.z *= bestPoint.lossyScale.z * 0.5f;
        rst.y = agentPos.y;
        rst = bestPoint.position + rst;
        
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
            CoverManager.Instance.OccupyCover(bestCover.GetComponent<CoverPoint>(), gameObject);
            navAgent.SetDestination(getCoverPosition(bestCover));
        }
    }
}