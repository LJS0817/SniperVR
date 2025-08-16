using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectionIndicator : MonoBehaviour
{
    RectTransform _indicator;
    Image _image;
    [SerializeField] bool _isEnabled;
    Camera _cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        if (_cam == null) return;
        Vector3 screenPosition = _cam.WorldToScreenPoint(transform.position);
        if (screenPosition.z > 0 && screenPosition.x > 0 && screenPosition.x < _cam.pixelWidth && screenPosition.y > 0 && screenPosition.y < _cam.pixelHeight)
        {
            //Debug.Log("2315234");
            if (_isEnabled)
            {
                _isEnabled = false;
                _indicator.gameObject.SetActive(_isEnabled);
            }
        }
        else
        {
            //Debug.Log("ASDASD");
            if (!_isEnabled)
            {
                _isEnabled = true;
                _indicator.gameObject.SetActive(_isEnabled);
            }
        }

        if (_isEnabled)
        {
            Vector3 halfScreen = new Vector3(_cam.pixelWidth, _cam.pixelHeight) / 2;

            screenPosition -= halfScreen;

            if (screenPosition.z < 0)
            {
                screenPosition *= -1;
            }

            float angle = Mathf.Atan2(screenPosition.y, screenPosition.x);

            _indicator.localEulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg - 90);
        }
    }

    public void Dead()
    {
        _isEnabled = false;
        _indicator.gameObject.SetActive(false);
        enabled = false;
    }

    public void SetIndicatorImage(RectTransform rect, Transform parent, ref Camera cam)
    {
        _indicator = Instantiate(rect, parent);
        _image = _indicator.GetChild(0).GetComponent<Image>();
        _indicator.gameObject.SetActive(false);
        _cam = cam;
    }
}
