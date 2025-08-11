using System.Security.Cryptography;
using UnityEngine;

public class CoverPoint : MonoBehaviour
{
    public enum CoverType { High_Edge, Low_Center }
    public CoverType type = CoverType.High_Edge;

    public enum BLOCKED_COVER_DIRECTION
    {
        E_NONE,
        E_LEFT,
        E_TOP,
        E_RIGHT,
        E_BOTTOM
    }
    [SerializeField] BLOCKED_COVER_DIRECTION _blockedDir;

    Rect _rect;
    float _padding = 0.5f;
    Vector3 _halfScale;

    private void Start()
    {
        _halfScale = transform.lossyScale * 0.5f;
        Vector3 position = transform.position;
        _rect = new Rect();

        // Left
        _rect.xMin = position.x - _halfScale.x;
        
        // Top
        _rect.yMin = position.z + _halfScale.z;

        // Right
        _rect.xMax = position.x + _halfScale.x;

        // Bottom
        _rect.yMax = position.z - _halfScale.z;

        DebugRectPosition(position);
    }

    public Vector3 GetNearestCoverPosition(Vector3 Pos, out float coverValue, out BLOCKED_COVER_DIRECTION coverDir)
    {
        Vector3 dir = Pos - transform.position;
        Debug.Log(dir);
        Vector3 rst = getSinglePosition(dir, out int coverValue0, out coverDir);
        rst += getAdditionalSinglePosition(dir, out int coverValue1);
        coverValue = coverValue0 * coverValue1;
        //if (_halfScale.x - Mathf.Abs(dir.x) < _halfScale.y - Mathf.Abs(dir.z))
        //{
        //    if (dir.x > 0) Debug.Log("Right Side");
        //    else Debug.Log("Left Side");
        //} else
        //{
        //    if (dir.z > 0) Debug.Log("Top Side");
        //    else Debug.Log("Bottom Side");
        //}

        return rst;
    }

    Vector3 getSinglePosition(Vector3 dir, out int coverValue, out BLOCKED_COVER_DIRECTION coverDir)
    {
        Vector3 rst = Vector3.zero;
        if (_halfScale.x - Mathf.Abs(dir.x) < _halfScale.y - Mathf.Abs(dir.z))
        {
            if ((dir.x > 0 && _blockedDir != BLOCKED_COVER_DIRECTION.E_RIGHT) || _blockedDir == BLOCKED_COVER_DIRECTION.E_LEFT) 
            { 
                rst.x = _rect.xMax + _padding;
                coverValue = -1;
                coverDir = BLOCKED_COVER_DIRECTION.E_RIGHT;
            }
            else
            { 
                rst.x = _rect.xMin - _padding;
                coverValue = 1;
                coverDir = BLOCKED_COVER_DIRECTION.E_LEFT;
            }
        }
        else
        {
            if ((dir.z > 0 && _blockedDir != BLOCKED_COVER_DIRECTION.E_TOP) || _blockedDir == BLOCKED_COVER_DIRECTION.E_BOTTOM)
            {
                rst.z = _rect.yMin + _padding;
                coverValue = 1;
                coverDir = BLOCKED_COVER_DIRECTION.E_TOP;
            }
            else
            {
                rst.z = _rect.yMax - _padding;
                coverValue = -1;
                coverDir = BLOCKED_COVER_DIRECTION.E_BOTTOM;
            }
        }
        return rst;
    }

    Vector3 getAdditionalSinglePosition(Vector3 dir, out int coverValue)
    {
        Vector3 rst = Vector3.zero;
        if (_halfScale.x - Mathf.Abs(dir.x) < _halfScale.y - Mathf.Abs(dir.z))
        {
            if ((dir.z > 0 && _blockedDir != BLOCKED_COVER_DIRECTION.E_TOP) || _blockedDir != BLOCKED_COVER_DIRECTION.E_BOTTOM) 
            { 
                rst.z = _rect.yMin - _padding;
                coverValue = 1;
            }
            else
            { 
                rst.z = _rect.yMax + _padding;
                coverValue = -1;
            }
        }
        else
        {
            if ((dir.x > 0 && _blockedDir != BLOCKED_COVER_DIRECTION.E_RIGHT) || _blockedDir == BLOCKED_COVER_DIRECTION.E_LEFT) 
            { 
                rst.x = _rect.xMax - _padding;
                coverValue = -1;
            }
            else
            {
                rst.x = _rect.xMin + _padding;
                coverValue = 1;
            }
        }
        return rst;
    }

    void DebugRectPosition(Vector3 position)
    {
        //GameObject obj = new GameObject("LeftTop");
        //obj.transform.position = new Vector3(_rect.xMin, position.y, _rect.yMin);
        //obj.transform.parent = transform;

        //obj = new GameObject("RightTop");
        //obj.transform.position = new Vector3(_rect.xMax, position.y, _rect.yMin);
        //obj.transform.parent = transform;

        //obj = new GameObject("LeftBottom");
        //obj.transform.position = new Vector3(_rect.xMin, position.y, _rect.yMax);
        //obj.transform.parent = transform;

        //obj = new GameObject("RightBottom");
        //obj.transform.position = new Vector3(_rect.xMax, position.y, _rect.yMax);
        //obj.transform.parent = transform;
    }

    private void OnDrawGizmos()
    {
        // 타입에 따라 기즈모 색상이나 모양을 다르게 표시하면 구분이 쉬움
        Gizmos.color = (type == CoverType.High_Edge) ? Color.cyan : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.4f);
        Gizmos.DrawRay(transform.position, transform.forward * 1.0f);
    }
}