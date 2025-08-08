using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] NPCManager.TYPE _type;
    NPCController _controller;
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
}
