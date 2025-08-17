using UnityEngine;

public class NPC : MonoBehaviour, ISerializationCallbackReceiver
{
    public enum NPC_STATE { E_SEARCH, E_CHASE, E_COVER, E_PEEK, E_AIMING, E_ATTACK, E_DEAD }

    [SerializeField] NPC_STATE _state;
    //[SerializeField] NPCManager.TYPE _type;

    public delegate void ChangeState(NPC_STATE state);
    
    NPCController _controller;
    int hp = 100;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _controller = GetComponent<NPCController>();
        _controller.AddEvent((NPC_STATE state) => { _state = state; });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attacked(int att)
    {
        hp -= att;
        if (hp <= 0) _controller.ChangeNPCState(NPC_STATE.E_DEAD);
    }

    public void OnBeforeSerialize()
    {
        //throw new System.NotImplementedException();
    }

    public void OnAfterDeserialize()
    {
        if(_controller != null) _controller.ChangeNPCState(_state);
    }
}
