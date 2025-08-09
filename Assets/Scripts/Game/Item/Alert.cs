using UnityEngine;

public class Alert : MonoBehaviour
{
    [SerializeField] bool _isActivate;
    Material _alertLightMaterial;

    Color _color;
    float _colorIntensity;
    float _colorThreshold;
    float _colorTarget;
    const float _colorMaxTarget = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isActivate = false;

        _alertLightMaterial = transform.GetChild(2).GetComponent<MeshRenderer>().material;
        _alertLightMaterial.EnableKeyword("_EMISSION");

        _color = _alertLightMaterial.GetColor("_EmissionColor");
        _colorTarget = _colorMaxTarget;
        _colorIntensity = 0f;
        _colorThreshold = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(_isActivate)
        {
            _colorThreshold += Time.deltaTime;
            _alertLightMaterial.SetColor("_EmissionColor", _color * Mathf.Lerp(_colorIntensity, _colorTarget, _colorThreshold));
            if(_colorThreshold > 1.05f)
            {
                _colorIntensity = _colorTarget;
                _colorTarget = _colorMaxTarget - _colorTarget;
                _colorThreshold = 0f;
            }
        }
    }
}
