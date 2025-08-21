using UnityEngine;

public class RadioTower : MonoBehaviour
{
    public delegate void BroadcastListener(Transform target);
    BroadcastListener _listeners;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<GameCharacter>().ConnectRadio(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void AddEventListener(BroadcastListener listener)
    {

    }

    public void BroadcastTarget(Transform target)
    {
        _listeners(target);
    }
}
