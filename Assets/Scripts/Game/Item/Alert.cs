using UnityEngine;

public class Alert : MonoBehaviour
{
    [SerializeField] bool _isActivate;
    Material _alertLightMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isActivate = false;
        _alertLightMaterial = transform.GetChild(2).GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if(_isActivate)
        {

        }
    }
}
