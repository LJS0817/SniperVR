using TMPro;
using UnityEngine;

public class Tagger : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    GameObject _detectedObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _detectedObject = null;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.up * -1000f, Color.blue);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.up * -1, out hit, float.MaxValue))
        {
            if (hit.collider)
            {
                float dist = Mathf.Round((hit.transform.position - transform.position).magnitude);
                text.text = dist + " m";
                if (hit.transform.tag == "Taggable")
                {
                    if (_detectedObject == null) _detectedObject = hit.transform.gameObject;
                    else if (!GameObject.ReferenceEquals(_detectedObject, hit.transform.gameObject))
                    {
                        _detectedObject.GetComponent<TaggableObject>().SetDistance(-1); //#
                        _detectedObject = hit.transform.gameObject; 
                    }
                    TaggableObject t = hit.transform.GetComponent<TaggableObject>();
                    if (!t.HasTag()) t.SetTag(TagPool.Instance.GetTag(), transform);
                    t.SetDistance((int)dist);                                           //#
                } else if (_detectedObject != null) 
                {
                    _detectedObject.GetComponent<TaggableObject>().SetDistance(-1);     //#
                    _detectedObject = null; 
                }
            }
        }
    }
}
