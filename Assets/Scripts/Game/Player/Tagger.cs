using TMPro;
using UnityEngine;

public class Tagger : MonoBehaviour
{
    [SerializeField] TextMeshPro text;

    [SerializeField] Transform HitImage;
    [SerializeField] Transform _firePoint;
    [SerializeField] Transform _rayForward;
    [SerializeField] float f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
                float dist = Vector3.Distance(hit.transform.position, _firePoint.position);
                text.text = Mathf.Round(dist) + " m";

                /////////////////////////////////////////////////////////////////////////////
                Vector3 hitPoint = hit.point;
                float timeToTarget = dist / f;
                
                Vector3 gravity = 0.5f * Physics.gravity * timeToTarget * timeToTarget;

                Vector3 windDeflection = Vector3.zero;
                if (WindController.Instance != null)
                {
                    Vector3 windForce = WindController.Instance.GetWindForceAtPosition();
                    windDeflection = 0.5f * windForce * timeToTarget * timeToTarget;
                }

                //_predictedFinalPosition = hitPoint + windDeflection;

                //HitImage.position = hitPoint;
                HitImage.position = _firePoint.position + _firePoint.forward * dist + gravity + windDeflection;

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
