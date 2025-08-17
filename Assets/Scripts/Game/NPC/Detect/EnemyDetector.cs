using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    LayerMask _targetLayer;
    Transform _lastTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameObject.layer == 12) _targetLayer = 8;
        else _targetLayer = 12;
        _lastTarget = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(_lastTarget != null)
        {

        }
    }

    public Vector3 GetTargetPos() { return _lastTarget.position; }

    public Transform GetTarget() {  return _lastTarget; }

    public bool DetectEnemy(List<Transform> targets)
    {
        if(targets.Count == 0) return false;
        Vector3 agentPos = transform.position;
        
        float minSqrDistance = float.MaxValue;
        if (_lastTarget != null) minSqrDistance = (agentPos - _lastTarget.position).sqrMagnitude;
        
        for (int i = 0; i < targets.Count; i++)
        {
            if (_lastTarget != null && targets[i].GetInstanceID() == _lastTarget.GetInstanceID()) break;
            Vector3 targetPos = targets[i].transform.position;
            Vector3 dirToPlayer = (agentPos - targetPos).normalized;
            if (Physics.Raycast(targetPos, dirToPlayer, 500f, _targetLayer))
            {
                float sqrDistance = (agentPos - targetPos).sqrMagnitude;

                if (sqrDistance < minSqrDistance)
                {
                    minSqrDistance = sqrDistance;
                    _lastTarget = targets[i].transform;
                }
            }
        }
        return _lastTarget != null;
    }
}
