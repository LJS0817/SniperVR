using UnityEngine;

public class Player : MonoBehaviour
{
    public Rifle Gun;
    ArduinoConnection _arduino;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _arduino = GetComponent<ArduinoConnection>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
