using UnityEngine;

public class CoverPoint : MonoBehaviour
{
    public enum CoverType { High_Edge, Low_Center }
    public CoverType type = CoverType.High_Edge;

    private void OnDrawGizmos()
    {
        // 타입에 따라 기즈모 색상이나 모양을 다르게 표시하면 구분이 쉬움
        Gizmos.color = (type == CoverType.High_Edge) ? Color.cyan : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.4f);
        Gizmos.DrawRay(transform.position, transform.forward * 1.0f);
    }
}