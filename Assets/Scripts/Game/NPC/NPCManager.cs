using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] Transform _player;
    [SerializeField] List<Transform> NPCList;

    [SerializeField] RectTransform _detectImageOri;
    [SerializeField] Transform _canvas;
    [SerializeField] Transform _scope;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < NPCList.Count; i++)
        {
            for(int j = 0; j <  NPCList[i].childCount; j++)
            {
                NPCList[i].GetChild(j).GetComponent<DetectionIndicator>().SetIndicatorImage(_detectImageOri, _canvas, _scope);
                NPCList[i].GetChild(j).GetComponent<CoverController>().playerTransform = _player;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
