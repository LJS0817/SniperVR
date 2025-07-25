using UnityEngine;

public class Tagger : MonoBehaviour
{
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
            if(hit.collider)
            {
                Debug.Log(hit.transform.name + " Distance  : " + Mathf.Round((hit.transform.position - transform.position).magnitude) + " m");
            }
        }
    }
}
