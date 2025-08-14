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

    [SerializeField] Transform _player;
    [SerializeField] List<Transform> NPCList;

    [SerializeField] RectTransform _detectImageOri;
    [SerializeField] Transform _canvas;
    [SerializeField] Camera _cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < NPCList.Count; i++)
        {
            for(int j = 0; j <  NPCList[i].childCount; j++)
            {
                NPCList[i].GetChild(j).GetComponent<NPCController>().Init(_player, _detectImageOri, _canvas, ref _cam);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
