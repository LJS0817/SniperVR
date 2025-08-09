using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    public float angle;
    //public List<>

    [SerializeField] LayerMask _targetMask;
    [SerializeField] LayerMask _obstructionMask;

    public List<Transform> Targets;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Targets = new List<Transform>();
        StartCoroutine(FOVRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FOVCheck();
        }
    }

    public bool VisibleObject()
    {
        return Targets != null && Targets.Count > 0;
    }

    void FOVCheck()
    {
        Collider[] checks = Physics.OverlapSphere(transform.position, radius, _targetMask);
        Transform target;
        if (checks.Length != 0)
        {
            Targets.Clear();
            for (int i = 0; i < checks.Length; i++)
            {
                target = checks[i].transform;
                Vector3 dir = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dir) < angle / 2)
                {
                    float dist = Vector3.Distance(transform.position, target.position);

                    if (Physics.Raycast(transform.position, dir, dist, ~_obstructionMask)) Targets.Add(target);
                }
            }
        }
        else if (Targets.Count > 0) Targets.Clear();
    }
}
