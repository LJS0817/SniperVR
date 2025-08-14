using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TaggableObject : MonoBehaviour
{
    [SerializeField] Transform _tag;
    [SerializeField] Transform _tagPos;
    [SerializeField] Transform _player;

    int _distance;
    DetectionIndicator _indicator;

    Vector3 _defaultScale;

    //const float MAX_SCALE = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _defaultScale = Vector3.zero;
        _tag = null;
        _distance = 0;
        if (_tagPos == null) { 
            _tagPos = transform.parent.GetChild(0);
            _indicator = transform.parent.GetComponent<DetectionIndicator>();
            _indicator.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_tag == null) return;
        _tag.LookAt(_player);

        float scale = (0.1f * _distance) / Scope.ZOOM;

        _tag.localScale = _defaultScale * scale;
    }

    public bool HasTag()
    {
        return _tagPos.childCount > 0;
    }

    public void SetTag(Transform t, Transform p)
    {
        _player = p;
        _tag = t;

        _tag.localScale = new Vector3(1, 1, 1);
        _tag.parent = _tagPos;
        _defaultScale = _tag.localScale;
        _tag.localPosition = Vector3.zero;
        _tag.rotation = Quaternion.identity;
        if(_indicator != null)
        {
            _indicator.enabled = true;
        }
    }

    public void SetDistance(int dist)
    {
        if (dist < 0) return;
        _distance = dist;
    }
}
