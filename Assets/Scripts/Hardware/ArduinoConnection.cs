using UnityEngine;
using System.IO.Ports;
using System;
using System.Collections.Generic;
using UnityEngine.Events; // UnityEvent 사용을 위해 추가

// Dictionary<string, int>를 인자로 받는 UnityEvent 정의
// 이렇게 사용자 정의 UnityEvent 타입을 선언해야 인스펙터에서 Dictionary<string, int>를 인자로 받는 메서드를 선택할 수 있습니다.
[Serializable]
public class SensorDataUpdateEvent : UnityEvent<Dictionary<string, int>> { }

// MyDictionaryEntry와 SerializableDictionary 클래스는 동일하게 유지

// Key-Value 쌍을 직렬화하기 위한 Serializable 클래스
[Serializable]
public class MyDictionaryEntry<TKey, TValue>
{
    public TKey Key;
    public TValue Value;

    public MyDictionaryEntry(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}

// Dictionary를 Inspector에서 보이게 하기 위한 Wrapper 클래스
[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<MyDictionaryEntry<TKey, TValue>> _entries = new List<MyDictionaryEntry<TKey, TValue>>();

    // 직렬화 시 호출됨: Dictionary 내용을 List로 변환
    public void OnBeforeSerialize()
    {
        _entries.Clear();
        foreach (var pair in this)
        {
            _entries.Add(new MyDictionaryEntry<TKey, TValue>(pair.Key, pair.Value));
        }
    }

    // 역직렬화 시 호출됨: List 내용을 Dictionary로 변환
    public void OnAfterDeserialize()
    {
        this.Clear();
        foreach (var entry in _entries)
        {
            if (entry.Key != null && !this.ContainsKey(entry.Key)) // 중복 키 방지
            {
                this.Add(entry.Key, entry.Value);
            }
        }
    }
}


public class ArduinoConnection : MonoBehaviour
{
    // === UnityEvent 정의 ===
    // 인스펙터에서 할당 가능하도록 SerializeField로 표시합니다.
    [SerializeField]
    private SensorDataUpdateEvent _onSensorDataUpdated = new SensorDataUpdateEvent();
    public SensorDataUpdateEvent OnSensorDataUpdated => _onSensorDataUpdated; // 외부에서 접근용 프로퍼티

    public string portName = "COM4";
    public int baudRate = 9600;
    private string buffer = "";

    // 기존의 SerializableDictionary<string, int> data는 그대로 유지
    public SerializableDictionary<string, int> data;

    private SerialPort serialPort;
    private bool isPortOpen = false;

    void Start()
    {
        data = new SerializableDictionary<string, int>(); // Start에서 초기화
        OpenSerialPort();
    }

    void OpenSerialPort()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 1;
            serialPort.Open();
            isPortOpen = true;
            Debug.Log($"Serial Port {portName} Opened successfully.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to open serial port {portName}: {e.Message}");
            isPortOpen = false;
        }
    }

    void Update()
    {
        if (isPortOpen && serialPort.IsOpen)
        {
            try
            {
                while (serialPort.BytesToRead > 0)
                {
                    char ch = (char)serialPort.ReadChar();
                    if (ch == '\n') // 줄 끝 (LF)
                    {
                        ParseData(buffer.Trim());
                        buffer = "";
                    }
                    else
                    {
                        buffer += ch;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Serial read error: {e.Message}");
            }
        }
    }

    void ParseData(string str)
    {
        Debug.Log($"[RAW] {str}");
        string[] keyValuePairs = str.Split(';');
        bool dataChanged = false; // 데이터 변경 여부 플래그

        foreach (string pair in keyValuePairs)
        {
            string[] parts = pair.Split(':');
            if (parts.Length == 2 && int.TryParse(parts[1], out int val))
            {
                // 데이터가 실제로 변경되었는지 확인
                if (!data.ContainsKey(parts[0]) || data[parts[0]] != val)
                {
                    data[parts[0]] = val;
                    dataChanged = true; // 변경되었으면 플래그 설정
                }
            }
        }

        if (dataChanged)
        {
            _onSensorDataUpdated?.Invoke(new Dictionary<string, int>(data));
        }
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
            Debug.Log($"Serial Port {portName} closed.");
        }
    }
}