using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TaggableObject : MonoBehaviour
{
    [SerializeField] Transform _tag;
    [SerializeField] TextMeshPro _distText;
    [SerializeField] Transform _tagPos;
    [SerializeField] Transform _player;

    int _distance;

    const float MIN_SCALE = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _tag = null;
        _distance = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(_tag == null) return;
        _tag.LookAt(_player);

        float scale = _distance / 50;
        if(scale < MIN_SCALE) scale = MIN_SCALE;
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

        _distText = t.GetChild(1).GetComponent<TextMeshPro>();
    }

    public void SetDistance(int dist)
    {
        if (dist < 0) _distText.text = "";
        else
        {
            _distance = dist;
            _distText.text = _distance + " m";
        }
    }
}
