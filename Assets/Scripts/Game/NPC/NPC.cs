using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum NPC_STATE { E_SEARCH, E_CHASE, E_COVER, E_PEEK, E_ATTACK, E_ATTACKING, E_DEAD }

    [SerializeField] NPC_STATE _state;
    [SerializeField] NPCManager.TYPE _type;
    
    NPCController _controller;
    int hp = 100;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _controller = NPCManager.GetNPCController(_type);
    }

    // Update is called once per frame
    void Update()
    {
        _controller.Update();
    }

    public void Attacked(int att)
    {
        hp -= att;
        if (hp <= 0) SetState(NPC_STATE.E_DEAD);
    }

    public NPC_STATE GetState()
    {
        return _state;
    }

    public void SetState(NPC_STATE state)
    {
        _state = state;
    }
}
