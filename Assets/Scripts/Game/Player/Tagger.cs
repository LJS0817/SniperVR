using TMPro;
using UnityEngine;

public class Tagger : MonoBehaviour
{
    [SerializeField] TextMeshPro text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
                    TaggableObject t = hit.transform.GetComponent<TaggableObject>();
                    if (!t.HasTag()) t.SetTag(TagPool.Instance.GetTag(), transform);
                    t.SetDistance((int)dist);                                           //#
                }
            }
        }
    }
}
