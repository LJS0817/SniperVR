using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpotChanger : MonoBehaviour
{
    int _curIndex;
    [SerializeField] List<Transform> listPos;
    public InputActionReference ChangeButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _curIndex = 0;
        ChangeButton.action.started += changeAction;
    }

    private void OnDestroy()
    {
        ChangeButton.action.started -= changeAction;
    }

    void changeAction(InputAction.CallbackContext cxt)
    {
        _curIndex++;
        if(_curIndex >= listPos.Count)
        {
            _curIndex = 0;
        }
        transform.position = listPos[_curIndex].position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
