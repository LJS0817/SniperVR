using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Rifle Gun;

    public InputActionReference Trigger;
    public InputActionReference Reload;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Trigger.action.started += triggerAction;
        Trigger.action.performed += triggerAction;
        Trigger.action.canceled += triggerAction;

        Reload.action.started += reloadAction;
    }

    private void OnDestroy()
    {
        Trigger.action.started -= triggerAction;
        Trigger.action.performed -= triggerAction;  
        Trigger.action.canceled -= triggerAction;

        Reload.action.started -= reloadAction;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            Gun.transform.localPosition = Vector3.zero;
        }
    }

    void triggerAction(InputAction.CallbackContext cxt)
    {
        float triggerValue = Trigger.action.ReadValue<float>();
        Debug.Log("Trigger Value: " + triggerValue);
        Gun.SetTriggerState(triggerValue);
    }

    void reloadAction(InputAction.CallbackContext cxt)
    {
        Debug.Log("Reload");
        Gun.Reload();
    }
}
