using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour, ISerializationCallbackReceiver
{
    public float radius;
    public float angle;
    //public List<>

    [SerializeField] LayerMask _targetMask;
    [SerializeField] LayerMask _obstructionMask;

    /// <summary>
    /// <para>0 => Wall</para>
    /// <para>1 => Friend</para>
    /// <para>2 => Enemy</para>
    /// </summary>
    public List<Transform>[] Targets;

    [SerializeField] List<Transform> a;
    [SerializeField] List<Transform> b;
    [SerializeField] List<Transform> c;

    Collider[] _checks;

    public void OnBeforeSerialize()
    {
        if(VisibleObject(0)) a = Targets[0];
        if(VisibleObject(1)) b = Targets[1];
        if(VisibleObject(2)) c = Targets[2];
    }

    public void OnAfterDeserialize()
    {
        //throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Targets = new List<Transform>[3];
        for (int i = 0; i < Targets.Length; i++)
        {
            Targets[i] = new List<Transform>();
        }

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

    public bool VisibleObject(int idx=-1)
    {
        if (Targets == null) return false;
        if (idx < 0) return Targets[0].Count * Targets[1].Count * Targets[2].Count > 0;
        return Targets[idx].Count > 0;
    }

    void clearArray()
    {
        Targets[0].Clear();
        Targets[1].Clear();
        Targets[2].Clear();
    }

    void FOVCheck()
    {
        int hits = Physics.OverlapSphereNonAlloc(transform.position, radius, _checks, _targetMask);
        if (hits != 0)
        {
            clearArray();
            Transform target;
            for (int i = 0; i < hits; i++)
            {
                target = _checks[i].transform;
                if (transform.GetInstanceID() == target.GetInstanceID()) continue;
                Vector3 dir = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dir) < angle / 2)
                {
                    if (!Physics.Linecast(transform.position, target.position, out RaycastHit hit, _obstructionMask))
                    {
                        if (target.gameObject.layer == gameObject.layer) Targets[1].Add(target);
                        else Targets[2].Add(target);
                    } else if(hit.transform.GetInstanceID() == target.GetInstanceID() && target.gameObject.layer == 7) Targets[0].Add(target);

                    //if (Physics.Raycast(transform.position, dir, 500f, ~_obstructionMask)) Targets.Add(target);
                }
            }
        }
        else if (VisibleObject()) clearArray();
    }
}
