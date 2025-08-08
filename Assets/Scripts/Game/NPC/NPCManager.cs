using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public enum TYPE
    {
        INNOCENT,

        FRIEND,

        ENEMY,
        BOSS,
    }

    public static NPCController GetNPCController(TYPE t)
    {
        switch(t)
        {
            default:
                return new Enemy();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
