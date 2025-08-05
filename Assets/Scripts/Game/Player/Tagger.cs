using TMPro;
using UnityEngine;

public class Tagger : MonoBehaviour
{
    [SerializeField] TextMeshPro text;

    [SerializeField] Transform HitImage;
    [SerializeField] Transform _firePoint;
    [SerializeField] Transform _rayForward;
    [SerializeField] float f;
    Vector3 _distFirePoint_Scope;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _distFirePoint_Scope = transform.localPosition - _firePoint.localPosition;
        Debug.Log(_distFirePoint_Scope);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(_rayForward.position, _rayForward.forward * 1000f, Color.blue);
        RaycastHit hit;
        if(Physics.Raycast(_rayForward.position, _rayForward.forward, out hit, 1000f))
        {
            if (hit.collider)
            {
                float dist = Mathf.Round((hit.transform.position - _firePoint.position).magnitude);
                text.text = dist + " m";

                /////////////////////////////////////////////////////////////////////////////
                _distFirePoint_Scope = _rayForward.localPosition - _firePoint.localPosition;
                Vector3 hitPoint = hit.point;
                hitPoint.y -= _distFirePoint_Scope.y;
                float timeToTarget = (dist + _distFirePoint_Scope.z) / f;
                //Debug.Log(dist + _distFirePoint_Scope.z);
                //hitPoint.y += (0.5f * Physics.gravity.y * timeToTarget * timeToTarget);

                Vector3 windDeflection = Vector3.zero;
                if (WindController.Instance != null)
                {
                    Vector3 windForce = WindController.Instance.GetWindForceAtPosition(hitPoint);
                    Vector3 windAcceleration = windForce;
                    windDeflection = 0.5f * windAcceleration * timeToTarget * timeToTarget;
                }

                //_predictedFinalPosition = hitPoint + windDeflection;
                HitImage.position = hitPoint;

                //Debug.Log($"예측된 최종 위치: {_predictedFinalPosition}");

                /////////////////////////////////////////////////////////////////////////////
                if (hit.transform.tag == "Taggable")
                {
                    TaggableObject t = hit.transform.GetComponent<TaggableObject>();
                    if (!t.HasTag()) t.SetTag(TagPool.Instance.GetTag(), transform);
                    t.SetDistance((int)dist);
                }
            }
        }
    }
}
