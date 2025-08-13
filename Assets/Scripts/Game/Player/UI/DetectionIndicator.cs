using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectionIndicator : MonoBehaviour
{
    RectTransform _indicator;
    Image _image;
    [SerializeField] bool _isEnabled;
    Transform _scope;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(_isEnabled)
        {
            Vector3 dir = (Camera.main.WorldToScreenPoint(_scope.position) - Camera.main.WorldToScreenPoint(transform.position)).normalized;
            Debug.Log(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            _indicator.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg);
        }
    }

    public void SetIndicatorImage(RectTransform rect, Transform parent, Transform scope)
    {
        _indicator = Instantiate(rect, parent);
        _image = _indicator.GetChild(0).GetComponent<Image>();
        _indicator.gameObject.SetActive(false);
        _scope = scope;
    }

    //public void SetActiveIndicator(bool b)
    //{
    //    _isEnabled = b;
    //}

    private void OnBecameVisible()
    {
        Debug.Log("ASDASD");
        if (!_isEnabled)
        {
            _isEnabled = true;
            _indicator.gameObject.SetActive(_isEnabled);
        }
    }

    private void OnBecameInvisible()
    {
        Debug.Log("2315234");
        if (_isEnabled)
        {
            _isEnabled = false;
            _indicator.gameObject.SetActive(_isEnabled);
        }
    }
}
