using UnityEngine;

public class Radio : MonoBehaviour
{
    [SerializeField] RadioTower _tower;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void ConnectRadioTower(RadioTower tower,RadioTower.BroadcastListener callback)
    {
        _tower = tower;
        _tower.AddEventListener(callback);
    }

    public void FindTarget(Transform t)
    {
        _tower.BroadcastTarget(t);
    }

    public void Disable()
    {
        enabled = false;
    }
}
