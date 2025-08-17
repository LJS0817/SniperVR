using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DetectionIndicator : MonoBehaviour
{
    public enum DETECT_TYPE { E_NORMAL, E_SEEK, E_FIND }

    [SerializeField] DETECT_TYPE _type;
    RectTransform _indicator;
    Image _image;
    [SerializeField] bool _isEnabled;
    float _detectSpeed;
    Camera _cam;
    float _targetFillAmount;
    public float _fillTime;

    private void OnDisable()
    {
        Debug.Log("SADKLJZXCVKLJSADIOMJLQ@:W#OJKEDM@!*)(#PIK@O");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _detectSpeed = 0.25f;
        _targetFillAmount = 1f;
        _isEnabled = false;
        _type = DETECT_TYPE.E_NORMAL;
    }

    // Update is called once per frame
    void Update()
    {
        if (_type == DETECT_TYPE.E_SEEK)
        {
            _fillTime += Time.deltaTime * _detectSpeed;
            _image.fillAmount = Mathf.Lerp(_image.fillAmount, _targetFillAmount, _fillTime);
            if(_fillTime >= 1f)
            {
                _type = _targetFillAmount > 0 ? DETECT_TYPE.E_FIND : DETECT_TYPE.E_NORMAL;
            }
        }
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
        _image.fillAmount = 0f;
        _type = DETECT_TYPE.E_NORMAL;
        enabled = false;
    }

    public void ResetIndicator()
    {
        _targetFillAmount = 0f;
        _type = DETECT_TYPE.E_SEEK;
    }

    public void Seek()
    {
        if (_type == DETECT_TYPE.E_SEEK) return;
        _targetFillAmount = 1f;
        _type = DETECT_TYPE.E_SEEK;
    }

    public bool FindTarget() { return _type == DETECT_TYPE.E_FIND; }

    public void SetIndicatorImage(RectTransform rect, Transform parent, ref Camera cam)
    {
        _indicator = Instantiate(rect, parent);
        _image = _indicator.GetChild(1).GetComponent<Image>();
        _indicator.gameObject.SetActive(false);

        _targetFillAmount = 1f;
        _image.fillAmount = 0f;
        _type = DETECT_TYPE.E_NORMAL;

        _cam = cam;
    }
}
