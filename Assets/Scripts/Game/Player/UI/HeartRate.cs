using UnityEngine;

public class HeartRate : MonoBehaviour
{
    public int pointCount = 200;
    public float speed = 2f;
    public float amplitude = 1f;
    public float xCnt = 1f;
    public float timeCnt = 1f;
    public float drawSpeed = 1f;


    LineRenderer line;
    //[SerializeField] LineRenderer line;
    private float[] values;
    private float time;
    float shiftTimer;

    Vector3 _startPos;
    void Start()
    {
        shiftTimer = 0;
        _startPos = transform.position;
        line = GetComponent<LineRenderer>();
        line.positionCount = pointCount;
        values = new float[pointCount];

        // 색상/알파 그라디언트 세팅
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.green, 0.0f),
                new GradientColorKey(Color.green, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.0f, 0.0f), // 맨 뒤는 투명
                new GradientAlphaKey(1.0f, 0.5f), // 중간쯤 선명
                new GradientAlphaKey(1.0f, 1.0f)  // 맨 앞은 불투명
            }
        );
        line.colorGradient = gradient;
    }

    float ECGWave(float t)
    {
        return
            Mathf.Exp(-Mathf.Pow((t - 0.1f) * 20f, 2)) * 0.3f + // P파
            -Mathf.Exp(-Mathf.Pow((t - 0.25f) * 100f, 2)) * 1.0f + // Q파
            Mathf.Exp(-Mathf.Pow((t - 0.3f) * 200f, 2)) * 2.0f + // R파
            -Mathf.Exp(-Mathf.Pow((t - 0.35f) * 100f, 2)) * 0.5f + // S파
            Mathf.Exp(-Mathf.Pow((t - 0.5f) * 50f, 2)) * 0.4f; // T파
    }

    void Update()
    {
        time += Time.deltaTime * speed;
        //float healthFactor = Mathf.Clamp01(1);

        //float heartbeat = Mathf.Exp(-Mathf.Pow((time % 1f) * 10f - 5f, 2)) * amplitude;
        //heartbeat += Mathf.PerlinNoise(time * 5f, 0f) * (1f - healthFactor) * 0.5f;
        time = (time * timeCnt) % 1f; // 0~1 사이에서 ECG 모양 반복
        float heartbeat =
            Mathf.Exp(-Mathf.Pow((time - 0.1f) * 20f, 2)) * 0.2f +   // P
            -Mathf.Exp(-Mathf.Pow((time - 0.25f) * 100f, 2)) * 0.3f + // Q
            Mathf.Exp(-Mathf.Pow((time - 0.3f) * 200f, 2)) * 1.0f +   // R
            -Mathf.Exp(-Mathf.Pow((time - 0.35f) * 100f, 2)) * 0.5f + // S
            Mathf.Exp(-Mathf.Pow((time - 0.5f) * 50f, 2)) * 0.3f;     // T
        heartbeat *= amplitude;

        shiftTimer += Time.deltaTime;
        if (shiftTimer >= 1f / drawSpeed)   // drawSpeed = 화면에 그려지는 속도 (샘플링 속도)
        {
            shiftTimer = 0f;

            // Shift (한 칸씩 밀기)
            for (int i = 0; i < pointCount - 1; i++)
                values[i] = values[i + 1];

            // 새 값 삽입
            values[pointCount - 1] = heartbeat;

            // 라인 업데이트
            for (int i = 0; i < pointCount; i++)
                line.SetPosition(i, _startPos + new Vector3(i * xCnt, values[i], 0));
        }
    }
}
