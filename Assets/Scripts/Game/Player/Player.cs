using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    public Rifle Gun;

    public InputActionReference Trigger;

    //ArduinoConnection _arduino;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Trigger.action.started += triggerAction;
        Trigger.action.performed += triggerAction;
        Trigger.action.canceled += triggerAction;
        //_arduino = GetComponent<ArduinoConnection>();
    }

    private void OnDestroy()
    {
        Trigger.action.started -= triggerAction;
        Trigger.action.performed -= triggerAction;  
        Trigger.action.canceled -= triggerAction;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void triggerAction(InputAction.CallbackContext cxt)
    {
        float triggerValue = Trigger.action.ReadValue<float>();
        Debug.Log("Trigger Value: " + triggerValue);
        Gun.SetTriggerState(triggerValue);
    }
}
