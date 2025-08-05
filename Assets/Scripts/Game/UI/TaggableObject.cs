using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TaggableObject : MonoBehaviour
{
    [SerializeField] Transform _tag;
    [SerializeField] Transform _tagPos;
    [SerializeField] Transform _player;

    int _distance;

    [SerializeField] float s = 1f;
    //const float MAX_SCALE = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _tag = null;
        s = 1f;
        _distance = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(_tag == null) return;
        _tag.LookAt(_player);

        float scale = _distance / s;
        //if(scale < MAX_SCALE) scale = MIN_SCALE;
        _tag.localScale = new Vector3(scale, scale, 1f);
    }

    public bool HasTag()
    {
        return _tag != null;
    }

    public void SetTag(Transform t, Transform p)
    {
        _player = p;
        _tag = t;

        _tag.parent = _tagPos;
        _tag.localPosition = Vector3.zero;
        _tag.rotation = Quaternion.identity;
    }

    public void SetDistance(int dist)
    {
        if (dist < 0) return;
        _distance = dist;
    }
}
