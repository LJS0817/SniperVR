using System.Collections;
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
    private float[] values;
    private float time;
    float shiftTimer;

    Gradient gradient;

    public float a = 1f;
    public float b = 1f;
    public float tSpd = 1f;
    bool change = false;

    void Start()
    {
        shiftTimer = 0;
        line = GetComponent<LineRenderer>();
        line.positionCount = pointCount;
        values = new float[pointCount];

        // 색상/알파 그라디언트 세팅 (시작 위치만 설정)
        gradient = new Gradient();
        line.colorGradient = gradient;

        HeartBeat();
    }

    float ECGWave(float t)
    {
        float value = 0f;

        // P 파 (P-wave)
        if (t >= 0.1f && t <= 0.2f)
        {
            float pTime = (t - 0.1f) / 0.1f;
            value = 0.2f * Mathf.Sin(pTime * Mathf.PI);
        }

        // QRS 복합체 (QRS complex)
        else if (t > 0.25f && t <= 0.3f)
        {
            // Q 파
            if (t <= 0.27f)
            {
                float qTime = (t - 0.25f) / 0.02f;
                value = -0.3f * qTime;
            }
            // R 파
            else if (t <= 0.28f)
            {
                float rTime = (t - 0.27f) / 0.01f;
                value = -0.3f + 1.0f * rTime;
            }
            // S 파
            else
            {
                float sTime = (t - 0.28f) / 0.02f;
                value = 0.7f - 1.2f * sTime;
            }
        }

        // T 파 (T-wave)
        else if (t > 0.4f && t <= 0.6f)
        {
            float tTime = (t - 0.4f) / 0.2f;
            value = 0.3f * Mathf.Sin(tTime * Mathf.PI);
        }

        return value;
    }

    void HeartBeat()
    {
        time = 0f;


        for (int i = 0; i < pointCount; i++)
        {
            time += Time.deltaTime * speed;
            time = (time * timeCnt) % 1f;

            float heartbeat = ECGWave(time);
            heartbeat *= amplitude;
            line.SetPosition(i, new Vector3(i * xCnt, heartbeat, 0));
        }
        change = true;
    }

    void Update()
    {
        //time += Time.deltaTime * speed;
        //time = (time * timeCnt) % 1f;
        //float heartbeat = ECGWave(time);
        //heartbeat *= amplitude;

        UpdateGradient();
        //shiftTimer += Time.deltaTime;

        //if (shiftTimer >= 1f / drawSpeed)
        //{
        //    shiftTimer = 0f;

        //    //// Shift
        //    //for (int i = 0; i < pointCount - 1; i++)
        //    //    values[i] = values[i + 1];

        //    //// 새 값 삽입
        //    //values[pointCount - 1] = heartbeat;

        //    //// 라인 업데이트
        //    //for (int i = 0; i < pointCount; i++)
        //    //    line.SetPosition(i, new Vector3(i * xCnt, values[i], 0));

        //    // 그라디언트 업데이트 로직
        //    UpdateGradient();
        //}
    }

    void UpdateGradient()
    {
        // 0부터 1까지의 정규화된 시간 값
        float normalizedTime = Mathf.Repeat(Time.time * tSpd, 1.6f);
        if(normalizedTime < 1.1f)
        {
            if (change) change = false;

            // 알파 키(AlphaKey) 설정
            gradient.SetKeys(
                new GradientColorKey[] {
                new GradientColorKey(Color.green, 0.0f),
                new GradientColorKey(Color.green, 1.0f)
                },
                new GradientAlphaKey[] {
                new GradientAlphaKey(0.0f, normalizedTime),
                new GradientAlphaKey(1.0f, normalizedTime + a), // 이 값을 조절해서 선명한 구간의 길이를 변경할 수 있습니다.
                new GradientAlphaKey(0.0f, normalizedTime + a + b)
                }
            );
            line.colorGradient = gradient;
        } else if(normalizedTime > 1.5f && !change)
        {
            Debug.Log("ASDASD");
            HeartBeat();
        }
    }
}