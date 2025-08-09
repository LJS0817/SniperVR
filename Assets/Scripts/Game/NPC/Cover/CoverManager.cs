using UnityEngine;
using System.Collections.Generic;

public class CoverManager : MonoBehaviour
{
    public static CoverManager Instance { get; private set; }

    private CoverPoint[] allCoverPoints;

    private Dictionary<int, GameObject> occupiedCovers = new Dictionary<int, GameObject>();

    [SerializeField] List<GameObject> _coverObjectMap;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        allCoverPoints = new CoverPoint[_coverObjectMap[0].transform.childCount];
        for (int i = 0; i < _coverObjectMap[0].transform.childCount; i++)
        {
            allCoverPoints[i] = _coverObjectMap[0].transform.GetChild(i).GetComponent<CoverPoint>();
        }
    }

    //// AI 주변의 엄폐물을 찾아주는 최적화된 함수
    //// center: 검색 중심 위치(AI 위치), radius: 검색 반경
    //public List<CoverPoint> GetNearbyAvailableCovers(Vector3 center, float radius)
    //{
    //    List<CoverPoint> nearbyCovers = new List<CoverPoint>();
    //    foreach (CoverPoint cover in allCoverPoints)
    //    {
    //        // 1. 점유되지 않았고
    //        // 2. 검색 반경 내에 있는 엄폐물만 필터링
    //        if (!IsOccupied(cover) && Vector3.Distance(center, cover.transform.position) <= radius)
    //        {
    //            nearbyCovers.Add(cover);
    //        }
    //    }
    //    return nearbyCovers;
    //}

    // 엄폐 지점 점유 상태 설정/해제
    public void OccupyCover(CoverPoint cover, GameObject agent)
    {
        occupiedCovers[cover.GetInstanceID()] = agent;
    }

    public void VacateCover(CoverPoint cover)
    {
        if (occupiedCovers.ContainsKey(cover.GetInstanceID()))
        {
            occupiedCovers.Remove(cover.GetInstanceID());
        }
    }

    public bool IsOccupied(CoverPoint cover)
    {
        return occupiedCovers.ContainsKey(cover.GetInstanceID());
    }
}