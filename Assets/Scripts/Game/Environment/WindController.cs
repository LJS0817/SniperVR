using UnityEngine;

public class WindController : MonoBehaviour
{
    [SerializeField] Vector3 _windDirection = new Vector3(1, 0, 0);
    [SerializeField] float _windStrength = 0.5f;

    public static WindController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public Vector3 GetWindForceAtPosition()
    {
        return _windDirection.normalized * _windStrength;
    }
}