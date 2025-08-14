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

    Collider[] _checks;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Targets = new List<Transform>();
        _checks = new Collider[64];
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
        int hits = Physics.OverlapSphereNonAlloc(transform.position, radius, _checks, _targetMask);
        if (hits != 0)
        {
            Targets.Clear();
            Transform target;
            for (int i = 0; i < hits; i++)
            {
                target = _checks[i].transform;
                if (transform.GetInstanceID() == target.GetInstanceID()) continue;
                Vector3 dir = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dir) < angle / 2)
                {
                    if (!Physics.Linecast(transform.position, target.position, _obstructionMask)) Targets.Add(target);
                    //if (Physics.Raycast(transform.position, dir, 500f, ~_obstructionMask)) Targets.Add(target);
                }
            }
        }
        else if (Targets.Count > 0) Targets.Clear();
    }
}
