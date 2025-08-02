using TMPro;
using UnityEngine;

public class Tagger : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

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
                text.text = Mathf.Round((hit.transform.position - transform.position).magnitude) + " m";
                //Debug.Log(hit.transform.name + " Distance  : " + text.text);

            }
        }
    }
}
